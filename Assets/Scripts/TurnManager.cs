using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core;

public class TurnManager : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string BossTag = "Boss";
    
    public void OnEnable()
    {
        EventSystem.Subscribe(Events.LevelLoaded, OnLevelLoaded);
        EventSystem.Subscribe(Events.TurnEnded, OnTurnEnded);
    }

    public void OnDisable()
    {
        EventSystem.Unsubscribe(Events.LevelLoaded, OnLevelLoaded);
        EventSystem.Unsubscribe(Events.TurnEnded, OnTurnEnded);
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
        EventSystem.Invoke(Events.RoundStarted);
        
        // get actor list 
        _actors = GetInitiativeList();
        
        // do turn
        StartTurn();
    }

    private List<Actor> GetInitiativeList()
    {
        // create the list
        var initiative = new List<Actor>();
        
        // add the player to the first position
        initiative.Add(Actor.All.Find(actor => actor.CompareTag(PlayerTag)));
        
        // add everything that is not a player or boss
        initiative.AddRange(Actor.All.Where(actor => !actor.CompareTag(PlayerTag) && !actor.CompareTag(BossTag)));
        
        // add the player to the first position
        initiative.Add(Actor.All.Find(actor => actor.CompareTag(BossTag)));
    
        return initiative;
    }

    private void EndRound()
    {
        // invoke event
        EventSystem.Invoke(Events.RoundEnded);
        
        // increment and start new round
        _round++;
        StartRound();
    }

    private void StartTurn()
    {
        // invoke event
        EventSystem.Invoke(Events.TurnStarted);
        
        // get actor
        var actor = _actors[_turn];
        
        // skips if actor cant act
        if ( actor != null || !actor.canAct ) EventSystem.Invoke(Events.TurnEnded);
        
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
