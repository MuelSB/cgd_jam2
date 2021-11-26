using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class Enemy : Entity
{
    public MapCoordinate currentTile;

    public override void ProcessTurn() 
    {
        StartCoroutine(TurnCoroutine());
    }

    private List<MapCoordinate> GetDesiredMove()
    {
        MapCoordinate playerTile = new MapCoordinate(2, 2 + currentTile.x);

        MapCoordinate target = playerTile;

        int widthTo, heightTo;

        MapManager.GetTileCountTo(currentTile, playerTile, out widthTo, out heightTo);
        /*
         * If playerTile is close enough to move next to then move to player
         * If not move to nearest point of interest in otherTargets
         */

        if(widthTo + heightTo > movementRange)
        {
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
        /*
        KeyValuePair<int, Ability> bestMoveToAttack = new KeyValuePair<int, Ability>(-1, null);

        foreach(Ability ability in abilities)
        {
            int range = ability.range;
            // If there is a tile in range to attack already then do so
            int distanceToTargetable = 0;
            // Else find nearest targetable tile and store its distance
            if (distanceToTargetable < bestMoveToAttack.Key || bestMoveToAttack.Key == -1)
            {
                bestMoveToAttack = new KeyValuePair<int, Ability>(distanceToTargetable, ability);
            }
        }
        return null;
        */
    }

    private IEnumerator UseAbilities()
    {
        bool usedStandardAbility = false;
        foreach(Ability ability in abilities)
        {
            bool isFreeAbility = ability.freeUse;
            if (usedStandardAbility && ability.freeUse == false) continue;


            List<MapCoordinate> targets = ability.CanUseAbility(currentTile);
            if(targets.Count > 0)
            {
                ability.UseAbility(targets[0]);
                if (isFreeAbility == false) usedStandardAbility = true;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator TurnCoroutine()
    {
        var path = GetDesiredMove();

        yield return Move(path);

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
            currentTile = step;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
