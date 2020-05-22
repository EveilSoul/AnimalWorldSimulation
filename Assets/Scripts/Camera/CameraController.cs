using UnityEngine;
using Unity.Entities;
using System;

public class CameraController : MonoBehaviour {

    public static CameraController instance;

    [SerializeField] private CameraFollow cameraFollow;

    private Vector3 cameraFollowPosition;
    private float cameraFollowZoom;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        cameraFollowZoom = 40f;
        cameraFollowPosition = new Vector3(-25, 30, 15);
        cameraFollow.Setup(() => cameraFollowPosition, () => cameraFollowZoom, true, true);
    }

    private void Update()
    {
        HandleCamera();
    }


    private void HandleCamera() {
        Vector3 moveDir = Vector3.zero;

        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");

        cameraFollowPosition = new Vector3(h, 0, v);

        float zoomSpeed = 200f;
        if (Input.mouseScrollDelta.y > 0) cameraFollowZoom -= 1 * zoomSpeed * Time.deltaTime;
        if (Input.mouseScrollDelta.y < 0) cameraFollowZoom += 1 * zoomSpeed * Time.deltaTime;

        cameraFollowZoom = Mathf.Clamp(cameraFollowZoom, 10f, 80f);

        if (Input.GetMouseButton(1))
        {
            cameraFollow.RotateCamera();
        }
    }
}

[Serializable]
public struct ObjectDescription
{
    public string Name;
    public int Count;
    public Entity Prefab;
}








