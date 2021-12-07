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

    public MapCoordinate currentTile;
    public float health;
    public List<Ability> abilities;
    public EntityType entityType;
    public int movementRange;

    public virtual void Damage(float dmg)
    {
        health -= dmg;

        if(entityType == EntityType.PLAYER)
        {
            Debug.Log("Player health: " + health.ToString());
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float hl) {
        health += hl;
        if(entityType == EntityType.PLAYER)
        {
            Debug.Log("Player health: " + health.ToString());
        }
    }

    public virtual void Die()
    {
        // If the entity is the player
        if (entityType == EntityType.PLAYER)
        {
            StopAllCoroutines();
            Debug.Log("Player dead");

            // Load the lose scene
            EventSystem.Invoke(Events.LoadLose);
        }
        else if(entityType == EntityType.BOSS)
        {
            StopAllCoroutines();
            // Load the win scene
            EventSystem.Invoke(Events.LoadWin);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // adds and removes entities to list on creation and destruction
    public static List<Entity> All = new List<Entity>();
    protected void OnEnable() => All.Add(this);
    protected void OnDisable() => All.Remove(this);

    // pure virtuals
    public abstract void ProcessTurn();
    protected void EndTurn()
    {
        EventSystem.Invoke(Events.TurnEnded);
    }
   
    public void SetCurrentTile(MapCoordinate newCoord)
    {
        currentTile = newCoord;
        var mtp = MapManager.GetMap().GetTileProperties(newCoord);
        mtp.tile_enitity = new Maybe<Entity>(this);
    }

    public void ChangeCurrentTile(MapCoordinate newCoord)
    {
        var mtp = MapManager.GetMap().GetTileProperties(currentTile);
        mtp.tile_enitity = new Maybe<Entity>();
        SetCurrentTile(newCoord);
    }
}
