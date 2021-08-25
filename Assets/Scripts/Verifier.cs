using System.Collections.Generic;
using UnityEngine;

public static class Verifier
{
    public static int MAX_ITERATION_COUNT = 100;

    public static Map map;

    // Checks whether the level is solvable by simulating it in a discrete manner
    public static bool IsLevelSolvable(Map map, List<Enemy> enemies)
    {
        if (enemies.Count == 0) return false;

        Verifier.map = map;

        int currentState = 0;
        int nOfStates = EnemyFactoryUtility.GetNumberOfStates(enemies);

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

    // A bucket-fill like algorithm that simulates every possible movement
    // of the player during 1 time step
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

    public static HashSet<Vector2Int> trivialPositions;

    /*
     * Checks if there exist a path to the goal which
     * is free for each state (i.e. level is trivial)
    */
    public static bool IsLevelTrivial(Map map, List<Enemy> enemies)
    {
        if (enemies.Count == 0) return true;

        Verifier.map = map;

        HashSet<Vector2Int> surveilledTiles = new HashSet<Vector2Int>();

        int nOfStates = EnemyFactoryUtility.GetNumberOfStates(enemies);

        for (int i = 0; i < nOfStates; i++)
            foreach (Enemy e in enemies)
                surveilledTiles.UnionWith(e.Pattern[i % e.Pattern.Count].SurveilledTiles);

        // Create a Graph representation from the Map and surveilledTiles
        // This Graph representation will be used to run A*

        Graph graph = new Graph();
        Node startNode = null;
        Node goalNode = null;

        for (int i = 0; i < map.N; i++)
        {
            for (int j = 0; j < map.M; j++)
            {
                Node from = new Node() { Description = new Vector2Int(i, j) };

                if (from.Description.Equals(map.StartPoint)) startNode = from;
                if (from.Description.Equals(map.EndPoint)) goalNode = from;

                if (surveilledTiles.Contains(new Vector2Int(i, j))) continue;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Node to = new Node() { Description = new Vector2Int(i + x, j + y) };

                        if (surveilledTiles.Contains(to.Description) ||
                            to.Description.x < 0 || to.Description.x >= map.N ||
                            to.Description.y < 0 || to.Description.y >= map.M ||
                            from.Equals(to)
                            ) continue;

                        // An Edge is diagonal if both x and y are != 0
                        bool diagonalEdge = (Mathf.Abs(x) + Mathf.Abs(y) == 2);

                        Edge e = new Edge()
                        {
                            From = from,
                            To = to,
                            Weight = diagonalEdge ? Mathf.Sqrt(2) : 1f // more weight to diagonal edge
                        };

                        graph.AddEdge(e);
                    }
                }
            }
        }

        //Debug.Log(graph);

        Edge[] aStarResult = AStar.Solve(graph, startNode, goalNode, Heuristics.EuclideanEstimator);

        // No path available => level non trivial
        if (aStarResult == null) return false;

        // Otherwise, level is trivial => create expose the set of tiles that compose the trivial path
        trivialPositions = new HashSet<Vector2Int>();
        foreach (Edge e in aStarResult)
            trivialPositions.Add(e.From.Description);

        trivialPositions.Remove(aStarResult[0].From.Description);

        return true;
    }
}
