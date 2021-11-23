using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    private MapCoordinate currentTile;

    public void ProcessTurn()
    {
        currentTile = new MapCoordinate(1, 1);
        StartCoroutine(TurnCoroutine());
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    private List<MapCoordinate> GetDesiredMove()
    {
        MapCoordinate playerTile = new MapCoordinate(5, 5);

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

        List<MapCoordinate> path = Pathfinding.FindRoute(currentTile, target);

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

    private void UseAbilities(MapCoordinate currentTile)
    {
        bool usedStandardAbility = false;
        foreach(Ability ability in abilities)
        {
            if (ability.freeUse == false)
            {
                if (usedStandardAbility == false)
                {
                    usedStandardAbility = true;
                }
                else
                {
                    continue;
                }
            }

            MapCoordinate target = ability.CanUseAbility(currentTile);
            if(target != null)
            {
                ability.UseAbility(target);
            }

        }
    }

    private IEnumerator TurnCoroutine()
    {
        GetDesiredMove();

        yield break;
        //EnemyManager.Instance.CalculatePath()
    }
}
