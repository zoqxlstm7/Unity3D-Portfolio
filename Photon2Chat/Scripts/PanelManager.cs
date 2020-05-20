using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    // 패널을 관리할 변수
    [SerializeField] static Dictionary<System.Type, BasePanel> panels = new Dictionary<System.Type, BasePanel>();

    /// <summary>
    /// 패널 등록
    /// </summary>
    /// <param name="panelType">패널 클래스 타입</param>
    /// <param name="newPanel">등록할 패널</param>
    public static void RegistPanel(System.Type panelType, BasePanel newPanel)
    {
        if (panels.ContainsKey(panelType))
        {
            Debug.Log("Already Regist. panelType: " + panelType);
        }
        else
        {
            panels.Add(panelType, newPanel);
        }
    }

    /// <summary>
    /// 등록된 패널 삭제
    /// </summary>
    /// <param name="panelType">삭제할 패널 클래스 타입</param>
    public static void UnRegistPanel(System.Type panelType)
    {
        if (panels.ContainsKey(panelType))
        {
            panels.Remove(panelType);
        }
        else
        {
            Debug.Log("Not Regist Panel. panelType: " + panelType);
        }
    }

    /// <summary>
    /// 등록된 패널 반환
    /// </summary>
    /// <param name="panelType">반환받을 패널 클래스 타입</param>
    /// <returns></returns>
    public static BasePanel GetPanel(System.Type panelType)
    {
        if (!panels.ContainsKey(panelType))
        {
            Debug.Log("Not exist panel. panelType: " + panelType);
            return null; 
        }

        return panels[panelType];
    }
}
