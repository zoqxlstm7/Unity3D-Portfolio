using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideManager : MonoBehaviour
{
    public Image guideImage;                // 가이드 메세지가 출력될 이미지

    string[] guideMessage = new string[7];  // 가이드 라인 메세지를 담는 배열
    float activatedTime;                    // 가이드 이미지 노출 시간    
    int guideNum = 0;                       // 출력될 가이드 메세지 번호
    bool isGuideLine = false;               // 가이드 이미지가 노출될지 여부

    private void Start()
    {
        guideImage.gameObject.SetActive(false);

        guideMessage[1] = "W S D A 키로 이동, 마우스로 시야를 회전할 수 있습니다.";
        guideMessage[2] = "장비를 습득한 후, I키를 눌러 인벤토리를 열고 장비를 장착하세요.";
        guideMessage[3] = "퀵슬롯에 물약을 끌어와 등록시킬 수 있습니다.";
        guideMessage[4] = "Q 키를 눌러 퀵슬롯에 등록된 물약을 사용할 수 있습니다.";
        guideMessage[5] = "SPACE 키를 눌러 대쉬를 사용할 수 있습니다.";
        guideMessage[6] = "마우스 좌클릭으로 공격을 할 수 있습니다. 미로를 헤쳐나가 보스를 물리쳐 보세요.";

        // 초기 가이드 실행
        SetGuide();
    }

    private void Update()
    {
        activatedTime -= Time.deltaTime;

        // 노출시간이 다 되었을 때, 가이드 이미지 숨김 처리
        if (activatedTime <= 0f && isGuideLine)
        {
            isGuideLine = false;
            guideImage.gameObject.SetActive(false);
        }
    }

    // 가이드 정보 셋팅
    public void SetGuide()
    {
        // 출력될 가이드 메세지 셋팅 및 노출시간 초기화
        guideNum++;
        isGuideLine = true;
        activatedTime = 3f;

        // 가이드 이미지 애미메이션 재생 및 가이드 메세지 노출
        guideImage.gameObject.SetActive(true);
        guideImage.GetComponentInChildren<Text>().text = guideMessage[guideNum];
        guideImage.GetComponent<Animator>().SetTrigger("push");
    }
}
