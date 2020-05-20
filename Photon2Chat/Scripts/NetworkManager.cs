using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Text statusText;   // 상태 텍스트
    [SerializeField] Text nickNameText; // 현재 닉네임 출력

    /// <summary>
    /// 상태 텍스트 업데이트
    /// </summary>
    private void Update()
    {
        // 상태가 변했을 때만 출력하도록
        if(statusText.text != PhotonNetwork.NetworkClientState.ToString())
        {
            statusText.text = PhotonNetwork.NetworkClientState.ToString();
            Debug.Log("Status: " + statusText.text);
        }
    }

    private void Awake()
    {
        // 씬레벨 동기화
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region 연결 콜백
    /// <summary>
    /// 포톤 네트워크 연결
    /// </summary>
    /// <param name="nickName">지정한 닉네임</param>
    public void OnConnect(string nickName)
    {
        PhotonNetwork.ConnectUsingSettings();

        PhotonNetwork.LocalPlayer.NickName = nickName;
    }

    /// <summary>
    /// 마스터로 접속됐을 때의 콜백 함수
    /// </summary>
    public override void OnConnectedToMaster()
    {
        // 로비로 입장
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// 연결이 끊겼을 때의 콜백 함수
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        nickNameText.text = string.Empty;

        // 방목록 초기화
        (PanelManager.GetPanel(typeof(LobbyPanel)) as LobbyPanel).ResetRoomList();

        // 로비패널을 숨기고 로그인 패널 노출
        PanelManager.GetPanel(typeof(LobbyPanel)).Close();
        PanelManager.GetPanel(typeof(LoginPanel)).Show();
    }
    #endregion

    #region 로비 룸 콜백 
    /// <summary>
    /// 로비로 입장했을 때의 콜백 함수
    /// </summary>
    public override void OnJoinedLobby()
    {
        // 로그인 패널을 숨기고 로비 패널 노출
        PanelManager.GetPanel(typeof(LoginPanel)).Close();
        PanelManager.GetPanel(typeof(LobbyPanel)).Show();

        nickNameText.text = "현재 접속자\n<color=yellow>" + PhotonNetwork.LocalPlayer.NickName + "</color>";
        Debug.Log("Joined Lobby: " + PhotonNetwork.LocalPlayer.NickName);
    }

    /// <summary>
    /// 룸 리스트 업데이트
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Debug.Log("list: " + roomList.Count);
        LobbyPanel lobbyPanel = PanelManager.GetPanel(typeof(LobbyPanel)) as LobbyPanel;

        // 생성된 방목록 오브젝트들을 얻어옴
        List<RoomData> roomDatas = new List<RoomData>();
        foreach (var item in lobbyPanel.Content.GetComponentsInChildren<RoomData>())
        {
            roomDatas.Add(item);
        }

        // 새 방목록 갱신
        foreach (var item in roomList)
        {
            // 방목록에서 방이 사라지는 방인지 검사
            if (!item.RemovedFromList)
            {
                // 업데이트된 방 목록중 현재 생성되어있는 같은 이름을 가진 방이 있는지검사
                RoomData findData = roomDatas.Find(delegate (RoomData a) { return a.RoomName == item.Name; });
                // 같은이름의 방이 목록에 없다면 새로 추가
                if(findData == null)
                {
                    GameObject room = Instantiate(lobbyPanel.Room, lobbyPanel.Content);

                    RoomData roomData = room.GetComponent<RoomData>();
                    roomData.RoomName = item.Name;
                    roomData.CountInfo = item.PlayerCount + " / " + item.MaxPlayers;

                    roomData.UpdateUI();
                }
                else // 같은 이름의 방이 있다면 카운트 업데이트
                {
                    findData.CountInfo = item.PlayerCount + " / " + item.MaxPlayers;
                    findData.UpdateUI();
                }
            }
            else // 사라지는 방이라면
            {
                // 생성된 방 오브젝트 중 이름이 같은 오브젝트 파괴
                foreach (var roomData in roomDatas)
                {
                    if (roomData.RoomName.Equals(item.Name))
                    {
                        Destroy(roomData.gameObject);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 방을 생성하거나 입장할 때의 콜백 함수
    /// </summary>
    public override void OnJoinedRoom()
    {
        // 로비패널을 숨기고 대화패널 노출
        PanelManager.GetPanel(typeof(LobbyPanel)).Close();
        PanelManager.GetPanel(typeof(ChatPanel)).Show();

        // 채팅 기록 초기화
        GameManager.Instance.ChatSystem.ResetChat();
        // 방이름 초기화
        ChatPanel chatPanel = PanelManager.GetPanel(typeof(ChatPanel)) as ChatPanel;
        chatPanel.RoomName.text = PhotonNetwork.CurrentRoom.Name;
    }

    /// <summary>
    /// 방을 떠날때의 콜백 함수
    /// </summary>
    public override void OnLeftRoom()
    {
        // 로비패널을 노출 대화패널 숨김
        PanelManager.GetPanel(typeof(ChatPanel)).Close();
        PanelManager.GetPanel(typeof(LobbyPanel)).Show();

        // 생성된 방정보 오브젝트 리셋
        (PanelManager.GetPanel(typeof(LobbyPanel)) as LobbyPanel).ResetRoomList();
    }

    /// <summary>
    /// 플레이어가 방에 입장했을 때의 콜백함수
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string msg = string.Format("<color=yellow>{0}</color>님이 입장하셨습니다.",
            newPlayer.NickName);

        GameManager.Instance.ChatSystem.UpdateChat(msg, true);
    }

    /// <summary>
    /// 플레이어가 방에서 퇴장할때의 콜백함수
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string msg = string.Format("<color=yellow>{0}</color>님이 퇴장하셨습니다.",
            otherPlayer.NickName);        

        GameManager.Instance.ChatSystem.UpdateChat(msg, true);
    }

    /// <summary>
    /// 방 생성이 실패했을 때의 콜백
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room failed. message: " + message);

        // 방 재생성
        CreateRoom();
    }

    /// <summary>
    /// 랜덤방 입장에 실패했을 때의 콜백
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join random room failed. message: " + message);
    }

    /// <summary>
    /// 방 참가에 실패했을 때의 콜백
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join room failed. message: " + message);
    }

    #endregion

    #region Button Event
    /// <summary>
    /// 방 생성
    /// </summary>
    public void CreateRoom()
    {
        string roomName = "ROOM " + Random.Range(1, 999);
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 });
    }

    /// <summary>
    /// 선택된 방에 입장
    /// </summary>
    /// <param name="roomName">입장할 방이름</param>
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// 랜덤방 입장
    /// </summary>
    public void JoinedRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// 방을 떠남
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// 포톤 네트워크 접속 종료
    /// </summary>
    public void OnDisconnect()
    {
        PhotonNetwork.Disconnect();
    }
    #endregion

    /// <summary>
    /// 현재 접속인원과 로비 인원 반환
    /// </summary>
    /// <returns></returns>
    public string GetCount()
    {
        string countStr = string.Format("접속인원: {0} / 로비인원: {1}",
            PhotonNetwork.CountOfPlayers,
            PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms);

        return countStr;
    }

    /// <summary>
    /// 현재 룸인원과 최대인원 반환
    /// </summary>
    /// <returns></returns>
    public string GetRoomCount()
    {
        string countStr = string.Format("접속인원: {0} / {1}",
            PhotonNetwork.CurrentRoom.PlayerCount,
            PhotonNetwork.CurrentRoom.MaxPlayers);

        return countStr;
    }
}
