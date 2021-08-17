using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class EnemyFactoryUtility
{
    public static float minDistanceFromStartEndPoints = 3f;
    public static float minDistanceFromEnemies = 2f;

    static int[] rotations = new int[8] { 0, 45, 90, 135, 180, 225, 270, 315 };

    /* Returns available positions at state stateIndex given the list of enemies */
    public static HashSet<Vector2Int> GetAvailablePositions(Map map, List<Enemy> enemies, int stateIndex)
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
                    if ((point - e.Pattern[stateIndex % e.Pattern.Count].Position).magnitude < minDistanceFromEnemies)
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
        if (availablePositions.Count == 0) return null;

        return availablePositions;
    }

    /* Returns an hash set of all positions that are available in every state */
    public static HashSet<Vector2Int> GetAlwaysAvailablePositions(Map map, List<Enemy> enemies)
    {
        int nOfStates = 1;
        foreach (Enemy e in enemies) nOfStates = (e.Pattern.Count > nOfStates) ? e.Pattern.Count : nOfStates;

        HashSet<Vector2Int>[] availablePositions = new HashSet<Vector2Int>[nOfStates];

        for (int i = 0; i < nOfStates; i++)
        {
            availablePositions[i] = GetAvailablePositions(map, enemies, i);
            if (availablePositions[i] == null) return null;
        }

        for (int i = 1; i < nOfStates; i++)
            availablePositions[0].IntersectWith(availablePositions[i]);

        return availablePositions[0];
    }

    public static Vector2Int GetRandomAvailablePosition(Map map, List<Enemy> enemies, int stateIndex)
    {
        HashSet<Vector2Int> availablePositions = GetAvailablePositions(map, enemies, stateIndex);

        if (availablePositions == null) return new Vector2Int(-1, -1);

        return availablePositions.ElementAt<Vector2Int>(Random.Range(0, availablePositions.Count));
    }

    public static Vector2Int GetRandomAlwaysAvailablePosition(Map map, List<Enemy> enemies)
    {
        HashSet<Vector2Int> availablePositions = GetAlwaysAvailablePositions(map, enemies);

        if (availablePositions == null) return new Vector2Int(-1, -1);

        return availablePositions.ElementAt<Vector2Int>(Random.Range(0, availablePositions.Count));
    }

    public static Vector2Int GetAvailablePositionNearCenter(Map map, List<Enemy> enemies, int stateIndex)
    {
        HashSet<Vector2Int> availablePositions = new HashSet<Vector2Int>();

        if (stateIndex == -1)
            availablePositions = GetAlwaysAvailablePositions(map, enemies);
        else
            availablePositions = GetAvailablePositions(map, enemies, stateIndex);

        if (availablePositions == null) return new Vector2Int(-1, -1);

        HashSet<Vector2Int> positionsToRemove = new HashSet<Vector2Int>();

        foreach (Vector2Int position in availablePositions)
        {
            int distanceFromCenterX = Math.Abs(position.x - map.N / 2);
            int distanceFromCenterY = Math.Abs(position.y - map.M / 2);

            int maxDistanceFromCenterX =  Mathf.FloorToInt(Mathf.Sqrt(map.N));
            int maxDistanceFromCenterY = Mathf.FloorToInt(Mathf.Sqrt(map.M)); ;

            if (distanceFromCenterX > maxDistanceFromCenterX || distanceFromCenterY > maxDistanceFromCenterY)
                positionsToRemove.Add(position);
        }

        availablePositions.ExceptWith(positionsToRemove);

        if (availablePositions.Count <= 0) return new Vector2Int(-1, -1);

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

    /*
     * Given the pattern, find a cut point and invert the two subpatterns
     * Used to create more diversity
     */
    public static void RandomizePatternOrder(List<EnemyState> pattern)
    {
        int nOfStates = pattern.Count;
        int cutPoint = Random.Range(0, nOfStates);

        List<EnemyState> cuttedRange = pattern.GetRange(cutPoint, nOfStates - cutPoint);
        pattern.RemoveRange(cutPoint, nOfStates - cutPoint);
        pattern.InsertRange(0, cuttedRange);
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

        //Debug.Log("SURVEILLED TILES");
        //for (int i = 0; i < surveilledTiles.Count; i++) Debug.Log(surveilledTiles.ToList()[i]);

        return surveilledTiles;
    }
}
