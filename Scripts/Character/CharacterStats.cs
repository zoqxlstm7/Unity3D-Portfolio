using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth;
    [SerializeField]
    int currentHealth;// { get; private set; }

    public bool isDie = false;

    public Stats damage;
    public Stats armor;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetRivival()
    {
        currentHealth = maxHealth;
        isDie = false;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, damage);

        currentHealth -= damage;
        //Debug.Log(transform.name + " take damaged. " + damage);

        if (currentHealth <= 0 && !isDie)
        {
            isDie = true;
            Die();
        }
    }

    public virtual void Die()
    {
        //Debug.Log(transform.name + "Died.");
    }
}
