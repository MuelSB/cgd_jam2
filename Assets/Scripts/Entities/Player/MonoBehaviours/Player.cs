using System;
using UnityEngine;
using Core;

public class Player : Entity
{
    private void Start()
    {
        // tell the UI about the player abilities
        foreach (var ability in abilities)
        {
            OnNewAbility(ability);
        }
    }

    public void OnNewAbility(Ability ability)
    {
        var data = new ButtonData { Ability = ability, Callback = () => UseAbility(ability) };
        EventSystem.Invoke<ButtonData>(Events.AddAbility, data);    
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    private void UseAbility(Ability ability)
    {
        // we dont have a way to use abilities
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
