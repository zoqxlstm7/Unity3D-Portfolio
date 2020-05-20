using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singletone
    private static GameManager instance;
    public static GameManager Instance
    {
        get => instance;
    }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion

    // 네트워크 매니저 객체 반환
    [SerializeField] NetworkManager networkManager;
    public NetworkManager NetworkManager
    {
        get => networkManager;
    }

    // 채팅 RPC 업데이트 객체 반환
    [SerializeField] ChatSystem chatSystem;
    public ChatSystem ChatSystem
    {
        get => chatSystem;
    }

    private void Start()
    {
        PanelManager.GetPanel(typeof(LoginPanel)).Show();
    }
}
