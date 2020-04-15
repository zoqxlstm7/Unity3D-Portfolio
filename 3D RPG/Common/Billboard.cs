using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        // 항상 카메라 방향을 보도록 설정
        transform.LookAt(Camera.main.transform);
    }
}
