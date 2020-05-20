using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스크롤 정보를 담을 클래스
/// </summary>
[System.Serializable]
public class ScrollData
{
    public Image bgImage;
    public float speed;
    public float offset; 
}

public class BGScroller : MonoBehaviour
{
    [SerializeField] ScrollData[] scrollDatas;  // 스크롤에 사용할 데이터

    private void Update()
    {
        UpdateScroll();
    }

    /// <summary>
    /// 텍스처 오프셋 업데이트
    /// </summary>
    void UpdateScroll()
    {
        for (int i = 0; i < scrollDatas.Length; i++)
        {
            SetTextureOffset(scrollDatas[i]);
        }
    }

    /// <summary>
    /// 텍스처 오프셋 셋팅
    /// </summary>
    /// <param name="scrollData">스크롤에 사용할 데이터</param>
    void SetTextureOffset(ScrollData scrollData)
    {
        // 오프셋 이동 및 초기화
        scrollData.offset += scrollData.speed * Time.deltaTime;
        if (scrollData.offset > 1)
            scrollData.offset = scrollData.offset % 1f;

        // 이동된 오프셋 적용
        Vector2 offset = new Vector2(scrollData.offset, 0);
        scrollData.bgImage.material.SetTextureOffset("_MainTex", offset);
    }
}
