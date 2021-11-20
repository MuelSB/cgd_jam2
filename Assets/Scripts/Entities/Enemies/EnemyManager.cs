using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private Dictionary<Enemy, Vector2> minions;
    private Boss boss;
    void Init(int starterMinions)
    {
        if(Instance != null)
        {
            Debug.LogError("More than one EnemyManager instance in scene!");
            Destroy(gameObject);
        }
        Instance = this;

        for(int i = 0; i < starterMinions; ++i)
        {
            minions.Add(new Enemy(), Vector2.zero);
        }
    }

    public void EnemyTurn()
    {
        foreach(var enemy in minions)
        {

        }
    }

    private void CalculateEnemyPath(Enemy enemy)
    {

    }

    private void CheckPlayerAdjacency(Enemy enemy)
    {

    }
}
