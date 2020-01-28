using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRay : MonoBehaviour
{
    [SerializeField] LayerMask interactable;
    [SerializeField] float camSpeed = 5f;
    // Update is called once per frame
    void Update()
    {
        if(rayHit().transform.tag != "Player" && rayHit().transform.tag != "Ground")
        {
            transform.position = Vector3.Lerp(transform.position, rayHit().point, Time.deltaTime * camSpeed);
        }
    }
    RaycastHit rayHit()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, interactable, QueryTriggerInteraction.Ignore);
        
        return hit;
    }
}
