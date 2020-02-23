using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyController : MonoBehaviour
{
    float yAxis;
    Rigidbody rigidbody;

    void Start()
    {
        yAxis = transform.position.y;

        rigidbody = GetComponent<Rigidbody>();

        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
    }

    private void Update()
    {
        if (transform.position.y <= yAxis)
            transform.position = new Vector3(transform.position.x, yAxis, transform.position.z);
    }

    //중력 온오프
    public void SetUseGravity(bool useGravity)
    {
        if(useGravity)
        {
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
        }
        else
        {
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
        }
    }

    public void DirectionAddForce(Vector3 directoin, float force)
    {
        rigidbody.AddForce(directoin * rigidbody.mass * force, ForceMode.Impulse);
    }
}
