using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class ChatSystem : MonoBehaviourPunCallbacks
{
    const int CHAT_MAX_INDEX = 12;                              // 최대 채팅 텍스트 갯수

    [SerializeField] Text[] chatArr = new Text[CHAT_MAX_INDEX]; // 채팅 텍스트를 담을 배열
    int chatIndex = 0;                                          // 채팅이 쓰여지는 배열 인덱스

    /// <summary>
    /// 채팅방 입장시 채팅관련 변수 리셋
    /// </summary>
    public void ResetChat()
    {
        for (int i = 0; i < chatArr.Length; i++)
        {
            chatArr[i].text = string.Empty;
        }
        chatIndex = 0;
    }

    /// <summary>
    /// 채팅메세지 RPC 전송
    /// </summary>
    /// <param name="msg">보낼 메세지</param>
    /// <param name="isEntry">입장시 출력문구인지 검사</param>
    public void UpdateChat(string msg, bool isEntry = false)
    {
        string str = string.Empty;
        
        if(!isEntry)
        {
            str = string.Format("<color=yellow>[{0}]</color>: {1}",
            PhotonNetwork.LocalPlayer.NickName, msg);
        }
        else
        {
            str = string.Format("{0}", msg);
        }
        
        photonView.RPC("RpcChat", RpcTarget.All, str);
    }

    /// <summary>
    /// 채팅 메세지 업데이트
    /// </summary>
    /// <param name="msg">보낼 메세지</param>
    [PunRPC]
    void RpcChat(string msg)
    {
        // 채팅 최대 갯수에 도달하지 않으면 순차적으로 메세지 입력
        if(chatIndex < CHAT_MAX_INDEX)
        {
            chatArr[chatIndex].text = msg;
            chatIndex++;
        }
        else
        {
            // 메세지를 앞으로 하나씩 당긴후 마지막 인덱스에 채팅메시지 추가
            for (int i = 1; i < chatArr.Length; i++)
            {
                chatArr[i - 1].text = chatArr[i].text; 
            }

            chatArr[chatArr.Length - 1].text = msg;
        }
    }
}
