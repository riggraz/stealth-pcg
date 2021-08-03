using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class EnemyFactoryUtility
{
    public static float minDistanceFromStartEndPoints = 4f;
    public static float minDistanceFromEnemies = 3f;

    static int[] rotations = new int[8] { 0, 45, 90, 135, 180, 225, 270, 315 };

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

        // If trivialPositions is not null that means that we've run the Verifier for triviality
        // i.e. we want to make the level non trivial
        if (Verifier.trivialPositions != null)
        {
            availablePositions.IntersectWith(Verifier.trivialPositions);
        }

        //Debug.Log("Available positions = " + availablePositions.Count);
        if (availablePositions.Count == 0) return new Vector2Int(-1, -1);

        return availablePositions.ElementAt<Vector2Int>(Random.Range(0, availablePositions.Count));
    }

    // Returns the most natural rotation given the position of the Enemy
    public static int GetRotation(Map map, Vector2Int position)
    {
        float[] weights = GetRotationWeights(map, position);

        int rotationIndex = weights.ToList<float>().IndexOf(Mathf.Max(weights));

        return rotations[rotationIndex];
    }

    // Returns the most natural rotation + the 2nd most natural (+/- 90° from the first)
    public static int[] Get2SimilarRotations(Map map, Vector2Int position)
    {
        float[] weights = GetRotationWeights(map, position);

        int rotation1Index = weights.ToList<float>().IndexOf(Mathf.Max(weights));
        int rotation1 = rotations[rotation1Index];

        weights[rotation1Index] = 0f;

        int rotation2Index = weights.ToList<float>().IndexOf(Mathf.Max(weights));
        int rotation2 = rotations[rotation2Index];

        return new int[] { rotation1, rotation2 };
    }

    // Assigns a weight to each rotation. The weight is directly proportional to
    // how natural that rotation is given the Enemy position and map layout
    private static float[] GetRotationWeights(Map map, Vector2Int position)
    {
        float[] weights = new float[8];

        float x = position.x / (float)map.N;
        float y = position.y / (float)map.M;

        weights[0] = y;
        weights[1] = (x + y) / 1.75f; // should be 2f
        weights[2] = x;
        weights[3] = (1 + x - y) / 1.75f; // should be 2f
        weights[4] = 1 - weights[0];
        weights[5] = 1 - weights[1];
        weights[6] = 1 - weights[2];
        weights[7] = 1 - weights[3];

        // normalize
        for (int i = 0; i < 8; i++) weights[i] /= 4f;

        return weights;
    }

    // Returns a random vision length
    public static int GetVisionLength()
    {
        return Random.Range(2, Mathf.CeilToInt(minDistanceFromStartEndPoints));
    }

    // Given the map and an EnemyState (position+rotation+visionLength) returns
    // a set containing all points of the map occupied by this EnemyState
    public static HashSet<Vector2Int> GetSurveilledTiles(Map map, EnemyState e)
    {
        HashSet<Vector2Int> surveilledTiles = new HashSet<Vector2Int>();

        surveilledTiles.Add(e.Position);

        int diagonalVisionLength = Mathf.CeilToInt(0.70711f * e.VisionLength);

        if (e.Rotation == 0)
            for (int i = 1; i <= e.VisionLength; i++)
                surveilledTiles.Add(new Vector2Int(e.Position.x, e.Position.y - i));
        else if (e.Rotation == 90)
            for (int i = 1; i <= e.VisionLength; i++)
                surveilledTiles.Add(new Vector2Int(e.Position.x - i, e.Position.y));
        else if (e.Rotation == 180)
            for (int i = 1; i <= e.VisionLength; i++)
                surveilledTiles.Add(new Vector2Int(e.Position.x, e.Position.y + i));
        else if (e.Rotation == 270)
            for (int i = 1; i <= e.VisionLength; i++)
                surveilledTiles.Add(new Vector2Int(e.Position.x + i, e.Position.y));
        else if (e.Rotation == 45)
            for (int i = 1; i <= diagonalVisionLength; i++)
                surveilledTiles.Add(new Vector2Int(e.Position.x - i, e.Position.y - i));
        else if (e.Rotation == 135)
            for (int i = 1; i <= diagonalVisionLength; i++)
                surveilledTiles.Add(new Vector2Int(e.Position.x - i, e.Position.y + i));
        else if (e.Rotation == 225)
            for (int i = 1; i <= diagonalVisionLength; i++)
                surveilledTiles.Add(new Vector2Int(e.Position.x + i, e.Position.y + i));
        else if (e.Rotation == 315)
            for (int i = 1; i <= diagonalVisionLength; i++)
                surveilledTiles.Add(new Vector2Int(e.Position.x + i, e.Position.y - i));

        Debug.Log("SURVEILLED TILES");
        for (int i = 0; i < surveilledTiles.Count; i++) Debug.Log(surveilledTiles.ToList()[i]);

        return surveilledTiles;
    }
}
