using System;
using System.Collections;
using UnityEngine;

public class AbilityBase : MonoBehaviour
{
    public enum AbilityName
    {
        Laser,
        OrbDrain,
        TimeStop
    }

    [Header("Ability")]
    public AbilityName curAbility;

    [Header("AbilityProperties")]
    public LayerMask interactable;
    private PlayerMovement player;
    private float curRadius, curRange, curDuration;
    RaycastHit hit;

    [Header("Orb")]
    public float orbCooldown;
    public float orbRadius;
    public float orbDelay;
    [Range(1f, 10f)]
    public float orbDur = 5f;
    [Range(1f, 10f)]
    public float orbDam = 1f;

    [Header("Laser")]
    public float laserCooldown;
    public float laserRadius;
    public float laserRange;
    public float laserDelay;
    [Range(1f, 10f)]
    public float laserDur = 5f;
    [Range(1f, 10f)]
    public float laserDamage = 1f;

    [Header("TimeStop")]
    public float timeStopCooldown;
    public float slowDur;
    public float slowDelay;
    public float timeTick;

    [HideInInspector]
    public float curtimeStopCooldown;
    [HideInInspector]
    public float curLaserCooldown;
    [HideInInspector]
    public float curOrbCooldown;

    [Header("Debugging")]
    public GameObject currentHitObject;
    public Transform handModel;
    public GameObject laser;
    private GameObject actualModel;
    private float currentHitDistance;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        actualModel = player.actualPlayer;
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            switch (curAbility)
            {
                case AbilityName.Laser:
                    ShootLaserBeam();
                    break;
                case AbilityName.OrbDrain:
                    OrbDrain();
                    break;
                case AbilityName.TimeStop:
                    TimeStop();
                    break;
            }
        }
    }

    public void TimeStop()
    {
        if(!IsInvoking())
        {
            curDuration = slowDur;
            InvokeRepeating("SlowDownTime", 0, slowDelay);
            StartCoroutine(DurationTimer());
        }
    }

    public void SlowDownTime()
    {
        //Invoke Method
        if(GameManager.gameTime > 0.1f)
        {
            GameManager.gameTime -= timeTick * Time.deltaTime;
            print("SLOW MODE");
        }
    }

    public void OrbDrain()
    {
        if (!IsInvoking() && curDuration <= 0)
        {
            curDuration = orbDur;
            StartCoroutine(DurationTimer());
            
            if (curDuration > 0)
            {
                InvokeRepeating("CheckOrb", 0, orbDelay);
            }
        }
    }

    public void CheckOrb()
    {
        //Invoke Method
        Collider[] enemies = Physics.OverlapSphere(handModel.transform.position, orbRadius, interactable);
        foreach (Collider enemy in enemies)
        {
            if (enemy.tag == "Enemy")
            {
                enemy.GetComponent<EnemyInfo>().AdjustHealth(orbDam);
                Debug.Log(enemy.name + "Hit");
            }
        }
    }

    public void ShootLaserBeam()
    {
        if (IsInvoking() && curDuration <= 0)
        {
            curDuration = laserDur;
            curRadius = laserRadius;
            StartCoroutine(DurationTimer());
            if (curDuration > 0)
            {
                player.SetAbility();
                InvokeRepeating("CheckLaser", 0, laserDelay);
                //LineRenderer curLaser = Instantiate(laser, handModel.transform.position, player.actualCam.rotation).GetComponent<LineRenderer>();
                //curLaser.SetPosition(0, handModel.transform.position);
            }
        }
    }

    public void CheckLaser()
    {
        //Invoke Method
        if (Physics.SphereCast(handModel.transform.position, laserRadius, player.actualCam.forward, out hit, laserRange, interactable, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.tag == "Enemy")
            {
                if (!IsInvoking())
                {
                    hit.transform.GetComponent<EnemyInfo>().AdjustHealth(laserDamage);
                }
                Debug.Log(hit.transform.name);
            }
            currentHitDistance = hit.distance;
            currentHitObject = hit.transform.gameObject;
            //curLaser.SetPosition(curLaser.positionCount - 1, hit.point);
        }
        else
        {
            //curLaser.SetPosition(curLaser.positionCount - 1, player.actualCam.forward * range);
            currentHitDistance = laserRange;
            currentHitObject = null;
        }
    }

    public IEnumerator ShootLaser()
    {
        while (true)
        {
            
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator DurationTimer()
    {
        while (curDuration > 0)
        {
            curDuration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if (curDuration < 0)
            {
                CancelInvoke();
                player.ReturnState();
                GameManager.gameTime = 1f;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (actualModel)
        {
            Debug.DrawLine(handModel.transform.position, handModel.transform.position + player.actualCam.transform.forward * currentHitDistance);
            Gizmos.DrawWireSphere(handModel.transform.position + player.actualCam.transform.forward * currentHitDistance, curRadius);
        }
    }
}
