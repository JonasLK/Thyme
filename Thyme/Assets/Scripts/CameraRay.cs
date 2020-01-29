using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRay : MonoBehaviour
{
    [SerializeField] LayerMask interactable;
    public float camSpeed;
    public float range = 4f;
    public bool camRequest;
    RaycastHit hit;
    private void Update()
    {
        if(Physics.Raycast(transform.position,transform.forward,out hit,range, interactable, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.tag != "Player")
            {
                camRequest = true;
            }
        }   

    }

    void FixedUpdate()
    {
        if (camRequest)
        {
            transform.position = (transform.forward * Time.fixedDeltaTime *camSpeed);
        }
    }

}