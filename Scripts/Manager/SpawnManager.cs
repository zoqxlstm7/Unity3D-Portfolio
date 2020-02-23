using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    #region
    public static SpawnManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public BattleUI battleUI;       //시작 카운트를 제어할 UI클래스

    public Image hpBar;             //타워의 체력바
    public Spawner[] spawners;      //타워마다의 스폰을 제어하는 클래스
    SpawnState state;               //현재 스폰의 상태를 담을 열거형 변수
    bool isNewState = false;        //상태변화의 유무

    float uiActivateTime;           //ui출현 시간

    private void Start()
    {
        state = SpawnState.Ready;
        StartCoroutine("FSMMain");
    }

    void SetState(SpawnState newState)
    {
        isNewState = true;
        state = newState;
    }

    IEnumerator FSMMain()
    {
        while (true)
        {
            isNewState = false;
            yield return StartCoroutine(state.ToString());
        }
    }

    //첫 진입 후 전투 준비 상태
    IEnumerator Ready()
    {
        //플레이어 행동 제어
        PlayerManager.isMove = false;

        battleUI.SetMainText("첫 번째 저지선이 개방됩니다");
        battleUI.ActivatedStartPanel(true);

        uiActivateTime = Time.time;

        do
        {
            yield return null;
            if (isNewState)
                break;

            //UI카운트 표시
            if (Time.time - uiActivateTime >= 1f)
            {
                if (!battleUI.CountAnimation())
                    break;

                uiActivateTime = Time.time;
            }

        } while (!isNewState);

        battleUI.ActivatedStartPanel(false);
        PlayerManager.isMove = true;
        SetState(SpawnState.PhaseOne);

        PlayerManager.isBattle = true; //배틀씬에서 유닛 생산 허용
    }

    //1페이즈 시작
    IEnumerator PhaseOne()
    {
        CharacterUI characterUI = spawners[0].GetComponent<CharacterUI>();

        //타워1의 스폰 시작
        spawners[0].isStartSpawn = true;
        UnitStats enemyStats = spawners[0].GetComponent<UnitStats>();

        characterUI.CreateHpBar(hpBar);
        characterUI.UpdateHpBar(enemyStats.maxHealth, enemyStats.GetCurrentHealth());
        characterUI.isFollowTarget = false;

        do
        {
            yield return null;
            if (isNewState)
                break;

            if (enemyStats.GetCurrentHealth() <= 0)
            {
                spawners[0].isStartSpawn = false;
                spawners[0].OnDeath();

                break;
            }

        } while (!isNewState);

        battleUI.SetPhaseText("두 번째 저지선이 개방됩니다");
        SetState(SpawnState.PhaseTwo);
    }

    //2페이즈 시작
    IEnumerator PhaseTwo()
    {
        CharacterUI characterUI = spawners[1].GetComponent<CharacterUI>();

        //타워2의 스폰 시작
        spawners[1].isStartSpawn = true;
        UnitStats enemyStats = spawners[1].GetComponent<UnitStats>();

        uiActivateTime = Time.time;

        do
        {
            yield return null;
            if (isNewState)
                break;

            if (Time.time - uiActivateTime > 3f)
            {
                battleUI.SetPhaseText("");

                characterUI.CreateHpBar(hpBar);
                characterUI.UpdateHpBar(enemyStats.maxHealth, enemyStats.GetCurrentHealth());
                characterUI.isFollowTarget = false;
            }

            if (enemyStats.GetCurrentHealth() <= 0)
            {
                spawners[1].isStartSpawn = false;
                spawners[1].OnDeath();

                break;
            }

        } while (!isNewState);

        battleUI.SetPhaseText("세 번째 저지선이 개방됩니다");
        SetState(SpawnState.PhaseThree);
    }

    //3페이즈 시작
    IEnumerator PhaseThree()
    {
        CharacterUI characterUI = spawners[2].GetComponent<CharacterUI>();

        //타워3 스폰 시작
        spawners[2].isStartSpawn = true;
        UnitStats enemyStats = spawners[2].GetComponent<UnitStats>();

        uiActivateTime = Time.time;

        do
        {
            yield return null;

            if (Time.time - uiActivateTime > 3f)
            {
                battleUI.SetPhaseText("");

                characterUI.CreateHpBar(hpBar);
                characterUI.UpdateHpBar(enemyStats.maxHealth, enemyStats.GetCurrentHealth());
                characterUI.isFollowTarget = false;
            }

            if (enemyStats.GetCurrentHealth() <= 0)
            {
                spawners[2].isStartSpawn = false;
                spawners[2].OnDeath();

                break;
            }

        } while (!isNewState);

        battleUI.SetMainText("저지선 붕괴");
        battleUI.ActivatedStartPanel(true);
        SetState(SpawnState.End);
    }

    //모든 페이즈 종료
    IEnumerator End()
    {
        PlayerManager.isBattle = false;

        uiActivateTime = Time.time;
        do
        {
            yield return null;
            if (isNewState)
                break;

            if (Time.time - uiActivateTime > 3f)
            {
                //모든 오브젝트를 숨겨준다
                PoolingManager.instance.ActivatedUnitAllHide();
                break;
            }
        } while (!isNewState);

        //PlayerManager.instance.player.GetComponent<FSMPlayer>().SetAgentStop(true);
        //PlayerManager.instance.player.position = Vector3.zero;
        //PlayerManager.instance.player.GetComponent<FSMPlayer>().SetAgentStop(false);
        //LoadingSceneManager.LoadScene("MainScene2");
        //PlayerManager.instance.LoadScene("MainScene2");
    }
}

public enum SpawnState { Ready, PhaseOne, PhaseTwo, PhaseThree, End };