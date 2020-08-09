using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T>
{
    #region Variables
    protected T owner;                      // 소유자
    protected StateMachine<T> stateMachine; // 상태 머신
    #endregion Variables

    #region Other Methods
    /// <summary>
    /// 소유자와 상태머신을 초기화하는 함수
    /// </summary>
    /// <param name="owner">소유자</param>
    /// <param name="stateMachine">상태머신</param>
    public void SetOwnerAndStateMachine(T owner, StateMachine<T> stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;

        // 상태 초기화 함수 실행
        OnInitialize();
    }

    // 상태 초기화 함수
    public virtual void OnInitialize() { }
    // 상태 진입시 처리 함수
    public virtual void OnEnter() { }
    // 상태 업데이트 처리 함수
    public abstract void OnUpdate(float deltaTime);
    // 상태 탈출시 처리 함수
    public virtual void OnExit() { }
    #endregion Other Methods
}

public sealed class StateMachine<T>
{
    #region Variables
    T owner;                            // 소유자

    State<T> currentState;              // 현재 상태
    public State<T> CurrentState => currentState;

    State<T> priviousState;             // 이전 상태
    public State<T> PriviousState => priviousState;

    float elapsedTimeInState = 0.0f;    // 상태 진입 후 흐른 시간
    public float ElapsedTimeInState => elapsedTimeInState;

    // 등록된 상태 객체
    Dictionary<System.Type, State<T>> states = new Dictionary<System.Type, State<T>>();
    #endregion Variables

    #region Other Methods
    /// <summary>
    /// 소유자와 초기상태 초기화 함수
    /// </summary>
    /// <param name="owner">소유자</param>
    /// <param name="initialState">초기 상태</param>
    public StateMachine(T owner, State<T> initialState)
    {
        this.owner = owner;

        // 초기 상태 등록
        RegistState(initialState);

        // 현재 상태 진입
        currentState = initialState;
        currentState.OnEnter();
    }

    /// <summary>
    /// 상태 등록 함수
    /// </summary>
    /// <param name="newState">등록할 상태</param>
    public void RegistState(State<T> newState)
    {
        // 등록되어 있다면 리턴
        if (states.ContainsKey(newState.GetType()))
            return;

        // 상태 객체 초기화 및 등록
        newState.SetOwnerAndStateMachine(owner, this);
        states.Add(newState.GetType(), newState);
    }

    /// <summary>
    /// 상태 업데이트 함수
    /// </summary>
    /// <param name="deltaTime">델타타임</param>
    public void UpdateState(float deltaTime)
    {
        // 흐른 시간 저장
        elapsedTimeInState += deltaTime;
        // 상태 업데이트 함수 호출
        currentState.OnUpdate(deltaTime);
    }

    /// <summary>
    /// 상태 변경 함수
    /// </summary>
    /// <typeparam name="R">상태타입</typeparam>
    /// <returns>변경된 상태</returns>
    public R ChangeState<R>() where R : State<T>
    {
        System.Type newType = typeof(R);

        // 등록된 상태가 아닌 경우 리턴
        if (!states.ContainsKey(newType))
            return null;

        // 같은 상태인 경우 현재 상태 그대로 리턴
        if(currentState.GetType() == newType)
        {
            return currentState as R;
        }

        // 현태 상태 탈출
        currentState.OnExit();

        // 새로운 상태로 현재 상태 변경
        priviousState = currentState;
        currentState = states[newType];

        // 흐른 시간 초기화 및 새로운 상태 진입
        elapsedTimeInState = 0.0f;
        currentState.OnEnter();

        return currentState as R;
    }
    #endregion Other Methods
}
