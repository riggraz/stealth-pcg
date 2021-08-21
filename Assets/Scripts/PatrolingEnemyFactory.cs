using System.Collections.Generic;
using UnityEngine;

public class PatrolingEnemyFactory : IEnemyFactory
{
    public int NOfStates { get; } = 8;

    public Enemy GenerateEnemy(Map map, List<Enemy> enemies)
    {
        Vector2Int position1 = EnemyFactoryUtility.GetRandomAvailablePosition(map, enemies, 0);

        // no positions available => enemy creation aborted => return null
        if (position1.x == -1) return null;

        Vector2Int position2 = new Vector2Int(-1, -1);
        HashSet<Vector2Int> availablePositions = EnemyFactoryUtility.GetAvailablePositions(map, enemies, 0);
        foreach (Vector2Int position in availablePositions)
        {
            if (position1.y == 0 && position.y == 0) continue; // otherwise enemy will trapass player in start point 

            if (position1.x == position.x || position1.y == position.y)
                if ((position - position1).magnitude >= 2 && (position - position1).magnitude <= 6)
                {
                    position2 = position;
                    break;
                }
        }

        if (position2.x == -1) return null;

        int rotation;

        if (position1.x < position2.x) rotation = 270;
        else if (position1.x > position2.x) rotation = 90;
        else if (position1.y < position2.y) rotation = 180;
        else rotation = 0;

        int visionLength = EnemyFactoryUtility.GetVisionLength();

        List<EnemyState> pattern = new List<EnemyState>();

        EnemyState enemyState1 = new EnemyState()
        {
            Position = position1,
            Rotation = rotation,
            VisionLength = visionLength
        };
        enemyState1.SurveilledTiles = EnemyFactoryUtility.GetSurveilledTiles(map, enemyState1);
        pattern.Add(enemyState1);

        EnemyState enemyState2 = new EnemyState()
        {
            Position = (position1 + position2) / 2,
            Rotation = rotation,
            VisionLength = visionLength
        };
        enemyState2.SurveilledTiles = EnemyFactoryUtility.GetSurveilledTiles(map, enemyState2);
        pattern.Add(enemyState2);

        EnemyState enemyState34 = new EnemyState()
        {
            Position = position2,
            Rotation = rotation,
            VisionLength = visionLength
        };
        enemyState34.SurveilledTiles = EnemyFactoryUtility.GetSurveilledTiles(map, enemyState34);
        pattern.Add(enemyState34); pattern.Add(enemyState34);

        EnemyState enemyState5 = new EnemyState()
        {
            Position = position2,
            Rotation = (rotation + 180) % 360,
            VisionLength = visionLength
        };
        enemyState5.SurveilledTiles = EnemyFactoryUtility.GetSurveilledTiles(map, enemyState5);
        pattern.Add(enemyState5);

        EnemyState enemyState6 = new EnemyState()
        {
            Position = (position1 + position2) / 2,
            Rotation = (rotation + 180) % 360,
            VisionLength = visionLength
        };
        enemyState6.SurveilledTiles = EnemyFactoryUtility.GetSurveilledTiles(map, enemyState6);
        pattern.Add(enemyState6);

        EnemyState enemyState78 = new EnemyState()
        {
            Position = position1,
            Rotation = (rotation + 180) % 360,
            VisionLength = visionLength
        };
        enemyState78.SurveilledTiles = EnemyFactoryUtility.GetSurveilledTiles(map, enemyState78);
        pattern.Add(enemyState78); pattern.Add(enemyState78);

        EnemyFactoryUtility.RandomizePatternOrder(pattern);

        Enemy enemy = new Enemy(pattern);
        return enemy;
    }
}
