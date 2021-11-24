using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core;

public class TurnManager : MonoBehaviour
{   
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

    private List<Entity> _actors = new List<Entity>();

    private int _round = 0;
    private int _turn = 0;
    
    private void OnLevelLoaded()
    {
        // reset tracked values
        Reset();

        // error check 
        if (Entity.All.Count < 1)
        {
            Debug.LogWarning("There are no valid Entities", this);
        }
        
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
        
        // reset the turn
        _turn = 0;
        
        // do turn
        StartTurn();
    }

    private List<Entity> GetInitiativeList()
    {
        // create the list
        var initiative = new List<Entity>();
        
        // add the player to the first position
        initiative.Add(Entity.All.Find(entity => entity.type == Entity.EntityType.PLAYER));
        
        // add everything that is not a player or boss
        initiative.AddRange(Entity.All.Where(entity => entity.type != Entity.EntityType.PLAYER && entity.type != Entity.EntityType.BOSS));
        
        // add the player to the first position
        initiative.Add(Entity.All.Find(entity => entity.type == Entity.EntityType.BOSS));
    
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
        var entity = _actors[_turn];
        
        // skips if actor cant act
        if (entity == null)
        {
            Debug.Log("not a valid entity");
            EventSystem.Invoke(Events.TurnEnded);
            return;
        }

        // start the actors turn
        entity.ProcessTurn();
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
