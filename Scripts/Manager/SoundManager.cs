using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Awake()
    {
        instance = this;
    }

    AudioSource player;

    public AudioClip attackSound;
    public AudioClip bowAttackSound;
    public AudioClip enemyAttackSound;
    public AudioClip skill01Sound;
    public AudioClip skill02Sound;

    private void Start()
    {
        player = GetComponent<AudioSource>();
    }

    //공격 사운드 반환
    public AudioClip AttackSound()
    {
        return attackSound; 
    }

    //활공격 사운드 반환
    public AudioClip BowAttackSound()
    {
        return bowAttackSound;
    }

    //적 공격 사운드 반환
    public AudioClip EnemyAttackSound()
    {
        return enemyAttackSound;
    }

    //스킬1 사운드 반환
    public AudioClip Skill01Sound()
    {
        return skill01Sound;
    }

    //스킬2 사운드 반환
    public AudioClip Skill02Sound()
    {
        return skill02Sound;
    }
}
