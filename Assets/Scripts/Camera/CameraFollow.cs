using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Camera myCamera;
    private Func<Vector3> GetCameraFollowPositionFunc;
    private Func<float> GetCameraZoomFunc;

    public void Setup(Func<Vector3> GetCameraFollowPositionFunc, Func<float> GetCameraZoomFunc, bool teleportToFollowPosition, bool instantZoom)
    {
        this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
        this.GetCameraZoomFunc = GetCameraZoomFunc;

        if (teleportToFollowPosition)
        {
            Vector3 cameraFollowPosition = GetCameraFollowPositionFunc();
            transform.position = cameraFollowPosition;
        }

        if (instantZoom)
        {
            myCamera.orthographicSize = GetCameraZoomFunc();
        }
    }

    private void Awake()
    {
        myCamera = transform.GetComponent<Camera>();
    }

    public void SetGetCameraFollowPositionFunc(Func<Vector3> GetCameraFollowPositionFunc)
    {
        this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
    }

    public void SetCameraFollowPosition(Vector3 cameraFollowPosition)
    {
        SetGetCameraFollowPositionFunc(() => cameraFollowPosition);
    }

    public void SetCameraZoom(float cameraZoom)
    {
        SetGetCameraZoomFunc(() => cameraZoom);
    }

    public void SetGetCameraZoomFunc(Func<float> GetCameraZoomFunc)
    {
        this.GetCameraZoomFunc = GetCameraZoomFunc;
    }


    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    public void RotateCamera()
    {
        var mouseX = Input.GetAxisRaw("Mouse X");
        var mouseY = Input.GetAxisRaw("Mouse Y");

        var cameraRotationSpeed = 40f;

        transform.RotateAround(transform.position, Vector3.up, mouseX * cameraRotationSpeed * Time.deltaTime);
        transform.Rotate(-mouseY * cameraRotationSpeed * Time.deltaTime, 0, 0);
    }

    private void HandleZoom()
    {
        if (GetCameraZoomFunc == null) return;
        float cameraZoom = GetCameraZoomFunc();

        float cameraZoomDifference = cameraZoom - myCamera.fieldOfView;
        float cameraZoomSpeed = 3f;

        myCamera.fieldOfView += cameraZoomDifference * cameraZoomSpeed * Time.deltaTime;

        if (cameraZoomDifference > 0)
        {
            if (myCamera.fieldOfView > cameraZoom)
            {
                myCamera.fieldOfView = cameraZoom;
            }
        }
        else
        {
            if (myCamera.fieldOfView < cameraZoom)
            {
                myCamera.fieldOfView = cameraZoom;
            }
        }
    }

    private void HandleMovement()
    {
        if (GetCameraFollowPositionFunc == null) return;
        Vector3 cameraFollowPosition = GetCameraFollowPositionFunc();
        float cameraMoveSpeed = 20f;

        transform.position += myCamera.transform.TransformDirection(cameraFollowPosition * cameraMoveSpeed * Time.deltaTime);
    }
}

