using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPanel : BasePanel
{
    #region Variables
    [SerializeField] Transform mapCamera;   // 맵카메라
    #endregion Variables

    #region BasePanel Methods
    public override void InitializePanel()
    {
        base.InitializePanel();

        mapCamera.gameObject.SetActive(false);
        Close();
    }
    #endregion BasePanel Methods

    #region Other Methods
    /// <summary>
    /// 맵 카메라의 초기 위치를 지정하는 함수
    /// </summary>
    void SetPositionMapCamera()
    {
        Transform playerPos = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().player.transform;
        // 맵사이즈를 벗어나지 않도록 계산
        float posX = Mathf.Clamp(playerPos.position.x, -MapCameraMoveHandler.MAX_X_POISTION, MapCameraMoveHandler.MAX_X_POISTION);
        float posY = Mathf.Clamp(playerPos.position.y, -MapCameraMoveHandler.MAX_Y_POSITION, MapCameraMoveHandler.MAX_Y_POSITION);

        mapCamera.transform.position = new Vector3(posX, posY, -10f);
    }
    #endregion Other Methods

    #region Button Methods
    /// <summary>
    /// 맵 오픈 버튼 함수
    /// </summary>
    public void OnOpenMapBtn()
    {
        Show();
        mapCamera.gameObject.SetActive(true);

        SetPositionMapCamera();
    }

    /// <summary>
    /// 맵 닫기 버튼 함수
    /// </summary>
    public void OnCloseMap()
    {
        mapCamera.gameObject.SetActive(false);
        Close();
    }
    #endregion Button Methods
}
