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
