using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolingManager : MonoBehaviour
{
    #region SingleTon
    public static PoolingManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public Canvas objectCanvas;

    //데미지 텍스트 오브젝트 풀링
    public DamageText damageText;
    public int damageTextAmount;
    List<DamageText> damageTextList = new List<DamageText>();

    //에너미 오브젝트 풀링
    public FSMUnit enemy;
    public int enemyAmount;
    List<FSMUnit> enemyList = new List<FSMUnit>();

    //아군 유닛 오브젝트 풀링
    public FSMUnit[] unit;
    public int unitAmount;
    List<FSMUnit> wariorList = new List<FSMUnit>();
    List<FSMUnit> archerList = new List<FSMUnit>();

    //화살 오브젝트 풀링
    public Projectile projectile;
    List<Projectile> projectileList = new List<Projectile>();

    //에너미 체력바 풀링
    public Image hpBar;
    List<Image> hpbarList = new List<Image>();

    private void Start()
    {
        //데미지 텍스트 오브젝트 생성
        for (int i = 0; i < damageTextAmount; i++)
        {
            DamageText newObject = Instantiate(damageText);
            newObject.gameObject.SetActive(false);
            newObject.transform.SetParent(objectCanvas.transform.Find("DamageTextParent"));

            damageTextList.Add(newObject);
        }

        int amount = enemyAmount + (unitAmount * unit.Length);
        //에너미 체력바 생성
        for (int i = 0; i < amount; i++)
        {
            Image newHpBar = Instantiate(hpBar);
            newHpBar.gameObject.SetActive(false);
            newHpBar.transform.SetParent(objectCanvas.transform.Find("HpBarParent"));

            hpbarList.Add(newHpBar);
        }

        //에너미 오브젝트 생성
        for (int i = 0; i < enemyAmount; i++)
        {
            FSMUnit newEnemy = Instantiate(enemy);
            newEnemy.GetComponent<CharacterUI>().CreateHpBar(hpbarList[i]);
            newEnemy.gameObject.SetActive(false);
            newEnemy.transform.SetParent(transform);

            enemyList.Add(newEnemy);
        }

        int hpbarIndex = enemyAmount;
        //아군 유닛 전사 오브젝트 생성
        for (int i = 0; i < unitAmount; i++)
        {
            FSMUnit newUnit = Instantiate(unit[0]);
            newUnit.GetComponent<CharacterUI>().CreateHpBar(hpbarList[hpbarIndex + i]);
            newUnit.gameObject.SetActive(false);
            newUnit.transform.SetParent(transform);

            wariorList.Add(newUnit);
        }

        //아군 유닛 궁수 오브젝트 생성
        hpbarIndex = enemyAmount + unitAmount;
        for (int i = 0; i < unitAmount; i++)
        {
            FSMUnit newUnit = Instantiate(unit[1]);
            newUnit.GetComponent<CharacterUI>().CreateHpBar(hpbarList[hpbarIndex + i]);
            newUnit.gameObject.SetActive(false);
            newUnit.transform.SetParent(transform);

            archerList.Add(newUnit);
        }

        for (int i = 0; i < (unitAmount*2); i++)
        {
            Projectile newProjectile = Instantiate(projectile);
            newProjectile.gameObject.SetActive(false);
            newProjectile.transform.SetParent(transform);

            projectileList.Add(newProjectile);
        }
    }

    //풀링된 데미지 텍스트를 반환
    public DamageText GetDamageText()
    {
        for (int i = 0; i < damageTextList.Count; i++)
        {
            if (!damageTextList[i].gameObject.activeInHierarchy)
            {
                return damageTextList[i];
            }
        }

        return null;
    }

    //풀링된 에너미 오브젝트를 반환
    public FSMUnit GetEnemy()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if(!enemyList[i].gameObject.activeInHierarchy)
            {
                return enemyList[i];
            }
        }

        return null;
    }

    //풀링된 전사 오브젝트 반환
    public FSMUnit GetHeavyKnight()
    {
        for (int i = 0; i < wariorList.Count; i++)
        {
            if (!wariorList[i].gameObject.activeInHierarchy)
                return wariorList[i];
        }

        return null;
    }

    //풀링된 궁수 오브젝트 반환
    public FSMUnit GetArcher()
    {
        for (int i = 0; i < archerList.Count; i++)
        {
            if (!archerList[i].gameObject.activeInHierarchy)
                return archerList[i];
        }

        return null;
    }

    //풀링된 발사체 오브젝트 반환
    public Projectile GetProjectile()
    {
        for (int i = 0; i < projectileList.Count; i++)
        {
            if (!projectileList[i].gameObject.activeInHierarchy)
                return projectileList[i];
        }

        return null;
    }

    //활성화된 에너미 오브젝트 모두 죽이기
    public void ActivatedEnemyAllDie()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].gameObject.activeInHierarchy)
                enemyList[i].GetComponent<UnitStats>().TakeDamage(enemyList[i].GetComponent<UnitStats>().GetCurrentHealth());
        }
    }

    //활성화 되어있는 모든 유닛 오브젝트 비활성화
    public void ActivatedUnitAllHide()
    {
        for (int i = 0; i < wariorList.Count; i++)
        {
            if (wariorList[i].gameObject.activeInHierarchy)
                wariorList[i].gameObject.SetActive(false);

            if (archerList[i].gameObject.activeInHierarchy)
                archerList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < hpbarList.Count; i++)
        {
            if (hpbarList[i].gameObject.activeInHierarchy)
                hpbarList[i].gameObject.SetActive(false);
        }
    }

    //풀링된 에너미 체력바 오브젝트 반환
    //public Image GetHpBar()
    //{
    //    for (int i = 0; i < enemyList.Count; i++)
    //    {
    //        if (!enemyList[i].gameObject.activeInHierarchy)
    //        {
    //            return hpbarList[i];
    //        }
    //    }

    //    return null;
    //}
}
