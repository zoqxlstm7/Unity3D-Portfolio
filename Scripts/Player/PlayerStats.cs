using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public override void Die()
    {
        base.Die();

        //player kill
        PlayerManager.instance.KillPlayer();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}
