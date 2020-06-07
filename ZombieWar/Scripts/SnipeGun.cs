﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnipeGun : Gun
{
    public override void Fire(Actor owner)
    {
        Bullet newBullet = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BulletManager.Generate((int)bulletStyle, firePoint.position);

        if (newBullet != null)
        {
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

            // 발사 이펙트 생성 함수 실행
            GenerateMuzzleEffect();

            newBullet.Fire(owner, firePoint.forward, Damage, firePoint.position, rangeOfShot, true);
            // 총구 방향에 맞춰 총알 회전
            newBullet.transform.rotation = firePoint.rotation;
            // 남은 총알 감소
            remainBulletCount--;

            lastActionTime = Time.time;

            // 탄피생성 함수 실행
            GenerateEmptyShell((int)bulletStyle, owner);
        }
    }
}
