using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    int maxValue = 100;                     // 최대 확률

    [Range(0,100)]
    [SerializeField] int dropValue = 60;    // 드랍될 확률

    [Range(0, 100)]
    [SerializeField] int itemDropRate1;     // 아이템1 드랍 확률
    [Range(0, 100)]
    [SerializeField] int itemDropRate2;     // 아이템2 드랍 확률
    [Range(0, 100)]
    [SerializeField] int itemDropRate3;     // 아이템3 드랍 확률

    /// <summary>
    /// 아이템이 드랍되는지 검사하는 함수
    /// </summary>
    /// <param name="gainer">아이템 획득자</param>
    public void DropTale(Actor gainer)
    {
        int pick = Random.Range(0, maxValue + 1);

        // 아이템 드랍된다면 드랍 함수 실행
        if(pick <= dropValue)
        {
            DropItem(gainer);
        }
    }

    /// <summary>
    /// 아이템 드랍 함수
    /// </summary>
    /// <param name="gainer">아이템 획득자</param>
    void DropItem(Actor gainer)
    {
        Player player = gainer.GetComponent<Player>();

        if(player != null)
        {
            // 아이템 드랍 함수 실행
            player.DropItem(maxValue, itemDropRate1, itemDropRate2, itemDropRate3);
        }
    }
}
