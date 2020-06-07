using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    [SerializeField] float distance = 10f;      //플레이어와의 거리
    [SerializeField] float height = 2f;         //카메라 높이
    [SerializeField] float sensivity = 2f;      //회전 민감도
    [SerializeField] float limitAngleY = 45f;   //x축에 대한 회전 제한각

    float currentCameraAngleX = 45f;            //현재 카메라 X축 각도
    float currentCameraAngleY = 0f;             //현재 카메라 Y축 각도

    public bool isCameraMove = false;           // 카메라가 다른 지점으로 이동해야하는지 체크

    private void LateUpdate()
    {
        // 카메라가 다른 지점으로 이동 중 일 경우 리턴
        if (isCameraMove)
            return;

        float mouseX = Input.GetAxisRaw("Mouse Y");
        float mouseY = Input.GetAxisRaw("Mouse X");

        currentCameraAngleX -= mouseX * sensivity;
        currentCameraAngleY += mouseY * sensivity;

        //x축에 대한 회전각 제한 1도~45도
        currentCameraAngleX = Mathf.Clamp(currentCameraAngleX, 1f, limitAngleY);

        //오일러각으로 변환
        Quaternion rotation = Quaternion.Euler(new Vector3(currentCameraAngleX, currentCameraAngleY, 0));

        transform.position = target.position - rotation * Vector3.forward * distance;
        transform.LookAt(target.position + Vector3.up * height);
    }
}
