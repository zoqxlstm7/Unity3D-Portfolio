using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float aliveTime;        //발사체 생존 시간

    private void Start()
    {
        aliveTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - aliveTime > 3)
            Destroy(gameObject);

        transform.Translate(Vector3.forward * 20f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //대상이 적인 경우 데미지를 입힘
        if (other.gameObject.layer == LayerMask.NameToLayer("enemy"))
        {
            transform.GetComponentInParent<CharacterCombat>().DoDamage();

            Destroy(gameObject);
        }
    }
}
