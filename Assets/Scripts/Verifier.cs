using System.Collections.Generic;
using UnityEngine;

public static class Verifier
{
    public static int MAX_ITERATION_COUNT = 100;

    public static Map map;

    public static bool VerifyLevel(Map map, List<Enemy> enemies)
    {
        Verifier.map = map;

        int currentState = 0;
        int nOfStates = 0;

        foreach (Enemy e in enemies)
            nOfStates = (e.Pattern.Count > nOfStates) ? e.Pattern.Count : nOfStates;

        HashSet<Vector2Int>[] surveilledTiles = new HashSet<Vector2Int>[nOfStates];
        for (int i = 0; i < surveilledTiles.Length; i++)
            surveilledTiles[i] = new HashSet<Vector2Int>();

        // A set of surveilled tiles by all enemies for each state
        for (int i = 0; i < nOfStates; i++)
            foreach (Enemy e in enemies)
                surveilledTiles[i].UnionWith(e.Pattern[i % e.Pattern.Count].SurveilledTiles);

        // A set containing all possible tiles the player may be occupying
        HashSet<Vector2Int> playerPositions = new HashSet<Vector2Int>();
        playerPositions.Add(map.StartPoint);

        int iterationCount = 0;

        while (!playerPositions.Contains(map.EndPoint))
        {
            iterationCount++;
            if (iterationCount > MAX_ITERATION_COUNT) break;

            currentState = (currentState + 1) % nOfStates;

            // Remove player from positions occupied by enemies
            playerPositions.ExceptWith(surveilledTiles[currentState]);

            // All possible movements of the player during 1 time step
            HashSet<Vector2Int> newPositions;
            HashSet<Vector2Int> cumulatedNewPositions = new HashSet<Vector2Int>();
            foreach (Vector2Int p in playerPositions)
            {
                newPositions = new HashSet<Vector2Int>();

                EvolvePlayer(p, 3, newPositions, surveilledTiles[currentState]);

                cumulatedNewPositions.UnionWith(newPositions);
            }

            playerPositions.UnionWith(cumulatedNewPositions);
        }

        if (playerPositions.Contains(map.EndPoint)) return true;
        else return false;
    }

    private static void EvolvePlayer(
        Vector2Int p,
        float walkableDistance,
        HashSet<Vector2Int> newPositions,
        HashSet<Vector2Int> surveilledTiles
        )
    {
        if (p.x < 0 || p.y < 0 || p.x >= map.N || p.y >= map.M) return;
        if (surveilledTiles.Contains(p)) return;

        newPositions.Add(p);

        if (Mathf.FloorToInt(walkableDistance) <= 0) return;

        EvolvePlayer(new Vector2Int(p.x, p.y - 1), walkableDistance - 1, newPositions, surveilledTiles);
        EvolvePlayer(new Vector2Int(p.x - 1, p.y), walkableDistance - 1, newPositions, surveilledTiles);
        EvolvePlayer(new Vector2Int(p.x, p.y + 1), walkableDistance - 1, newPositions, surveilledTiles);
        EvolvePlayer(new Vector2Int(p.x + 1, p.y), walkableDistance - 1, newPositions, surveilledTiles);
    }
}
