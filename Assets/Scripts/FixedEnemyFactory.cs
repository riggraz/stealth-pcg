using System.Collections.Generic;
using UnityEngine;

public class FixedEnemyFactory : IEnemyFactory
{
    public int NOfStates { get; } = 1;

    public Enemy GenerateEnemy(Map map, List<Enemy> enemies)
    {
        Vector2Int position = EnemyFactoryUtility.GetRandomAvailablePosition(map, enemies, 0);

        // no positions available => enemy creation aborted => return null
        if (position.x == -1) return null;

        int rotation = EnemyFactoryUtility.GetRotation(map, position);
        int visionLength = EnemyFactoryUtility.GetVisionLength();

        EnemyState enemyState = new EnemyState
        {
            Position = position,
            Rotation = rotation,
            VisionLength = visionLength
        };

        enemyState.SurveilledTiles = EnemyFactoryUtility.GetSurveilledTiles(map, enemyState);

        List<EnemyState> pattern = new List<EnemyState> { enemyState };
        Enemy enemy = new Enemy(pattern);

        return enemy;
    }
}
