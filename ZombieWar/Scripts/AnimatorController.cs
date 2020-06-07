using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController
{
    Animator animator;          // 애니메이터 객체
    string currnetAnimation;    // 현재 재생중인 애니메이션

    /// <summary>
    /// 초기화 함수
    /// </summary>
    /// <param name="animator">지정할 애니메이터 객체</param>
    public AnimatorController(Animator animator)
    {
        this.animator = animator;
    }

    /// <summary>
    /// bool형 애니메이션 실행
    /// </summary>
    /// <param name="name">실행할 애니메이션 이름</param>
    /// <param name="value">활성 비활성 여부</param>
    public void PlayAnimator(string name, bool value)
    {
        currnetAnimation = name;
        animator.SetBool(name, value);
    }

    /// <summary>
    /// trigger형 애니메이션 실행
    /// </summary>
    /// <param name="name">실행할 애니메이션 이름</param>
    public void PlayAnimator(string name)
    {
        if (!string.IsNullOrEmpty(currnetAnimation))
        {
            animator.SetBool(currnetAnimation, false);
        }
        
        animator.SetTrigger(name);
    }

    /// <summary>
    /// 애니메이터 객체 반환
    /// </summary>
    /// <returns></returns>
    public Animator GetAnimator()
    {
        return animator;
    }
}
