using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerScript : MonoBehaviour
{
    public float MinYaw = -360;
    public float MaxYaw = 360;
    public float MinPitch = -60;
    public float MaxPitch = 60;
    public float LookSensitivity = 1;

    public float MoveSpeed = 10;
    public float SprintSpeed = 30;
    private float currMoveSpeed = 0;

    protected CharacterController movementController;
    protected Camera playerCamera;

    protected bool isControlling;
    protected float yaw;
    protected float pitch;

    protected Vector3 velocity;


    protected virtual void Start()
    {

        movementController = GetComponent<CharacterController>();   //  Character Controller
        playerCamera = GetComponentInChildren<Camera>();            //  Player Camera

        isControlling = true;
        ToggleControl();    //  Toggle Player control
    }

    protected virtual void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
            transform.position = new Vector3(3, 0, 0); // Hit "R" to spawn in this position

        Vector3 direction = Vector3.zero;
        direction += transform.forward * Input.GetAxisRaw("Vertical");
        direction += transform.right * Input.GetAxisRaw("Horizontal");

        direction.Normalize();

        if (movementController.isGrounded)
        {
            velocity = Vector3.zero;
        }
        else
        {
            velocity += -transform.up * (9.81f * 10) * Time.deltaTime; // Gravity
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {  // Player can sprint by holding "Left Shit" keyboard button
            currMoveSpeed = SprintSpeed;
        }
        else
        {
            currMoveSpeed = MoveSpeed;
        }

        direction += velocity * Time.deltaTime;
        movementController.Move(direction * Time.deltaTime * currMoveSpeed);

        // Camera Look
        yaw += Input.GetAxisRaw("Mouse X") * LookSensitivity;
        pitch -= Input.GetAxisRaw("Mouse Y") * LookSensitivity;

        yaw = ClampAngle(yaw, MinYaw, MaxYaw);
        pitch = ClampAngle(pitch, MinPitch, MaxPitch);

        transform.eulerAngles = new Vector3(0, yaw, 0.0f);
        playerCamera.transform.eulerAngles = new Vector3(pitch, yaw, 0);

        Vector3 tempPos = playerCamera.transform.localPosition;
        float cameraHeight = Map(pitch, MinPitch, MaxPitch, 1, 8);
        playerCamera.transform.localPosition = new Vector3(tempPos.x, cameraHeight ,tempPos.z);

    }   

    protected float ClampAngle(float angle)
    {
        return ClampAngle(angle, 0, 360);
    }

    protected float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    protected void ToggleControl()
    {

        playerCamera.gameObject.SetActive(isControlling);

#if UNITY_5
		Cursor.lockState = (isControlling) ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !isControlling;
#else
        Screen.lockCursor = isControlling;
#endif

    }

    protected float Map(float x, float x1, float x2, float y1, float y2)
    {
        var m = (y2 - y1) / (x2 - x1);
        var c = y1 - m * x1; // point of interest: c is also equal to y2 - m * x2, though float math might lead to slightly different results.

        return m * x + c;
    }

}
