using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public void ProcessTurn()
    {

    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public MapTile GetDesiredMove(MapCoordinate selfTile, MapCoordinate playerTile, MapCoordinate[] otherTargets)
    {
        /*
         * If playerTile is close enough to move next to then move to player
         * If not move to nearest point of interest in otherTargets
         */

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
    }

    public void UseAbilities(MapCoordinate currentTile)
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
}
