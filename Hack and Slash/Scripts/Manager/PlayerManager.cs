using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;   

public class PlayerManager : MonoBehaviour
{
    #region SingleTon
    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public static bool isMove = true;
    public static bool isBattle = false;

    public Transform player;

    //플레이어 사망
    public void KillPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);        
    }

    public void LoadScene(string sceneName)
    {
        player.GetComponent<FSMPlayer>().SetAgentStop(true);
        player.position = Vector3.zero;
        player.GetComponent<FSMPlayer>().SetAgentStop(false);

        LoadingSceneManager.LoadScene(sceneName);
    }
}
