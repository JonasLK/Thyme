using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    [Header("Climbing")]
    [SerializeField] float sphereDis = 0.25f;
    [SerializeField] float rayDis = 0.5f;
    [SerializeField] public bool hang;
    [SerializeField] public Transform offset;
    public Transform jumpUpPoint;
    public GameObject currentHitObject;
    private float currentHitDistance;
    PlayerMovement player;
    GameObject actualModel;
    private RaycastHit hit;
    bool hittingSide;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        if (!player)
        {
            Debug.LogError("No PlayerMovement Script Found");
        }
        else
        {
            actualModel = player.actualPlayer;
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckLedge();
    }

    private void CheckLedge()
    {
        if(GetComponent<Rigidbody>().velocity.y < 0.5f)
        {
            ShootSphere();
        }
        if (hang)
        {
            if (Input.GetButtonDown("Jump"))
            {
                //Play climb Animation
                StartCoroutine(LedgeCollider(hit.collider));
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<PlayerJump>().Jump();
            }
        }
    }

    private void ShootSphere()
    {
        if (Physics.SphereCast(offset.position, sphereDis, player.actualPlayer.transform.forward, out hit, rayDis, player.wallMask, QueryTriggerInteraction.UseGlobal))
        {
            //forDebugging
            currentHitDistance = hit.distance;
            currentHitObject = hit.transform.gameObject;

            if (hit.transform.tag == "Ledge")
            {
                GetComponent<PlayerMovement>().ResetAnime();
                GetComponent<PlayerMovement>().playerAnime.SetTrigger("isHanging");
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                hang = true;
            }
        }
        else
        {
            //forDebugging
            currentHitDistance = rayDis;
            currentHitObject = null;

            GetComponent<Rigidbody>().useGravity = true;
        }
    }

    private IEnumerator LedgeCollider(Collider ledge)
    {
        float ledgeCooldown = 0.3f;
        Collider currentLedge = ledge;
        while (currentLedge)
        {
            currentLedge.enabled = false;
            yield return new WaitForSeconds(ledgeCooldown);
            hang = false;
            currentLedge.enabled = true;
            currentLedge = null;
        }


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (actualModel)
        {
            Debug.DrawLine(offset.position, offset.position + actualModel.transform.forward * currentHitDistance);
            Gizmos.DrawWireSphere(offset.position + actualModel.transform.forward * currentHitDistance, sphereDis);
        }
    }
}
