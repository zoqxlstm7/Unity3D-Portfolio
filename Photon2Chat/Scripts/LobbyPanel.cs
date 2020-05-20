using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : BasePanel
{
    [SerializeField] Transform content; // 룸 오브젝트가 정렬될 부모객체
    public Transform Content
    {
        get => content;
    }
    [SerializeField] GameObject room;   // 룸 객체
    public GameObject Room
    {
        get => room;
    }
    [SerializeField] Text countText;    // 인원수 표시 텍스트

    NetworkManager networkManager;

    protected override void Initialize()
    {
        base.Initialize();
    }

    private void Start()
    {
        networkManager = GameManager.Instance.NetworkManager;
    }

    /// <summary>
    /// 생성되어있는 방목록삭제
    /// </summary>
    public void ResetRoomList()
    {
        foreach (var item in content.GetComponentsInChildren<RoomData>())
        {
            Destroy(item.gameObject);
        }
    }

    /// <summary>
    /// 패널 내 업데이트 처리
    /// </summary>
    protected override void UpdatePanel()
    {
        countText.text = networkManager.GetCount();
    }

    /// <summary>
    /// 방 생성 버튼
    /// </summary>
    public void OnCreateRoom()
    {
        networkManager.CreateRoom();
    }

    /// <summary>
    /// 랜덤방 입장 버튼
    /// </summary>
    public void OnJoinedRandomRoom()
    {
        networkManager.JoinedRandomRoom();
    }

    /// <summary>
    /// 접속종료 버튼
    /// </summary>
    public void OnDisconnect()
    {
        networkManager.OnDisconnect();
    }
}
