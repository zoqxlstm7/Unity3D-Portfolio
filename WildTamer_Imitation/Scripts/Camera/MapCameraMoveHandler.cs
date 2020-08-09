using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCameraMoveHandler : MonoBehaviour, IPointerDownHandler , IDragHandler
{
    #region Variables
    public const float MAX_X_POISTION = 5f;     // 이동가능한 최대 x지점
    public const float MAX_Y_POSITION = 20f;    // 이동가능한 최대 y지점

    [SerializeField] Transform mapCamera;       // 맵카메라 위치
    [SerializeField] float speed;               // 드래그 이동속도

    Vector2 priviousPos = Vector3.zero;        // 이전좌표
    #endregion Variables

    #region EventSystems Interface
    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 지점과 백그라운 지점 사이에 거리벡터 계산
        Vector3 inputVector = (eventData.position - priviousPos);
        inputVector.z = 0.0f;

        // 이동벡터 계산
        inputVector = inputVector.normalized;
        inputVector = inputVector * speed * Time.deltaTime;

        // 방향벡터와 반대방향으로 맵카메라 이동
        mapCamera.position -= inputVector;
        // 범위를 벗어나지 않도록 계산
        float posX = Mathf.Clamp(mapCamera.position.x, -MAX_X_POISTION, MAX_X_POISTION);
        float posY = Mathf.Clamp(mapCamera.position.y, -MAX_Y_POSITION, MAX_Y_POSITION);
        mapCamera.position = new Vector3(posX, posY, -10f);

        // 이전지점 갱신
        priviousPos = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 이전지점 갱신 및 드래그 함수 호출
        priviousPos = eventData.position;
        OnDrag(eventData);
    }
    #endregion EventSystems Interface
}
