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

    public MapTile Move(MapTile mapTile)
    {

        return null;
    }

    public void UseAbilities()
    {

    }

    public void UseItem()
    {
        
    }

}
