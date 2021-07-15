using System.Collections.Generic;
using UnityEngine;

public class FixedEnemyFactory : IEnemyFactory
{
    public int NOfStates { get; } = 1;

    public Enemy GenerateEnemy(Map map, List<Enemy> enemies)
    {
        Vector2Int position = EnemyFactoryUtility.GetRandomAvailablePosition(map, enemies, 0);
        int rotation = EnemyFactoryUtility.GetRotation(map, position);
        int visionLength = EnemyFactoryUtility.GetVisionLength();

        EnemyState enemyState = new EnemyState
        {
            Position = position,
            Rotation = rotation,
            VisionLength = visionLength
        };

        // return status -1 = enemy cannot be created => we return null
        if (enemyState.Position.x == -1) return null;

        List<EnemyState> pattern = new List<EnemyState> { enemyState };
        Enemy enemy = new Enemy(pattern);

        return enemy;
    }
}
