using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemyFactoryUtility
{
    public static float minDistanceFromStartEndPoints = 6f;
    public static float minDistanceFromEnemies = 6f;

    public static Vector2Int GetRandomAvailablePosition(Map map, List<Enemy> enemies, int stateIndex)
    {
        HashSet<Vector2Int> availablePositions = new HashSet<Vector2Int>();

        for (int i = 0; i < map.M; i++)
        {
            for (int j = 0; j < map.N; j++)
            {
                Vector2Int point = new Vector2Int(j, i);
                bool skip = false;

                if ((point - map.StartPoint).magnitude < minDistanceFromStartEndPoints) continue;
                if ((point - map.EndPoint).magnitude < minDistanceFromStartEndPoints) continue;

                foreach (Enemy e in enemies)
                {
                    if ((point - e.Pattern[stateIndex].Position).magnitude < minDistanceFromEnemies)
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip) continue;

                availablePositions.Add(new Vector2Int(j, i));
            }
        }

        Debug.Log("Available positions = " + availablePositions.Count);

        if (availablePositions.Count == 0) return new Vector2Int(-1, -1);

        return availablePositions.ElementAt<Vector2Int>(Random.Range(0, availablePositions.Count));
    }

    public static int GetRotation(Map map, Vector2Int position)
    {
        float x = position.x / (float) map.N;
        float y = position.y / (float) map.M;

        int[] rotations = new int[8] { 0, 45, 90, 135, 180, 225, 270, 315 };
        float[] weights = new float[8];

        weights[0] = y;
        weights[1] = (x + y) / 1.75f; // should be 2f
        weights[2] = x;
        weights[3] = (1 + x - y) / 1.75f; // should be 2f
        weights[4] = 1 - weights[0];
        weights[5] = 1 - weights[1];
        weights[6] = 1 - weights[2];
        weights[7] = 1 - weights[3];

        for (int i = 0; i < 8; i++) weights[i] /= 4f;

        int rotationIndex = weights.ToList<float>().IndexOf(Mathf.Max(weights));

        //float sum = 0;
        //for (int i = 0; i < 8; i++) sum += weights[i];

        //Debug.Log(
        //    weights[0] + "," +
        //    weights[1] + "," +
        //    weights[2] + "," +
        //    weights[3] + "," +
        //    weights[4] + "," +
        //    weights[5] + "," +
        //    weights[6] + "," +
        //    weights[7] + "==" + sum
        //    );

        return rotations[rotationIndex];
    }

    public static int GetVisionLength()
    {
        return Random.Range(2, Mathf.CeilToInt(minDistanceFromStartEndPoints));
    }
}
