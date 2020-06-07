using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandContainer : MonoBehaviour
{
    [SerializeField] Transform parent;  // 총이 장착될 위치
    public Transform Parent
    {
        get => parent;
    }
}
