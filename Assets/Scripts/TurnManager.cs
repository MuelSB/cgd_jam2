using System.Collections.Generic;
using UnityEngine;
using Core;

public class TurnManager : MonoBehaviour
{
    // Event reg constants
    private const string LevelLoaded  = "LevelLoaded"; 
    private const string RoundStarted = "RoundStarted"; 
    private const string RoundEnded   = "RoundEnded";
    private const string TurnStarted  = "TurnStarted"; 
    private const string TurnEnded    = "TurnEnded";


    public void OnEnable()
    {
        EventSystem.Subscribe(LevelLoaded, OnLevelLoaded);
        EventSystem.Subscribe(TurnEnded, OnTurnEnded);
    }

    public void OnDisable()
    {
        EventSystem.Unsubscribe(LevelLoaded, OnLevelLoaded);
        EventSystem.Unsubscribe(TurnEnded, OnTurnEnded);
    }

    private List<Actor> _actors = new List<Actor>();

    private int _round = 0;
    private int _turn = 0;
    
    private void OnLevelLoaded()
    {
        // reset tracked values
        Reset();
        
        // start the first round
        StartRound();
    }

    private void Reset()
    {
        _round = 0;
        _turn = 0;
    }

    private void StartRound()
    {
        // invoke event
        EventSystem.Invoke(RoundStarted);
        
        // get actor list 
        _actors = Actor.All;
        
        // do turn
        StartTurn();
    }

    private void EndRound()
    {
        // invoke event
        EventSystem.Invoke(RoundEnded);
        
        // increment and start new round
        _round++;
        StartRound();
    }

    private void StartTurn()
    {
        // invoke event
        EventSystem.Invoke(TurnStarted);
        
        // get actor
        var actor = _actors[_turn];
        
        // skips if actor cant act
        if ( actor != null || !actor.canAct ) EventSystem.Invoke(TurnEnded);
        
        // start the actors turn
        actor.StartTurn();
    }

    private void OnTurnEnded()
    {
        // we purposefully do not invoke the event here
        // instead we subscribe OnTurnEnd() and call it from within the actor
        
        // increase current turn
        _turn++;
        
        // check turn is valid
        if (_turn < _actors.Count)
        {
            // start next turn
            StartTurn();
            return;
        }
        
        // end current round
        EndRound();
    }

}
