using System.Collections.Generic;
using UnityEngine;
using Core;

public abstract class Actor : MonoBehaviour
{
    public static List<Actor> All = new List<Actor>();
    
    private void OnEnable() => All.Add(this);
    private void OnDisable() => All.Remove(this);

    public bool canAct = true;

    public abstract void StartTurn();
    
    protected void EndTurn()
    {
        EventSystem.Invoke("TurnEnded");
    }
    
}
