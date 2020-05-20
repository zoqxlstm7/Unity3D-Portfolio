using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
    [SerializeField] Text infoText;     // 방정보가 출력될 텍스트

    [SerializeField] string roomName;   // 방 이름
    public string RoomName
    {
        get => roomName;
        set => roomName = value;
    }
    [SerializeField] string countInfo;  // 방인원수
    public string CountInfo
    {
        get => countInfo;
        set => countInfo = value;
    }

    /// <summary>
    /// 방정보 UI 업데이트
    /// </summary>
    public void UpdateUI()
    {
        infoText.text = roomName + "\n" + countInfo;
    }

    /// <summary>
    /// 선택한 방 참가 버튼
    /// </summary>
    public void OnJoinRoom()
    {
        GameManager.Instance.NetworkManager.JoinRoom(roomName);
    }
}
