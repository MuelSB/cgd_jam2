using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;

public class Enemy : Entity
{
    public List<MapTileProperties.TileType> targetableTiles;

    public EnemiesData.EnemyType enemyType;
    public bool canPassDestroyedTiles = false;
    public Canvas healthUICanvas;
    public Image healthImage;

    public int expValue;

    private float initialHealthWidth;
    private float maxHealth;

    public List<int> abilityTimers;

    private void Start()
    {
        maxHealth = health;
        initialHealthWidth = healthImage.rectTransform.rect.width;
        healthUICanvas.worldCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
    }

    private void Update()
    {
        Camera camera = Camera.main;
        healthUICanvas.transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }

    public override void ProcessTurn() 
    {
        EnemyManager.Instance.currentEnemyTurn = this;
        StartCoroutine(TurnCoroutine());
    }

    public override void Damage(float dmg)
    {
        base.Damage(dmg);
        if (health < maxHealth) healthUICanvas.enabled = true;
        healthImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (health / maxHealth) * initialHealthWidth);
    }

    public override void Die()
    {
        EventSystem.Invoke<Enemy>(Events.EntityKilled, this);
        base.Die();
    }

    private List<MapCoordinate> GetDesiredMove()
    {
        Maybe<MapCoordinate> playerTile = new Maybe<MapCoordinate>();
        Maybe<MapCoordinate> target = new Maybe<MapCoordinate>();
        int widthTo = 0, heightTo = 0;
        var player = MetaGeneratorHelper.getPlayerFromGrid(MapManager.GetMap().GetTiles());
        if(player.is_some)
        {
            playerTile = new Maybe<MapCoordinate>(player.value);
            target = playerTile;

            MapManager.GetAbsoluteTileCountTo(currentTile, playerTile.value, out widthTo, out heightTo);
        }

        
        if((widthTo + heightTo > movementRange) || player.is_some == false)
        {
            foreach(Ability ability in abilities)
            {
                if(ability.targetType == Ability.AbilityTarget.BUILDING)
                {
                    var potentialTargets = MetaGeneratorHelper.getClosestTiles(MapManager.GetMap().GetTiles(), currentTile, targetableTiles);
                    if(potentialTargets.is_some)
                    {
                        target = new Maybe<MapCoordinate>(potentialTargets.value[Random.Range(0, potentialTargets.value.Count)]);
                    }
                }
            }
        }

        if (target.is_some == false) return new List<MapCoordinate>();

        List<MapCoordinate> path = Pathfinding.FindRoute(target.value, currentTile, canPassDestroyedTiles);

        return path;
    }

    private IEnumerator UseAbilities()
    {
        int apUsed = 0;
        for(int i = 0; i < abilities.Count; ++i)
        {
            Ability ability = abilities[i];
            int cost = ability.cost;
            if ((cost > 0 && apUsed > 0) || ability.turnsCooldown > abilityTimers[i]) continue;


            List<MapCoordinate> targets = ability.GetTargetableTiles(currentTile, entityType);
            if(targets.Count > 0)
            {
                MapCoordinate target = targets[Random.Range(0, targets.Count)];
                yield return AbilityManager.Instance.ExecuteAbility(ability, this, target);
                apUsed += cost;
                abilityTimers[i] = 0;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator TurnCoroutine()
    {
        for (int i = 0; i < abilityTimers.Count; ++i) abilityTimers[i]++;
        var path = GetDesiredMove();

        if (path.Count > 0)
        {
            yield return Move(path);
        }
        yield return UseAbilities();

        EndTurn();
        yield break;
    }

    private IEnumerator Move(List<MapCoordinate> path)
    {
        int stepsTaken = 0;
        MapCoordinate pos = currentTile;
        path.Reverse();
        foreach (var step in path)
        {
            float t = 0.0f;
            while (t < 1.0f)
            {
                this.transform.position = Vector3.Slerp(
                    MapManager.GetMap().GetTileObject(pos).transform.position,
                    MapManager.GetMap().GetTileObject(step).transform.position,
                    t += (2f * Time.deltaTime)
                    );
                yield return 0;
            }
            pos = step;
            if (stepsTaken++ >= movementRange) break;
            yield return new WaitForSeconds(0.2f);
        }
        ChangeCurrentTile(pos);
    }
}
