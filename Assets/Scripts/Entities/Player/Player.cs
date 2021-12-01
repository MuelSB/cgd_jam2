using System;
using UnityEngine;
using Core;
using UnityEditor.SceneManagement;

public class Player : Entity
{
    [Header("Player Components")]
    [SerializeField] private PlayerInput _input = default;
    [SerializeField] private PlayerMovement _movement = default;
    
    [SerializeField] private int AP = 2;
    
    private Ability _ability = null;
    
    private void Start()
    {
        // tell the UI about the player abilities
        foreach (var ability in abilities) OnNewAbility(ability);
    }

    public void OnNewAbility(Ability ability)
    {
        var data = new ButtonData { Ability = ability, Callback = () => AbilitySelected(ability) };
        EventSystem.Invoke<ButtonData>(Events.AddAbility, data);    
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    private void AbilitySelected(Ability ability)
    {
        // do nada while moving
        if (_movement.IsMoving) return;
        
        // set the current ability
        _ability = ability;
        
        // TODO : ADD SELECTION HIGHLIGHT
        
        // hack in the movement
        _input.onSelected.AddListener( OnSelectMapTile );
    }

    public void OnSelectMapTile(MapCoordinate coord)
    {
        // hack in move stuff
        if (_ability.name == "Move")
        {
            // TODO : need to check if move is in range
            // start the move selection coroutine
            _movement.MovePlayer(this, MapManager.GetMap().GetTileObject(coord));
            print("Move");
        }
        else
        {
            // if ( ability can be used ( coord ) ) 
            // use ability 
            // clear input callbacks
            print("Other Ability");
        }
        
        // stop input
        _input.onSelected.RemoveAllListeners();
    }

    public void OnControlled()
    {
        
    }

    public void OnUnControlled()
    {

    }

    public override void ProcessTurn()
    {
        EventSystem.Invoke(Events.PlayerTurnStarted);
    }

    [ContextMenu("End Turn")]
    public void TestEndTurn()
    {
        EndTurn();
    }
}
