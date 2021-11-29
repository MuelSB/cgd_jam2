using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class Player : Entity
{
    private void Start()
    {
        // broadcasts the player
        EventSystem.Invoke(Events.BroadcastPlayer, this);
    }

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
    
}
