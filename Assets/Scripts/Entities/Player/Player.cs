using System.Linq;
using UnityEngine;
using Core;
using Unity.VisualScripting;


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

    [SerializeField] private int averageEnemyExp = 3;

    [SerializeField] private int apPerLevel = 1;
    [SerializeField] private int ap = 2;
    [SerializeField] private int maxAP = 2;

    [SerializeField] private float healthPerLevel = 10;
    [SerializeField] private float hp = 10.0f;
    [SerializeField] private Sprite APSprite;

    private int experience = 0;
    private int level = 1;

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
        for (int i = 0; i < ap; i++)
        {
            newAPSprite(APSprite);
        }
        // Set the player's starting health value based on HP, AP based on MAX_AP
        health = hp;
        ap = maxAP;
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
        
        // hack in move stuff
        if (_ability.name == "Move")
        {
            //Checks if Ability Costs exceeds current AP
            if (ap - _ability.cost < 0)
            {
                Debug.Log("Not enough AP!");
                return;
            }
            else
            {
                //Takes away AP from the cost of the ability
                ap -= _ability.cost;

                //Invokes Event to update UIManager to reduce AP
                var pck = new APpackage { CurrentAP = ap, APToReduce = _ability.cost };
                EventSystem.Invoke(Events.ReduceAP, pck);
            }

            // TODO : need to check if move is in range
            // start the move selection coroutine
            _movement.MovePlayer(this, MapManager.GetMap().GetTileObject(coord));
        }
        else
        {
            // not fully implimented
            //var co = StartCoroutine(AbilityManager.Instance.ExecuteAbility(_ability, coord));
            if (ap - _ability.cost < 0)
            {
                Debug.Log("Not enough AP!");
                return;
            }
            else
            {
                ap -= _ability.cost;
                var pck = new APpackage { CurrentAP = ap, APToReduce = _ability.cost };
                EventSystem.Invoke(Events.ReduceAP, pck);
            }
            print("Other Ability");
            if (ap == 0)
            {
                EventSystem.Invoke(Events.PlayerTurnEnded);
            }
            _abilityCoroutine = StartCoroutine(AbilityManager.Instance.ExecuteAbility(_ability,this, coord));
        }
        
        // remove highlight
        EventSystem.Invoke(Events.AbilityDeselected);
    }

    public override void ProcessTurn()
    {
        ap = maxAP;
        ap = 1 + level;
        EventSystem.Invoke(Events.ResetAP);
        EventSystem.Subscribe<Enemy>(Events.EntityKilled, OnKilledEnemy);
        EventSystem.Invoke(Events.PlayerTurnStarted);
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
        ap += apPerLevel;
        health += healthPerLevel;
    }
}
