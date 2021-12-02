using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;

public class Enemy : Entity
{
    public EnemiesData.EnemyType enemyType;
    public bool canPassDestroyedTiles = false;
    public Canvas healthUICanvas;
    public Image healthImage;

    private float initialHealthWidth;
    private float maxHealth;

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

    private List<MapCoordinate> GetDesiredMove()
    {
        MapCoordinate playerTile = new MapCoordinate(1, 0);

        MapCoordinate target = playerTile;

        int widthTo, heightTo;

        MapManager.GetAbsoluteTileCountTo(currentTile, playerTile, out widthTo, out heightTo);
        
        if(widthTo + heightTo > movementRange)
        {
            foreach(Ability ability in abilities)
            {
                if(ability.targetType == Ability.AbilityTarget.BUILDING)
                {
                    var potentialTargets = MetaGeneratorHelper.getClosestSpecialTiles(MapManager.GetMap().GetTiles(), currentTile);
                    if(potentialTargets.is_some)
                    {
                        target = potentialTargets.value[Random.Range(0, potentialTargets.value.Count)];
                    }
                }
            }
        }

        List<MapCoordinate> path = Pathfinding.FindRoute(target, currentTile, canPassDestroyedTiles);

        return path;
    }

    private IEnumerator UseAbilities()
    {
        int apUsed = 0;
        foreach(Ability ability in abilities)
        {
            int cost = ability.cost;
            if (cost > 0 && apUsed > 0) continue;


            List<MapCoordinate> targets = ability.GetTargetableTiles(currentTile, entityType);
            if(targets.Count > 0)
            {
                MapCoordinate target = targets[Random.Range(0, targets.Count)];
                yield return AbilityManager.Instance.ExecuteAbility(ability, this, target);
                apUsed += cost;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator TurnCoroutine()
    {
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
                    )
                    + Vector3.up;
                yield return 0;
            }
            pos = step;
            if (stepsTaken++ >= movementRange) break;
            yield return new WaitForSeconds(0.2f);
        }
        ChangeCurrentTile(pos);
    }
}
