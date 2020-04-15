using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMoter : MonoBehaviour
{
    NavMeshAgent agent;

    Transform target;       //대상을 따라감

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(target != null)
        {
            FaceTarget();
            MoveToPoint(target.position);
        }
    }

    //해당 지점으로 이동
    public void MoveToPoint(Vector3 point)
    {
        agent.SetDestination(point);
    }

    //타겟 추적
    public void FollowTarget(Interactable newTarget)
    {
        agent.stoppingDistance = newTarget.radius; ;
        agent.updateRotation = false;

        target = newTarget.interactionPoint.transform;
    }

    //타겟 소멸
    public void StopFollowingTarget()
    {
        agent.stoppingDistance = 0f;
        agent.updateRotation = true;

        target = null;
    }

    //타겟의 범위 내로 들어가도 타겟이 움직을때 타겟을 바라보도록함
    public void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }
}
