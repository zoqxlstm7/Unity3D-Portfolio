using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Motor : MonoBehaviour
{
    Rigidbody rb;
    Transform camTrn;

    public float speed = 5f;    //이동속도

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        camTrn = Camera.main.transform;

        // 리지드바디 각도 제한
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    // 플레이어 이동 및 회전 처리
    public void Move(float v, float h)
    {
        // 카메라 방향에 따른 플레이어 이동 처리
        Vector3 camForward = Vector3.Scale(camTrn.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 dir = camForward * v + camTrn.right * h;
        rb.MovePosition(transform.position + dir.normalized * speed * Time.deltaTime);

        // 플레이어 회전 처리
        if (dir.magnitude > 1f) dir.Normalize();
        dir = transform.InverseTransformDirection(dir);
        float turnAmount = Mathf.Atan2(dir.x, dir.z);
        transform.Rotate(0, turnAmount * 500f * Time.deltaTime, 0);
    }
}
