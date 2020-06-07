using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;        //따라갈 대상

    public Vector3 offset;          //카메라 위치 오프셋

    public float zoomSpeed = 4f;    //줌 속도
    public float minZoom = 5f;      //최소 줌 거리
    public float maxZoom = 15f;     //최대 줌 거리

    public float pitch = 2f;        //타겟과의 거리
    public float currentZoom = 15f; //현재 줌 거리

    private void Update()
    {
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        transform.position = target.position - offset * currentZoom;
        transform.LookAt(target.position + Vector3.up * pitch);
    }
}
