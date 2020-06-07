using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public const int MAX_PLAYER = 4;       // 최대 플레이어 지정

    // 캐시 데이터를 보관할 변수
    Dictionary<string, Queue<GameObject>> caches = new Dictionary<string, Queue<GameObject>>();
    public Dictionary<string, Queue<GameObject>> Caches
    {
        get => caches;
    }

    // 방에 있는 플레이어 수 반환
    public int PlayerCountInRoom => PhotonNetwork.CurrentRoom.PlayerCount;
    // 설정된 닉네임 반환
    public string NickName => PhotonNetwork.NickName;
    // 마스터 클라이언트인지 반환
    public bool IsMasterClient => PhotonNetwork.IsMasterClient;

    // 스타팅 지점
    Vector3[] startingPoints =
    {
        new Vector3(2,0,0),
        new Vector3(0,0,2),
        new Vector3(-2,0,0),
        new Vector3(0,0,-2)
    };

    int startingIndex = 0;      // 스타팅 지점 인덱스

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// 서버 연결
    /// </summary>
    /// <param name="nickName">적용될 닉네임</param>
    public void Connect(string nickName)
    {
        PhotonNetwork.NickName = nickName;
        string msg = "서버 접속을 시작합니다...";
        GameManager.Instance.GetCurrentSceneManager<TitleSceneManager>().SetServerStateText(msg);

        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// 서버에 연결됐을 때의 콜백함수
    /// </summary>
    public override void OnConnectedToMaster()
    {
        string msg = "마스터 서버에 연결되었습니다...";
        GameManager.Instance.GetCurrentSceneManager<TitleSceneManager>().SetServerStateText(msg);

        // 로비로 접속 시도
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// 로비로 입장했을 때의 콜백함수
    /// </summary>
    public override void OnJoinedLobby()
    {
        string msg = "로비에 연결되었습니다...";
        GameManager.Instance.GetCurrentSceneManager<TitleSceneManager>().SetServerStateText(msg);

        // 로비씬 로드
        GameManager.Instance.SceneController.LoadScene(SceneNameConstant.LOBBY_SCENE);
    }

    /// <summary>
    /// 랜덤방 입장
    /// </summary>
    public void JoinRandomRoom(int selectModelIndex, int selectGunIndex)
    {
        // 선택한 캐릭터 및 총 모델 인덱스 저장
        GameManager.Instance.ModelIndex = selectModelIndex;
        GameManager.Instance.GunModelIndex = selectGunIndex;
        
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// 방을 나감
    /// </summary>
    public void LeftRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// 랜덤방 입장에 실패했을 때의 콜백함수
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed. message: " + message);
        // 방생성
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MAX_PLAYER });
    }

    /// <summary>
    /// 방에 입장했을 때의 콜백함수
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        // 룸에 들어온 순서에 따른 스타팅 지점 인덱스 설정
        startingIndex = PlayerCountInRoom - 1;

        // 대기씬으로 이동
        GameManager.Instance.SceneController.LoadScene(SceneNameConstant.LOADING_SCENE);
    }

    /// <summary>
    /// 방을 떠날때의 콜백 함수
    /// </summary>
    public override void OnLeftRoom()
    {
        // 메인 BGM 재생
        GameManager.Instance.SoundManager.StopBGM();
        GameManager.Instance.SoundManager.PlayBGM(AudioNameConstant.MAIN_SOUND);

        // 로비씬으로 이동
        GameManager.Instance.SceneController.LoadScene(SceneNameConstant.LOBBY_SCENE);
    }

    /// <summary>
    /// 마스터 클라이언트에서 게임 실행
    /// </summary>
    public void OnGotoInGameScene()
    {
        // 마스터 클라이언트만 실행 가능
        if (PhotonNetwork.IsMasterClient)
        {
            // 게임시작 후 룸에 들어올 수 없도록함
            PhotonNetwork.CurrentRoom.IsOpen = false;
            // 인게임 씬으로 이동
            PhotonNetwork.LoadLevel(SceneNameConstant.INGAME_SCENE);
            StartCoroutine(CompleteLoadLevel());
        }
    }

    /// <summary>
    /// 씬 로드가 완료되었는지 체크
    /// </summary>
    /// <returns></returns>
    IEnumerator CompleteLoadLevel()
    {
        float progress = 0f;

        while (progress < 1)
        {
            yield return null;

            progress = PhotonNetwork.LevelLoadingProgress;
            Debug.Log("progress" + progress);
        }

        Debug.Log("Load Level");
        // 플레이어 객체 생성
        // 마스터 클라이언트에서 씬을 일괄로드하므로 RPC로 실행
        photonView.RPC("GeneratePlayer", RpcTarget.AllBuffered);
        //PhotonNetwork.Instantiate("Prefabs/Player", Vector3.zero, Quaternion.identity);
    }
    
    /// <summary>
    /// 플레이어 생성
    /// </summary>
    [PunRPC]
    void GeneratePlayer()
    {
        PhotonNetwork.Instantiate("Prefabs/Player", startingPoints[startingIndex], Quaternion.identity);

        Debug.Log("startingIndex: " + startingIndex);
    }
}
