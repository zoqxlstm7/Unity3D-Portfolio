using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalDeadUI : MonoBehaviour
{
    #region Property
    [SerializeField] Button tamingBtn;      // 테이밍 버튼

    public Animal owner { get; set; }       // 소유자
    public string FilePath { get; set; }    // 파일 경로
    #endregion Property

    #region Unity Methods
    private void Update()
    {
        // 테이밍 리스트가 최대치인지 검사하여 테이밍 버튼 활성/비활성
        InGameSceneManager inGameSceneManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>();
        if(inGameSceneManager.player.TamingCount == InGameSceneManager.MAX_TAMING_COUNT)
        {
            tamingBtn.interactable = false;
        }
        else
        {
            tamingBtn.interactable = true;
        }
    }
    #endregion Unity Methods

    #region Other Methods
    /// <summary>
    /// 오브젝트 제거 함수
    /// </summary>
    /// <param name="inGameSceneManager"></param>
    void Remove(InGameSceneManager inGameSceneManager)
    {
        // 소유자 제거
        inGameSceneManager.animalManager.Remove(owner.FilePath, owner);
        // ui 제거
        inGameSceneManager.deadUIManager.Remove(FilePath, this);
    }
    #endregion Other Methods

    #region Button Methods
    /// <summary>
    /// 테이밍 버튼 처리 함수
    /// </summary>
    public void OnTamingBtn()
    {
        // 플레이어 주변 랜덤 위치 계산
        Vector3 ranPos = Random.onUnitSphere;
        ranPos *= 2f;
        ranPos.z = 0f;

        InGameSceneManager inGameSceneManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>();
        // 같은 종류에 동물을 생성하여 플레이어 테이밍리스트에 추가
        Animal animal = inGameSceneManager.animalManager.Generate(owner.FilePath, inGameSceneManager.player.transform.position + ranPos);
        inGameSceneManager.player.tamingList.Add(animal);

        // 아군 레이어로 변경
        animal.gameObject.layer = LayerMask.NameToLayer("Player");
        animal.targetMask = 1 << LayerMask.NameToLayer("Enemy");

        // 오브젝트 제거
        Remove(inGameSceneManager);
    }

    /// <summary>
    /// 수집 버튼 처리 함수
    /// </summary>
    public void OnColletBtn()
    {
        InGameSceneManager inGameSceneManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>();
        // 수집카운트 증가

        // 오브젝트 제거
        Remove(inGameSceneManager);
    }
    #endregion Button Methods
}
