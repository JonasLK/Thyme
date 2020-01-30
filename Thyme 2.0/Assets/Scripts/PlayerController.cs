using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (CharacterController))]

public class PlayerController : MonoBehaviour {
    CharacterController cc;
    [SerializeField] Vector3 moveV3 = Vector3.zero;

    enum State {
        Normal,
        Dead,
        Dash,
        TPCC // third person character controller
    }

    [SerializeField] State curState = State.Normal;
    [Header ("Input")]
    [SerializeField] string horInput = "Horizontal";
    [SerializeField] string vertInput = "Vertical";
    [SerializeField] string jumpInput = "Jump";
    [SerializeField] string dashInput = "Dash";
    [Header ("Stats")]
    [SerializeField] float walkSpeed = 3;
    [SerializeField] float rotateSpeed = 12;
    [SerializeField] float accelerationSpeed = 5;
    [SerializeField] float gravityStrength = -20;
    [SerializeField] float gravitySpeed = 120;
    [SerializeField] float jumpStrength = 30;
    [Header("Camera")]
    [SerializeField] Transform holderCam = null;
    [SerializeField] GameObject actualCam = null;
    [SerializeField] public Vector3 camOffset;
    [SerializeField] float camSpeed = 5;

    void Start () {
        cc = GetComponent<CharacterController> ();
        angleGoal = transform.eulerAngles.y;
    }

    void Update () {
        switch (curState) {

            case State.Normal:
                FinalMove ();
                break;

            case State.TPCC:
                SetAngle ();
                MoveForward ();
                DashInput();

                Gravity ();
                JumpInput ();

                SetCamPos();

                FinalMove ();
                break;
            case State.Dash:
                SetAngle();
                DashForward();
                break;
        }
        
    }

    void FinalMove () {
        cc.Move (moveV3 * Time.deltaTime);
    }

    void DashInput()
    {
        if (Input.GetButtonDown(dashInput))
        {
            curState = State.Dash;
        }
    }

    void DashForward()
    {
        actualCam.GetComponent<CamShake>().StartCoroutine(actualCam.GetComponent<CamShake>().LowScreenShake());
        curState = State.TPCC;
    }

    //Inputs
    float GetHorInput () {
        return Input.GetAxis (horInput);
    }

    float GetVertInput () {
        return Input.GetAxis (vertInput);
    }

    // X and Z math

    float AnalMagnitude () {
        return Mathf.Min (1, Vector2.SqrMagnitude (new Vector2 (GetHorInput (), GetVertInput ())));
    }

    float AnalAngle () {
        return Mathf.Atan2 (GetVertInput (), -GetHorInput ()) * Mathf.Rad2Deg + holderCam.transform.eulerAngles.y - 90;
    }

    float curAcceleration = 0;
    void MoveForward () {
        curAcceleration = Mathf.MoveTowards (curAcceleration, AnalMagnitude (), Time.deltaTime * accelerationSpeed);
        Vector3 forwardHelper = transform.TransformDirection (0, 0, walkSpeed * curAcceleration);
        forwardHelper *= walkSpeed;
        moveV3.x = forwardHelper.x;
        moveV3.z = forwardHelper.z;

    }

    float angleGoal = 0;
    void SetAngle () {
        if (AnalMagnitude () > 0) {
            angleGoal = AnalAngle ();
        }

        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, angleGoal, 0), Time.deltaTime * rotateSpeed);
    }

    // Gravity math

    bool IsGrounded () {
        RaycastHit hit;
        if (Physics.SphereCast (transform.position, cc.radius * 1.1f, Vector3.down, out hit, cc.height / 2.1f, LayerMask.GetMask ("Default"), QueryTriggerInteraction.Ignore)) {
            CancelInvoke ("CayoteTime");
            Invoke ("CayoteTime", 0.2f);
        }

        if (moveV3.y > 0) {
            CancelInvoke ("CayoteTime");
        }

        return IsInvoking ("CayoteTime");
    }
    void CayoteTime () {
        //invoke function
    }

    void Gravity () {
        if (IsGrounded () == true) {
            moveV3.y = gravityStrength / 10;
        } else {
            if (moveV3.y > 0) {
                moveV3.y = Mathf.MoveTowards (moveV3.y, gravityStrength, Time.deltaTime * gravitySpeed); //going up
            } else {
                moveV3.y = Mathf.MoveTowards (moveV3.y, gravityStrength, Time.deltaTime * gravitySpeed * 2); //going down
            }
        }
    }

    void JumpInput () {
        if (Input.GetButtonDown (jumpInput)) {
            CancelInvoke ("JumpBuffer");
            Invoke ("JumpBuffer", 0.2f);
        }

        if (IsInvoking ("JumpBuffer") == true && IsGrounded () == true) {
            moveV3.y = jumpStrength;
            CancelInvoke("JumpBuffer");
        }
    }

    void JumpBuffer () {

    }

    //camera position

    void SetCamPos(){
        holderCam.transform.position = Vector3.Lerp(holderCam.transform.position, transform.position + camOffset,Time.deltaTime * camSpeed);
    }

}