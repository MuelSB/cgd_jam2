using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public enum EntityType
    {
        PLAYER,
        MINION,
        BOSS,
    }

    public float health;
    public List<Ability> abilities;
    public EntityType type;
    public int movementRange;

    public void Damage(float dmg)
    {
        health -= dmg;
        if(health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

}
