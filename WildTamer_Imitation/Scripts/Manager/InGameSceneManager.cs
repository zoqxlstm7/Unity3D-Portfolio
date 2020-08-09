using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSceneManager : BaseSceneManager
{
    #region Variablse
    public const int MAX_TAMING_COUNT = 20;                 // 최대 테이밍 가능 수

    public Player player;

    public CacheManager cacheManager = new CacheManager();  // 전체 캐시 관리 객체
    public AnimalManager animalManager;                     // 동물 캐시 관리 객체
    public ProjectileManager projectileManager;             // 발사체 캐시 관리 객체
    public BombManager bombManager;                         // 폭탄 캐시 관리 객체
    public EffectManager effectManager;                     // 이펙트 캐시 관리 객체
    public AnimalDeadUIManager deadUIManager;               // 동물 사망시 노출 UI 관리 객체

    public SpawnManager spawnManager;                       // 스폰 매니저
    #endregion Variablse

    #region BaseScene Methods
    protected override void InitialzieScene()
    {   
    }

    protected override void UpdateScene()
    {
    }
    #endregion BaseScene Methods

    #region Other Methods
    #endregion Other Methods
}
