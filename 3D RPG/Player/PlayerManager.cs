using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager instance;

    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }
    #endregion

    public Transform player;

    private void Start()
    {
        // 마우스 커서 잠금 처리
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 플레이어가 죽었을 때 씬 로드
    public void KillPlayer()
    {
        StartCoroutine(LoadRestartPoint());
    }

    IEnumerator LoadRestartPoint()
    {
        yield return new WaitForSeconds(2.0f);
        player.GetComponent<PlayerAnimator>().SetState(State.IDLE);
        // 현재 씬으로 재로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
