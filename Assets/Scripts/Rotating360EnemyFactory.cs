using System.Collections.Generic;
using UnityEngine;

public class Rotating360EnemyFactory : IEnemyFactory
{
    public int NOfStates { get; } = 8;

    public Enemy GenerateEnemy(Map map, List<Enemy> enemies)
    {
        Vector2Int position = EnemyFactoryUtility.GetAvailablePositionNearCenter(map, enemies, 0);

        // no positions available => enemy creation aborted => return null
        if (position.x == -1) return null;

        int startRotation = EnemyFactoryUtility.GetRotation(map, position);
        int visionLength = EnemyFactoryUtility.GetVisionLength();

        List<EnemyState> pattern = new List<EnemyState>();

        for (int i = 0; i < NOfStates; i++)
        {
            EnemyState enemyState = new EnemyState()
            {
                Position = position,
                Rotation = startRotation,
                VisionLength = visionLength
            };
            enemyState.SurveilledTiles = EnemyFactoryUtility.GetSurveilledTiles(map, enemyState);

            startRotation = (startRotation + 45) % 360;

            // add two times the same state, so the enemy stays in place for a while
            pattern.Add(enemyState);
            //pattern.Add(enemyState);
        }

        Enemy enemy = new Enemy(pattern);
        return enemy;
    }
}
