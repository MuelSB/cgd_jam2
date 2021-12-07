using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;

public class Player : Entity
{
    [Header("Player Components")]
    [SerializeField] private PlayerInput _input = default;
    [SerializeField] private PlayerMovement _movement = default;

    [SerializeField] private int averageEnemyExp = 3;

    [SerializeField] private int APPerLevel = 1;
    [SerializeField] private int AP = 2;

    [SerializeField] private float healthPerLevel = 10;
    [SerializeField] private float HP = 10.0f;

    private int experience = 0;
    private int level = 1;

    private Ability _ability = null;

    private new void OnEnable()
    {
        EventSystem.Subscribe<Enemy>(Events.EntityKilled, OnKilledEnemy);
        _movement._moveComplete.AddListener(OnActionComplete);
        base.OnEnable();
    }

    private new void OnDisable()
    {
        EventSystem.Unsubscribe<Enemy>(Events.EntityKilled, OnKilledEnemy);
        _movement._moveComplete.AddListener(OnActionComplete);
        base.OnDisable();
    }
    
    private void Start()
    {
        // tell the UI about the player abilities
        foreach (var ability in abilities) OnNewAbility(ability);

        // Set the player's starting health value based on HP
        health = HP;
    }

    public void OnNewAbility(Ability ability)
    {
        var data = new ButtonData { Ability = ability, Callback = () => AbilitySelected(ability) };
        EventSystem.Invoke<ButtonData>(Events.AddAbility, data);    
    }
    
    private void AbilitySelected(Ability ability)
    {
        // do nada while moving
        if (_movement.IsMoving) return;
        
        // set the current ability
        _ability = ability;

        // Send highlight into to the UI
        var highlights = _ability.GetTargetableTiles(currentTile, EntityType.PLAYER);
        EventSystem.Invoke(Events.AbilitySelected, highlights);

        // hack in the movement
        _input.onSelected.AddListener( OnSelectMapTile );
    }

    private void OnActionComplete()
    {
        if (AP >= 1) return;
        EventSystem.Invoke(Events.PlayerTurnEnded);
        EndTurn();
    }
    
    private void OnSelectMapTile(MapCoordinate coord)
    {
        // quit if range invalid
        if (!InRange(coord)) return;
        
        // minus AP
        AP -= _ability.cost;
        
        // hack in move stuff
        if (_ability.name == "Move")
        {
            _movement.MovePlayer(this, MapManager.GetMap().GetTileObject(coord));
        }
        else
        {
            // not fully implimented
            var co = StartCoroutine(AbilityManager.Instance.ExecuteAbility(_ability,this, coord));
        }
        
        // stop input
        _input.onSelected.RemoveAllListeners();
        
        // remove highlight
        EventSystem.Invoke(Events.AbilityDeselected);
    }

    private bool InRange(MapCoordinate target)
    {
        var validTiles = _ability.GetTargetableTiles(currentTile, EntityType.PLAYER);
        var result = false;

        foreach (var t in validTiles.Where(t => t.x == target.x && t.y == target.y))
        {
            result = true;
        }
        
        return result;
    }

    public override void ProcessTurn()
    {
        AP = 1 + level;
        EventSystem.Invoke(Events.PlayerTurnStarted);
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
