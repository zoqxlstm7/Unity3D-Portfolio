using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    public GameObject[] previewObject = new GameObject[2];  //프리뷰에 적용할 오브젝트
    public Transform spawnParent;                           //생성지점을 담은 부모 오브젝트

    GameObject[] previewWarior = new GameObject[3];         //전사 프리뷰 오브젝트 풀링
    GameObject[] previewArcher = new GameObject[3];         //궁수 프리뷰 오브젝트 풀링

    bool isPreview = false;                                 //현재 프리뷰상태인지 여부
    Unit SelectedUnit = Unit.None;                          //현재 선택된 유닛을 구별할 열거형 변수

    private void Start()
    {
        //전사 프리뷰 오브젝트 풀링
        for (int i  = 0; i < previewWarior.Length; i++)
        {
            previewWarior[i] = Instantiate(previewObject[0], spawnParent.GetChild(i).transform);
            previewWarior[i].SetActive(false);
        }

        //궁수 프리뷰 오브젝트 풀링
        for (int i = 0; i < previewArcher.Length; i++)
        {
            previewArcher[i] = Instantiate(previewObject[1], spawnParent.GetChild(i).transform);
            previewArcher[i].SetActive(false);
        }
    }

    void Update()
    {
        //배틀씬이 아닌경우 프리뷰 생성 동작 제어
        //if (!PlayerManager.isBattle)
        //    return;

        //프리뷰 상태일 경우 마우스 포인터를 따라 프리뷰 생성
        if(isPreview)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                spawnParent.position = hit.point;
            }
        }

        //프리뷰 제거
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPreview = false;
            PreviewDeActivated();

            SelectedUnit = Unit.None;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isPreview = false;
            PreviewDeActivated();

            //유닛 생성
            Create(SelectedUnit);
        }

        //게임시스템상 플레이어가 움직일수 없거나 프리뷰 상태일 경우 아래 동작 제어
        if (!PlayerManager.isMove || isPreview)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isPreview = true;

            //전사 프리뷰 생성
            for (int i = 0; i < previewWarior.Length; i++)
            {
                previewWarior[i].SetActive(true);
            }
            SelectedUnit = Unit.HeavyKnight;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isPreview = true;

            //궁수 프리뷰 생성
            for (int i = 0; i < previewArcher.Length; i++)
            {
                previewArcher[i].SetActive(true);
            }
            SelectedUnit = Unit.Archer;
        }
    }

    //선택된 유닛에 따라 생성지점에 유닛 생성
    void Create(Unit unit)
    {
        switch(unit)
        {
            case Unit.HeavyKnight:
                for (int i = 0; i < 3; i++)
                {
                    FSMUnit newUnit = PoolingManager.instance.GetHeavyKnight();
                    if(newUnit != null)
                    {
                        newUnit.transform.position = spawnParent.GetChild(i).transform.position;
                        newUnit.gameObject.SetActive(true);
                    }
                }
                break;

            case Unit.Archer:
                for (int i = 0; i < 3; i++)
                {
                    FSMUnit newUnit = PoolingManager.instance.GetArcher();
                    if (newUnit != null)
                    {
                        newUnit.transform.position = spawnParent.GetChild(i).transform.position;
                        newUnit.gameObject.SetActive(true);
                    }
                }

                break;
        }

        SelectedUnit = Unit.None;
    }

    //활성화된 프리뷰 오브젝트 비활성화
    void PreviewDeActivated()
    {
        for (int i = 0; i < 3; i++)
        {
            if(previewWarior[i].activeInHierarchy)
                previewWarior[i].SetActive(false);

            if (previewArcher[i].activeInHierarchy)
                previewArcher[i].SetActive(false);
        }
    }
}

public enum Unit { HeavyKnight, Archer, None };
