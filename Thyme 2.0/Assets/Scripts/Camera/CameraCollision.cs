using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("Settings")]
    public float minDis = 1.0f;
    public float maxDis = 4.0f;
    public float smooth = 10.0f;
    Vector3 tempDir;
    public Vector3 tempDirSmooth;
    public LayerMask interact;
    public float dis;
    bool camRequest;

    void Awake()
    {
        tempDir = transform.localPosition.normalized;
        dis = transform.localPosition.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camPosWeWant = transform.parent.TransformPoint(tempDir * maxDis);
        RaycastHit hit;

        if(Physics.Linecast(transform.parent.position,camPosWeWant,out hit,interact,QueryTriggerInteraction.Ignore))
        {
            dis = Mathf.Clamp(hit.distance, minDis, maxDis);
        }
        else
        {
            dis = maxDis;
        }
        camRequest = true;
    }
    private void FixedUpdate()
    {
        if (camRequest)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, tempDir * dis, Time.fixedDeltaTime * smooth);
        }
    }
}
