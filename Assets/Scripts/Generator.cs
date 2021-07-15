using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public int minSize, maxSize;
    public Instantiator instantiator;

    private Map map;
    private List<Enemy> enemies;

    void Start() { GenerateLevel(); }

    void GenerateLevel()
    {
        GenerateMap();
        GenerateEnemies();

        instantiator.InstantiateLevel(map, enemies);
    }

    // Randomly draws M and N such that they are in range [minSize, maxSize] and M >= N
    // Randomly draws x-coordinate of start and end points
    // (their z-coordinate is always 0 and M-1 respectively)
    void GenerateMap()
    {
        map = new Map();

        int maxDiffBetweenDims = 5; // max difference in size between M and N

        int N = Random.Range(minSize, maxSize + 1);
        int M = N + Random.Range(0, maxDiffBetweenDims + 1);
        M = Mathf.Clamp(M, N, maxSize);

        int startPointX = Random.Range(0, N);
        int endPointX = Random.Range(0, N);

        map.M = M;
        map.N = N;
        map.StartPoint = new Vector2Int(startPointX, 0);
        map.EndPoint = new Vector2Int(endPointX, map.M - 1);

        Debug.Log("Map: M=" + map.M + ", N=" + map.N + ", start=" + map.StartPoint + ", end=" + map.EndPoint);
    }

    void GenerateEnemies()
    {
        enemies = new List<Enemy>();

        FixedEnemyFactory fixedEnemyFactory = new FixedEnemyFactory();

        int nOfEnemies = Mathf.FloorToInt(map.M * map.N / 50f) + 1;
        //int nOfEnemies = 1;
        Debug.Log("# of enemies = " + nOfEnemies);

        for (int i = 0; i < nOfEnemies; i++)
        {
            Enemy enemy = fixedEnemyFactory.GenerateEnemy(map, enemies);
            if (enemy == null) Debug.Log("Enemy could not be created.");
            else enemies.Add(enemy);
        }
    }
}
