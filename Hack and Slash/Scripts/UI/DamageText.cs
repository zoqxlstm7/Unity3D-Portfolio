using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour {

    Animator animator;

    float showTime;
    float showBetweenTime = 1f;
    bool isVisible = false;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        isVisible = true;
        animator.SetTrigger("BouncingText");
        showTime = Time.time + showBetweenTime;
    }

    private void Update()
    {
        if(Time.time > showTime && isVisible)
        {
            isVisible = false;
            transform.gameObject.SetActive(false);
        }
    }
}
