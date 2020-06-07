﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Gun
{
    const int MAX_GENERATE_BULLET = 5;  // 최대 산탄총알 생성 횟수
    const float DEFAULT_ROTATION = 15f;

    public override void Fire(Actor owner)
    {
        // 시작 산탄 발사 방향
        float rotation = -30f;

        if (owner.photonView.IsMine)
        {
            // 총 종류에 따른 사운드 재생
            GameManager.Instance.SoundManager.PlaySFX(bulletStyle.ToString());
        }
        else
        {
            // 다른 사람에 것인 경우 소리를 줄여서 출력
            GameManager.Instance.SoundManager.PlaySFX(bulletStyle.ToString(), SoundManager.OTHER_SFX_SOUND_VOLUME);
        }

        // 발사되는 산탄수만큼 탄알 반복 생성 후 처리
        for (int i = 0; i < MAX_GENERATE_BULLET; i++)
        {
            Bullet newBullet = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BulletManager.Generate((int)bulletStyle, firePoint.position);

            if (newBullet != null)
            {
                // 발사 이펙트 생성 함수 실행
                GenerateMuzzleEffect();

                newBullet.Fire(owner, firePoint.forward, Damage, firePoint.position, rangeOfShot);

                // 산탄 방향 지정
                Quaternion quat = Quaternion.identity;
                quat.eulerAngles = new Vector3(firePoint.localRotation.x + rotation, firePoint.localRotation.y, firePoint.localRotation.z);
                firePoint.localRotation = quat;

                // 총구 방향에 맞춰 총알 회전
                newBullet.transform.rotation = firePoint.rotation;

                // 산탄 방향 변경
                rotation += DEFAULT_ROTATION;
            }
        }

        // 남은 총알 감소
        remainBulletCount--;

        lastActionTime = Time.time;

        // 탄피생성 함수 실행
        GenerateEmptyShell((int)bulletStyle, owner);
    }
}
