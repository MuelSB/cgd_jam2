using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Move(MapCoordinate mapCoordinate)
    {

    }

    public void UseAbilities()
    {

    }

    public void UseItem()
    {
        
    }

}
