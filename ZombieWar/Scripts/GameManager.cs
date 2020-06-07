using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SingleTone
    static GameManager instance;
    public static GameManager Instance
    {
        get => instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    // 현재 씬을 관리하는 객체 할당
    BaseSceneManager currentSceneManager;
    public BaseSceneManager CurrentSceneManager
    {
        set => currentSceneManager = value;
    }

    // 네트워크 관리 객체 반환
    [SerializeField] NetworkManager networkManager;
    public NetworkManager NetworkManager
    {
        get => networkManager;
    }

    // 씬 관리 객체 반환
    [SerializeField] SceneController sceneController;
    public SceneController SceneController
    {
        get => sceneController;
    }

    // 오디오 관리 객체 반환
    [SerializeField] SoundManager soundManager;
    public SoundManager SoundManager
    {
        get => soundManager;
    }

    // 캐릭터 모델 오브젝트
    [SerializeField] GameObject[] models;
    public GameObject[] Models
    {
        get => models;
    }

    // 총 모델 오브젝트
    [SerializeField] GameObject[] gunModels;
    public GameObject[] GunModels
    {
        get => gunModels;
    }

    // 선택된 캐릭터 모델 인덱스
    [SerializeField] int modelIndex;            
    public int ModelIndex
    {
        get => modelIndex;
        set => modelIndex = value;
    }
    // 선택된 총 모델 인덱스
    [SerializeField] int gunModelIndex;
    public int GunModelIndex
    {
        get => gunModelIndex;
        set => gunModelIndex = value;
    }             

    private void Start()
    {
        // 현재 씬을 관리하는 객체 캐싱
        BaseSceneManager baseSceneManager = FindObjectOfType<BaseSceneManager>();
        currentSceneManager = baseSceneManager;
    }

    /// <summary>
    /// 현재 씬을 관리하는 객체 반환
    /// </summary>
    /// <typeparam name="T">반환받을 타입</typeparam>
    /// <returns></returns>
    public T GetCurrentSceneManager<T>() where T : BaseSceneManager
    {
        return currentSceneManager as T;
    }
}
