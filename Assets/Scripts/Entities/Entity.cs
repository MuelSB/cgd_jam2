using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

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

    // adds and removes entities to list on creation and destruction
    public static List<Entity> All = new List<Entity>();
    private void OnEnable() => All.Add(this);
    private void OnDisable() => All.Remove(this);

    // pure virtuals
    public abstract void ProcessTurn();
    protected virtual void EndTurn()
    {
        EventSystem.Invoke(Events.TurnEnded);
    }
   

}
