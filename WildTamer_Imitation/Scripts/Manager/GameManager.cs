using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SingleTone
    static GameManager instance;
    public static GameManager Instance => instance;

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
    #endregion SingleTone

    #region Variables
    BaseSceneManager currentSceneManager;   // 현재 씬의 씬매니저
    #endregion Variables

    #region Property
    #endregion Property

    #region Unity Methods
    private void Start()
    {
        SetInitialCurrentSceneManager();
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 초기 씬 매니저 설정 함수
    /// </summary>
    void SetInitialCurrentSceneManager()
    {
        currentSceneManager = FindObjectOfType<BaseSceneManager>();
    }

    /// <summary>
    /// 현재 씬의 씬매니저 반환 함수
    /// </summary>
    /// <typeparam name="T">씬매니저 타입</typeparam>
    /// <returns>현재 씬 매니저</returns>
    public T GetCurrentSceneManager<T>() where T : BaseSceneManager
    {
        return currentSceneManager as T;
    }
    #endregion Other Methods
}
