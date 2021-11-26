using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class Player : Entity
{
    public List<Items> items;

    Player()
    {
        health = 100.0f;
        type = EntityType.PLAYER;
        movementRange = 5;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void UseAbilities()
    {

    }

    public void UseItem()
    {
        
    }

    public void OnControlled()
    {
        Debug.Log("Player controlled.");
    }

    public void OnUnControlled()
    {

    }

    public override void ProcessTurn()
    {
        
    }

    [ContextMenu("End Turn")]
    public void TestEndTurn()
    {
        EndTurn();
    }
}
