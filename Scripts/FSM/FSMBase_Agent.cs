using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FSMBase : MonoBehaviour
{
    //에이전트의 이동속도를 지정
    public void SetSpeed(float value)
    {
        agent.speed = value;
    }

    //에이전트의 목표위치를 지정
    public void SetDestination(Vector3 destination)
    {
        agent.destination = destination;
    }

    //에이전트가 정지되어있는지 상태를가져옴
    public bool GetIsStopAgent()
    {
        return agent.isStopped;
    }

    //에이전트의 정지유무를 설정
    public void SetAgentStop(bool isStop)
    {
        agent.isStopped = isStop;

        //정지될 경우 정지된 지점을 destination으로 지정
        if (!isStop)
            agent.destination = transform.position;
    }

    //에이전트 컴포넌트의 사용유무를 상태를 가져옴
    public bool GetAgentEnable()
    {
        return agent.enabled;
    }

    //에이전트 컴포넌트의 사용유무를 설정
    public void SetAgentEnable(bool isEnable)
    {
        agent.enabled = isEnable;
    }
}
