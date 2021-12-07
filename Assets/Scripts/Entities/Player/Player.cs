using System.Linq;
using UnityEngine;
using Core;
using Unity.VisualScripting;


public class Player : Entity
{
    [Header("Player Components")]
    [SerializeField] private PlayerInput _input = default;
    [SerializeField] private PlayerMovement _movement = default;

    [SerializeField] private int averageEnemyExp = 3;

    [SerializeField] private int apPerLevel = 1;
    [SerializeField] private int ap = 2;
    [SerializeField] private int maxAP = 2;

    [SerializeField] private float healthPerLevel = 10;
    [SerializeField] private float hp = 10.0f;

    private int _experience = 0;
    private int _level = 1;

    private Ability _ability = null;
    private Coroutine _abilityCoroutine = null;

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

        // Set the player's starting health value based on HP, AP based on MAX_AP
        health = hp;
        ap = maxAP;
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
        if (_abilityCoroutine != null) return;

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
        print("Complete");
        if (ap > 0) return;
        _input.onSelected.RemoveAllListeners();
        EventSystem.Invoke(Events.PlayerTurnEnded);
        EndTurn();
    }
    
    private void OnSelectMapTile(MapCoordinate coord)
    {
        // quit if range invalid
        if (!InRange(coord)) return;
        
        // minus AP
        ap -= _ability.cost;
        
        // hack in move stuff
        if (_ability.name == "Move")
        {
            _movement.MovePlayer(this, MapManager.GetMap().GetTileObject(coord));
        }
        else
        {
            _abilityCoroutine = StartCoroutine(AbilityManager.Instance.ExecuteAbility(_ability,this, coord));
        }
        
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
        ap = 1 + _level;
        EventSystem.Invoke(Events.PlayerTurnStarted);
    }

    private void OnKilledEnemy(Enemy enemy)
    {
        GainExperience(enemy.expValue);
    }

    public void GainExperience(int exp)
    {
        _experience += exp;
        while(_experience >= Mathf.CeilToInt(2 * (Mathf.Pow(_level, 1.5f))) * averageEnemyExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        _level++;
        ap += apPerLevel;
        health += healthPerLevel;
    }
}
