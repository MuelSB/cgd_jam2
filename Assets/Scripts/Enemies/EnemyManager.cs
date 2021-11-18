using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private Dictionary<Enemy, Vector2> minions;
    void Init()
    {
        if(Instance != null)
        {
            Debug.LogError("More than one EnemyManager instance in scene!");
            Destroy(gameObject);
        }
        Instance = this;

    }

    public void EnemyTurn()
    {
        foreach(var enemy in minions)
        {
            enemy.Key.Move();
        }
    }

    private void CalculateEnemyPath(Enemy enemy)
    {

    }

    private void CheckPlayerAdjacency(Enemy enemy)
    {

    }
}
