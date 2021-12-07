using System;
using System.Collections.Generic;
using UnityEngine;
using Core;
using UnityEditor.SceneManagement;

public struct APpackage
{
    public int CurrentAP;
    public int APToReduce;
}

public class Player : Entity
{
    [Header("Player Components")]
    [SerializeField] private PlayerInput _input = default;
    [SerializeField] private PlayerMovement _movement = default;
    [SerializeField] private PlayerAttack _skills = default;

    [SerializeField] private int averageEnemyExp = 3;

    [SerializeField] private int APPerLevel = 1;
    [SerializeField] private int AP = 2;
    [SerializeField] private int MAX_AP = 2;

    [SerializeField] private float healthPerLevel = 10;
    [SerializeField] private float HP = 10.0f;

    [SerializeField] private Sprite APSprite;

    private int experience = 0;
    private int level = 1;

    private Ability _ability = null;
    
    private void Start()
    {
        // tell the UI about the player abilities
        foreach (var ability in abilities) OnNewAbility(ability);
        for (int i = 0; i < AP; i++)
        {
            newAPSprite(APSprite);
        }
        // Set the player's starting health value based on HP, AP based on MAX_AP
        health = HP;
        AP = MAX_AP;
    }

    public void OnNewAbility(Ability ability)
    {
        var data = new ButtonData { Ability = ability, Callback = () => AbilitySelected(ability) };
        EventSystem.Invoke<ButtonData>(Events.AddAbility, data);    
    }
    public void newAPSprite(Sprite sprite)
    {
        var data = new UIData { image_src = sprite };
        EventSystem.Invoke<UIData>(Events.AddAP, data);
    }
    
    //public void TakeDamage(int damage)
    //{
    //    health -= damage;
    //}

    private void AbilitySelected(Ability ability)
    {
        // do nada while moving
        if (_movement.IsMoving) return;
        
        // set the current ability
        _ability = ability;

        // TODO : ADD SELECTION HIGHLIGHT
        var highlights = _ability.GetTargetableTiles(currentTile, EntityType.PLAYER);
        EventSystem.Invoke(Events.HighlightTiles, highlights);

        // hack in the movement
        _input.onSelected.AddListener( OnSelectMapTile );
    }

    public void OnSelectMapTile(MapCoordinate coord)
    {
        // hack in move stuff
        if (_ability.name == "Move")
        {

            //Checks if Ability Costs exceeds current AP
            if (AP - _ability.cost < 0)
            {
                Debug.Log("Not enough AP!");
                return;
            }
            else
            {
                //Takes away AP from the cost of the ability
                AP -= _ability.cost;

                //Invokes Event to update UIManager to reduce AP
                var pck = new APpackage { CurrentAP = AP, APToReduce = _ability.cost };
                EventSystem.Invoke(Events.ReduceAP, pck);
            }

            // TODO : need to check if move is in range
            // start the move selection coroutine
            _movement.MovePlayer(this, MapManager.GetMap().GetTileObject(coord));
            print("Move");

            if (AP == 0)
            {
                Debug.Log("Player endturn is called");
                EventSystem.Invoke(Events.PlayerTurnEnded);
            }
        }
        else
        {
            // not fully implimented
            //var co = StartCoroutine(AbilityManager.Instance.ExecuteAbility(_ability, coord));
            if (AP - _ability.cost < 0)
            {
                Debug.Log("Not enough AP!");
                return;
            }
            else
            {
                AP -= _ability.cost;
                var pck = new APpackage { CurrentAP = AP, APToReduce = _ability.cost };
                EventSystem.Invoke(Events.ReduceAP, pck);
            }
            print("Other Ability");
            if (AP == 0)
            {
                EventSystem.Invoke(Events.PlayerTurnEnded);
            }
        }

        /*
        if (AP == 0)
        {
            //Ends Player Turn
            EventSystem.Invoke(Events.PlayerTurnEnded);
            // stop input
            _input.onSelected.RemoveAllListeners();
            // remove highlight
            EventSystem.Invoke(Events.DisableHighlights);
        }*/
        //An Attempt to delay event system from invoking anything to pause ProcessTurn failed.

        if (AP == 0)
        {
            // stop input
            _input.onSelected.RemoveAllListeners();
        }
       
        // remove highlight
        EventSystem.Invoke(Events.DisableHighlights);
    }

    public override void ProcessTurn()
    {
        AP = MAX_AP;
        EventSystem.Invoke(Events.ResetAP);
        EventSystem.Subscribe<Enemy>(Events.EntityKilled, OnKilledEnemy);
        EventSystem.Invoke(Events.PlayerTurnStarted);
    }

    [ContextMenu("End Turn")]
    public void TestEndTurn()
    {
        if (AP == 0)
            EndTurn();
    }

    public void setMaxAP(int max_ap)
    {
        AP = max_ap;
    }
    
    protected override void EndTurn()
    {
        EventSystem.Unsubscribe<Enemy>(Events.EntityKilled, OnKilledEnemy);
        base.EndTurn();
    }

    private void OnKilledEnemy(Enemy enemy)
    {
        GainExperience(enemy.expValue);
    }

    public void GainExperience(int exp)
    {
        experience += exp;
        while(experience >= Mathf.CeilToInt(2 * (Mathf.Pow(level, 1.5f))) * averageEnemyExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        AP += APPerLevel;
        health += healthPerLevel;
    }
}
