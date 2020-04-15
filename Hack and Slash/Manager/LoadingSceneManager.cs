using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;         //넘어갈 씬
    public Image progressBar;               //로딩 진행 상황을 보여줄 이미지

    private void Start()
    {
        StartCoroutine("LoadScene");
    }

    public static void LoadScene(string SceneName)
    {
        nextScene = SceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;

        while(!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;              

            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                    timer = 0f;
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if(progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    break;
                }
            }
        }
    }
}
