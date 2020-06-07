using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChatManager : MonoBehaviourPun
{
    [SerializeField] Text[] msgTexts;               // 메세지를 출력할 텍스트 객체들
    [SerializeField] InputField inputField;         // 인풋필드 
    [SerializeField] ScrollRect scrollRect;         // 스크롤뷰

    public bool IsInputChat { get; set; } = false;  // 입력중인지 검사

    int textIndex = 0;                              // 메세지를 출력할 텍스트 인덱스

    private void Update()
    {
        // 입력중인지 검사
        if (inputField.isFocused)
            IsInputChat = true;
        else
            IsInputChat = false;

        // 스크롤바의 위치를 제일 아래로 내림
        scrollRect.verticalNormalizedPosition = 0.0f;
    }

    /// <summary>
    /// 채팅을 위한 키가 입력된 경우 처리하는 함수
    /// </summary>
    public void ChatInputKey()
    {
        // 인풋필드에 텍스트가 비워져있는 경우는 인풋필드 활성화
        if (string.IsNullOrEmpty(inputField.text))
        {
            inputField.ActivateInputField();
        }
        else
        {
            // 인풋필드가 채워져 있는 경우는 메세지를 보냄
            string nickName = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero.NickName;

            SendMsg(inputField.text, nickName);
            inputField.text = string.Empty;
        }
    }

    /// <summary>
    /// 메세지를 보내는 함수
    /// </summary>
    /// <param name="msg">보낼메세지</param>
    /// <param name="nickName">보내는 사람 닉네임</param>
    public void SendMsg(string msg, string nickName)
    {
        string formatMsg = string.Format("<color=yellow>[{0}]</color>: {1}", nickName, msg);

        // 메세지 전송 RPC 실행
        photonView.RPC("SendMsgRPC", RpcTarget.AllBuffered, formatMsg);
    }

    /// <summary>
    /// 메세지 전송 RPC
    /// </summary>
    /// <param name="msg">보낼 내용</param>
    [PunRPC]
    void SendMsgRPC(string msg)
    {
        // 채팅 최대 갯수에 도달하지 않으면 순차적으로 메세지 입력
        if (textIndex < msgTexts.Length)
        {
            msgTexts[textIndex].text = msg;
            textIndex++;
        }
        else
        {
            //메세지를 앞으로 하나씩 당긴후 마지막 인덱스에 채팅메시지 추가
            for (int i = 1; i < msgTexts.Length; i++)
            {
                msgTexts[i - 1].text = msgTexts[i].text;
            }

            msgTexts[msgTexts.Length - 1].text = msg;
        }
    }
}
