using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    [SerializeField] InputField inputField;

    protected override void Initialize()
    {
        base.Initialize();

        // 인풋필드 활성화
        inputField.ActivateInputField();
    }

    /// <summary>
    /// 설정된 닉네임으로 포톤네트워크 연결
    /// </summary>
    public void OnConnect()
    {
        GameManager.Instance.NetworkManager.OnConnect(inputField.text);
    }
}
