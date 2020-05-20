using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanel : BasePanel
{
    [SerializeField] Text countText;        // 현재 방 인원을 보여줄 텍스트
    [SerializeField] Text roomName;         // 현재 방 이름
    public Text RoomName
    {
        get => roomName;
    }

    [SerializeField] InputField inputField; // 인풋필드 객체

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
    /// 패널 내 업데이트 처리
    /// </summary>
    protected override void UpdatePanel()
    {
        UpdateCount();
        UpdateChat();
    }

    /// <summary>
    /// 룸내 인원수 업데이트
    /// </summary>
    void UpdateCount()
    {
        countText.text = networkManager.GetRoomCount();
    }

    /// <summary>
    /// 채팅 입력 대기
    /// </summary>
    void UpdateChat()
    {
        // 텍스트박스가 비어있지 않고
        if (!string.IsNullOrEmpty(inputField.text))
        {
            // 엔터키가 눌렸다면 채팅입력
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                GameManager.Instance.ChatSystem.UpdateChat(inputField.text);
                inputField.text = string.Empty;

                // 인풋필드 활성화
                inputField.ActivateInputField();
            }
        }
    }

    /// <summary>
    /// 방을 떠나는 버튼
    /// </summary>
    public void OnLeaveRoom()
    {
        networkManager.LeaveRoom();
    }
}
