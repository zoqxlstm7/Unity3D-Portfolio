using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    Animator animator;

    public Image startPanel;
    public Text mainText;
    public Text countText;
    public Text phaseText;

    int count = 5;

    private void Start()
    {
        animator = countText.GetComponent<Animator>();
        startPanel.gameObject.SetActive(false);
    }

    public void ActivatedStartPanel(bool value)
    {
        startPanel.gameObject.SetActive(value);
    }

    public void SetMainText(string text)
    {
        mainText.text = text;
    }

    public void SetPhaseText(string text)
    {
        phaseText.text = text;
    }
    
    //카운트 텍스트와 카운트애미메이션 효과를 보여줌
    public bool CountAnimation()
    {
        if (count == 0)
        {
            countText.text = "";
            return false;
        }

        countText.text = count.ToString();
        animator.SetTrigger("BouncingText");

        count -= 1;

        return true;
    }
}
