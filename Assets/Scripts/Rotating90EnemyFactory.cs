using System.Collections.Generic;
using UnityEngine;

public class Rotating90EnemyFactory : IEnemyFactory
{
    public int NOfStates { get; } = 4;

    public Enemy GenerateEnemy(Map map, List<Enemy> enemies)
    {
        Vector2Int position = EnemyFactoryUtility.GetRandomAlwaysAvailablePosition(map, enemies);

        // no positions available => enemy creation aborted => return null
        if (position.x == -1) return null;

        int[] rotations = EnemyFactoryUtility.Get2SimilarRotations(map, position);
        int visionLength = EnemyFactoryUtility.GetVisionLength();

        List<EnemyState> pattern = new List<EnemyState>();

        for (int i = 0; i < 2; i++)
        {
            EnemyState enemyState = new EnemyState()
            {
                Position = position,
                Rotation = rotations[i],
                VisionLength = visionLength
            };
            enemyState.SurveilledTiles = EnemyFactoryUtility.GetSurveilledTiles(map, enemyState);

            // add two times the same state, so the enemy stays in place for a while
            pattern.Add(enemyState);
            pattern.Add(enemyState);
        }

        EnemyFactoryUtility.RandomizePatternOrder(pattern);

        Enemy enemy = new Enemy(pattern);
        return enemy;
    }
}
