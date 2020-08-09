using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    #region Singtone
    static JoyStick instance;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion Singtone

    #region Variables
    [SerializeField] RectTransform bgImg;           // 스틱 백그라운드
    [SerializeField] RectTransform joystickImg;     // 조이스틱

    Vector2 inputVector;                            // 입력벡터
    float bgRadius = 0.0f;                          // 백그라운드 반지름
    float stickRadius = 0.0f;                       // 스틱 반지름
    #endregion Variables

    private void Start()
    {
        // 반지름 계산
        bgRadius = bgImg.rect.width / 2;
        stickRadius = joystickImg.rect.width / 2;
    }

    #region EventSystems Interface
    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 지점과 백그라운 지점 사이에 거리벡터 계산
        inputVector = (eventData.position - (Vector2)bgImg.position);
        // 조이스틱을 백그라운드 범위 내로 조정
        inputVector = Vector2.ClampMagnitude(inputVector, bgRadius - stickRadius);

        // 조이스틱을 입력벡터만큼 이동
        joystickImg.localPosition = inputVector;

        // 방향벡터 계산
        inputVector = inputVector.normalized;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 포인터가 눌렸을 때 드래그 함수 호출
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 포인터가 떼어졌을 때 입력벡터와 조이스틱 위치 초기화
        inputVector = Vector2.zero;
        joystickImg.localPosition = Vector3.zero;
    }
    #endregion EventSystems Interface

    #region Other Methods
    /// <summary>
    /// 수평값을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public static float GetHorizontalValue()
    {
        return instance.inputVector.x;
    }

    /// <summary>
    /// 수직값을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public static float GetVerticalValue()
    {
        return instance.inputVector.y;
    }
    #endregion Other Methods
}
