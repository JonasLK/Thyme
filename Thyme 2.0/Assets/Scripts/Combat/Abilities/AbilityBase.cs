using System;
using System.Collections;
using UnityEngine;

public class AbilityBase : MonoBehaviour
{
    public enum AbilityName
    {
        Laser,
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
    public float orbRadius;
    public float orbDelay;
    [Range(1f, 10f)]
    public float orbDur = 5f;
    [Range(1f, 10f)]
    public float orbDamage = 1f;

    [Header("Laser")]
    public float laserRadius;
    public float laserRange;
    public float laserDelay;
    [Range(1f, 10f)]
    public float laserDur = 5f;
    [Range(1f, 10f)]
    public float laserDamage = 1f;

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
                case AbilityName.TimeStop:
                    StopTimeOrb();
                    break;
            }
        }
    }

    public void ShootLaserBeam()
    {
        if(curDuration <= 0)
        {
            curRadius = laserRadius;
            player.SetAbility();
            StartCoroutine(ShootLaserAbility(curRadius, laserRange,laserDamage));
        }
    }

    public void StopTimeOrb()
    {
        curRadius = orbRadius;
        StartCoroutine(ShootOrb(curRadius, orbDamage));
    }

    public IEnumerator ShootLaserAbility(float rad, float range, float dam)
    {
        curDuration = laserDur;
        //LineRenderer curLaser = Instantiate(laser, handModel.transform.position,player.actualCam.rotation).GetComponent<LineRenderer>();
        while (curDuration > 0)
        {
            //curLaser.SetPosition(0, handModel.transform.position);
            //Fire Laser for Range
            if (Physics.SphereCast(handModel.transform.position, rad, player.actualCam.forward, out hit, range, interactable, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.tag == "Enemy")
                {
                    //hit.transform.GetComponent<EnemyInfo>().health -= damage;
                    yield return new WaitForSeconds(laserDelay);
                }
                currentHitDistance = hit.distance;
                currentHitObject = hit.transform.gameObject;
                //curLaser.SetPosition(curLaser.positionCount-1,hit.point);
            }
            else
            {
                //curLaser.SetPosition(curLaser.positionCount-1, player.actualCam.forward * range);
                currentHitDistance = range;
                currentHitObject = null;
            }
            yield return new WaitForEndOfFrame();
            curDuration -= Time.deltaTime;
        }
        //Destroy(curLaser.gameObject);
        player.ReturnState();
    }

    public IEnumerator ShootOrb(float rad, float dam)
    {
        curDuration = orbDur;
        while (curDuration > 0)
        {
            Collider[] enemies = Physics.OverlapSphere(handModel.transform.position, rad, interactable);
            foreach (Collider enemy in enemies)
            {
                if(enemy.tag == "Enemy")
                {
                    //TODO
                    //enemy.GetComponent<EnemyInfo>().health -= dam;
                    Debug.Log(enemy.name + "Hit");
                    yield return new WaitForSeconds(orbDelay);
                }
            }
            yield return new WaitForEndOfFrame();
            curDuration -= Time.deltaTime;
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
