using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class Enemy : Entity
{

    public override void ProcessTurn() 
    {
        StartCoroutine(TurnCoroutine());
    }

    private List<MapCoordinate> GetDesiredMove()
    {
        MapCoordinate playerTile = new MapCoordinate(1, 0);

        MapCoordinate target = playerTile;

        int widthTo, heightTo;

        MapManager.GetAbsoluteTileCountTo(currentTile, playerTile, out widthTo, out heightTo);
        
        if(widthTo + heightTo > movementRange)
        {
            List<MapCoordinate> potentialTargets = new List<MapCoordinate>();
            foreach(Ability ability in abilities)
            {
                if(ability.targetType == Ability.AbilityTarget.BUILDING)
                {
                    // Find nearest building, that is your target
                }
            }
        }

        List<MapCoordinate> path = Pathfinding.FindRoute(target, currentTile);

        return path;
    }

    private IEnumerator UseAbilities()
    {
        int apUsed = 0;
        foreach(Ability ability in abilities)
        {
            int cost = ability.cost;
            if (cost > 0 && apUsed > 0) continue;


            List<MapCoordinate> targets = ability.GetTargetableTiles(currentTile);
            if(targets.Count > 0)
            {
                MapCoordinate target = targets[0];
                yield return AbilityManager.Instance.ExecuteAbility(ability, target);
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
        MapCoordinate pos = currentTile;
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
            yield return new WaitForSeconds(0.2f);
        }
        ChangeCurrentTile(pos);
    }
}
