using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCacheableEffect : MonoBehaviour
{
    #region Property
    public string FilePath { get; set; }    // 파일 경로
    #endregion Property

    #region Unity Methods
    private void OnEnable()
    {
        StartCoroutine("CheckAlive");
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 이펙트가 실행중인지 검사
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            // 파티클 실행이 종료 되었다면 제거
            if (!GetComponent<ParticleSystem>().IsAlive(true))
            {
                GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().effectManager.Remove(FilePath, gameObject);
                break;
            }
        }
    }
    #endregion Other Methods
}
