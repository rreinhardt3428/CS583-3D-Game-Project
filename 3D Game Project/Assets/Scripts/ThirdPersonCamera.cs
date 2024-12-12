using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCamera : MonoBehaviour
{
    //Create axis states to manage sensitivity, bounds, and inputs
    public Cinemachine.AxisState horizontalAxis;
    public Cinemachine.AxisState verticalAxis;
    //Grab the position of the camera orientation
    public Transform cameraOrientation;

    private float sensitivity;

    void Start()
    {
        //Make the cursor invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);

        horizontalAxis.m_MaxSpeed = 300f * sensitivity; // Adjust the multiplier (300f) if necessary
        verticalAxis.m_MaxSpeed = 300f * sensitivity;   // Adjust the multiplier (300f) if necessary
    }

    void Update()
    {
        //Get mouse movement input and apply values for vertical/horizontal rotations
        horizontalAxis.Update(Time.deltaTime);
        verticalAxis.Update(Time.deltaTime);
    }

    void LateUpdate()
    {
        //Actually updates the players rotation based for horionztal and vertical values
        cameraOrientation.localEulerAngles = new Vector3(verticalAxis.Value, cameraOrientation.localEulerAngles.y, cameraOrientation.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, horizontalAxis.Value, transform.eulerAngles.z);
    }
}
