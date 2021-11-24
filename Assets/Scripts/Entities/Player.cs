using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class Player : Entity
{
    public List<Items> items;

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void Move(MapCoordinate mapCoordinate)
    {

    }

    public void UseAbilities()
    {

    }

    public void UseItem()
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
