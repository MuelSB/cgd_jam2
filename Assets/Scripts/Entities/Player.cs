using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
