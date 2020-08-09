using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager
{
    #region Variables
    // 패널을 등록하여 관리할 변수
    static Dictionary<System.Type, BasePanel> panels = new Dictionary<System.Type, BasePanel>();
    #endregion Variables

    #region Other Methods
    /// <summary>
    /// 패널 등록 함수
    /// </summary>
    /// <param name="panelType">패널 타입</param>
    /// <param name="panel">등록할 패널</param>
    public static void RegistPanel(System.Type panelType, BasePanel panel)
    {
        // 등록되어 있지 않다면 추가
        if (!panels.ContainsKey(panelType))
        {
            panels.Add(panelType, panel);
        }
    }

    /// <summary>
    /// 패널 등록해제 함수
    /// </summary>
    /// <param name="panelType">패널 타입</param>
    public static void UnRegistPanel(System.Type panelType)
    {
        // 등록되어 있다면 제거
        if (panels.ContainsKey(panelType))
        {
            panels.Remove(panelType);
        }
    }

    /// <summary>
    /// 패널 반환 함수
    /// </summary>
    /// <typeparam name="T">패널 타입</typeparam>
    /// <returns>반환받을 패널</returns>
    public static T GetPanel<T>() where T : BasePanel
    {
        System.Type panelType = typeof(T);

        // 등록되어 있는 패널이 아니라면 리턴
        if (!panels.ContainsKey(panelType))
            return null;

        return panels[panelType] as T;
    }
    #endregion Other Methods
}
