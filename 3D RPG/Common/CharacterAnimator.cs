using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class CharacterAnimator : MonoBehaviour
{
    protected Animator animator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    public abstract void MoveAnimation();       // 무브 애니메이션
    public abstract void AttackAnimation();     // 공격 애니메이션
    public abstract void TriggerAnimation(string animationName);            // trigger 파라미터 애니메이션
    public abstract void BoolAnimation(string animationName, bool isSet);   // bool 파라미터 애니메이션
}

public enum State
{
    IDLE,
    RUN,
    ATTACK,
    DODGE,
    TAKE_HIT,
    DIE
}
