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
    public float laserRadius;
    public float orbRadius;
    public float range;
    [Range(1f,10f)]
    public float laserDamage = 1f;
    [Range(1f, 10f)]
    public float orbDamage = 1f;
    [Range(1f,10f)]
    public float duration = 5f;
    public LayerMask interactable;
    private PlayerMovement player;
    private float curRadius, curRange, curDuration;
    RaycastHit hit;

    [Header("Debugging")]
    public GameObject currentHitObject;
    private float currentHitDistance;
    private GameObject actualModel;
    public Transform handModel;

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
        curRadius = laserRadius;
        player.SetAbility();
        StartCoroutine(ShootAbility(curRadius, range,laserDamage));
    }

    public void StopTimeOrb()
    {
        curRadius = orbRadius;
        StartCoroutine(ShootAbility(curRadius, 0, orbDamage));
    }

    public IEnumerator ShootAbility(float radius, float range, float damage)
    {
        curDuration = duration;
        while (curDuration > 0)
        {
            //Fire Laser for Range
            if (Physics.SphereCast(handModel.transform.position, radius, player.actualCam.forward, out hit, range, interactable, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.tag == "Enemy")
                {
                    //hit.transform.GetComponent<EnemyInfo>().health -= damage;
                }
                currentHitDistance = hit.distance;
                currentHitObject = hit.transform.gameObject;
            }
            else
            {
                currentHitDistance = range;
                currentHitObject = null;
            }
            yield return new WaitForEndOfFrame();
            curDuration -= Time.deltaTime;
        }
        player.ReturnState();
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
