using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : BaseSceneManager
{
    const float COLOR_VARIATION_TIME_RATE = 0.8f;   // 색상 변화 주기

    [SerializeField] Button connectButton;          // 접속 버튼
    [SerializeField] Text connectText;              // 접속 텍스트
    [SerializeField] Text inputText;                // 인풋 텍스트
    [SerializeField] Text serverStateText;          // 서버 상태 텍스트
    [SerializeField] Text isMuteText;           // 사운드 on/off 텍스트 표시

    float originAlpha;                              // 게임시작 텍스트 본래 알파값
    bool isConnected = false;                         // 접속버튼이 눌렸는지 여부

    public override void Initialize()
    {
        // 메인 BGM 재생
        GameManager.Instance.SoundManager.StopBGM();
        GameManager.Instance.SoundManager.PlayBGM(AudioNameConstant.MAIN_SOUND);

        base.Initialize();

        // 닉네임이 입력될때까지 인터랙터블 불가
        connectButton.interactable = false;
        // 기존 알파값 저장
        originAlpha = connectText.color.a;
    }

    public override void UpdateManager()
    {
        base.UpdateManager();

        TextColorPingPong(connectText);

        connectButton.interactable = InputTextIsNullCheck();
    }

    /// <summary>
    /// 텍스트 알파값 핑퐁
    /// </summary>
    /// <param name="text">변경할 텍스트 오브젝트</param>
    void TextColorPingPong(Text text)
    {
        Color newColor = text.color;
        // PingPong: 타임값에 따라 0부터 length값까지 계속 반복
        newColor.a = Mathf.PingPong(Time.time * 0.8f, originAlpha);
        text.color = newColor;
    }

    /// <summary>
    /// 인풋필드에 텍스트가 입력되었는지 검사
    /// </summary>
    /// <returns>입력여부</returns>
    bool InputTextIsNullCheck()
    {
        if (string.IsNullOrEmpty(inputText.text))
            return false;

        return true;
    }

    /// <summary>
    /// 서버 상태 메세지 셋팅
    /// </summary>
    /// <param name="msg">셋팅할 메세지</param>
    public void SetServerStateText(string msg)
    {
        serverStateText.text = msg;
    }

    /// <summary>
    /// 로비로 접속 시작
    /// </summary>
    public void OnConnect()
    {
        // 두번 클릭 방지
        if (!isConnected)
        {
            GameManager.Instance.NetworkManager.Connect(inputText.text);
            isConnected = true;
        }
    }

    /// <summary>
    /// 사운드 on/off
    /// </summary>
    public void OnIsMute()
    {
        SoundManager soundManager = GameManager.Instance.SoundManager;

        soundManager.IsMute = !soundManager.IsMute;

        // mute 설정이면 재생 중지
        if (!soundManager.IsMute)
            soundManager.StopBGM();
        else
            soundManager.PlayBGM(soundManager.CurrentBGM);

        isMuteText.text = "Sound\n" + (soundManager.IsMute ? "On" : "Off");
    }
}
