using System.Collections.Generic;
using UnityEngine;

public class Rotating90EnemyFactory : IEnemyFactory
{
    public int NOfStates { get; } = 4;

    public Enemy GenerateEnemy(Map map, List<Enemy> enemies)
    {
        Vector2Int position = EnemyFactoryUtility.GetRandomAvailablePosition(map, enemies, 0);

        // no positions available => enemy creation aborted => return null
        if (position.x == -1) return null;

        int[] rotations = EnemyFactoryUtility.Get2SimilarRotations(map, position);
        int visionLength = EnemyFactoryUtility.GetVisionLength();

        List<EnemyState> pattern = new List<EnemyState>();

        for (int i = 0; i < 4; i++)
        {
            pattern.Add(new EnemyState()
            {
                Position = position,
                Rotation = rotations[i / 2],
                VisionLength = visionLength
            });
        }

        Enemy enemy = new Enemy(pattern);
        return enemy;
    }
}
