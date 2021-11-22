using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Items
{
    public enum ItemsType
    {
        INVALID,
        HEALTH,
        STRENGTH
    }

    public bool usedItem;
    public abstract void UseItem();

}
