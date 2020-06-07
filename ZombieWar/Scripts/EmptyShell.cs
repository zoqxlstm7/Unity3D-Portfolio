using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyShell : MonoBehaviour
{
    const float LIFE_TIME = 3.0f;               // 탄피 생존 시간

    [SerializeField] Rigidbody myRigidbody;     // 리지드바디 컴포넌트
    [SerializeField] float speed;               // 날아갈 속도

    string filePath;                            // 파일 경로
    public string FilePath
    {
        set => filePath = value;
    }

    float startGenerateTime;                    // 생성된 시간

    private void Update()
    {
        CheckedLifeTime();
    }

    /// <summary>
    /// 생존 시간 검사
    /// </summary>
    void CheckedLifeTime()
    {
        // 생존 시간이 지나면 파괴처리
        if(Time.time - startGenerateTime > LIFE_TIME)
        {
            DestroyEmptyShell();
        }
    }

    /// <summary>
    /// 탄피 배출 처리 함수
    /// </summary>
    /// <param name="point">배출 방향</param>
    public void AddForece(Transform point)
    {
        myRigidbody.AddForce(point.up * speed, ForceMode.Impulse);

        startGenerateTime = Time.time;
    }

    /// <summary>
    /// 탄피 파괴
    /// </summary>
    void DestroyEmptyShell()
    {
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EmptyShellManager.Remove(filePath, gameObject);
    }
}
