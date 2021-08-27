using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public string seed = "";
    private int minSize, maxSize;
    [Range(1, 10)]
    public int difficulty = 3;

    public Instantiator instantiator;
    public GameController gameController;

    private Map map;
    private List<Enemy> enemies;

    private void Start() {
        GenerateLevel();
    }

    // Generate a new level until it is both solvable and not trivial
    public void GenerateLevel()
    {
        InitializeRandomState();

        bool solvable, trivial;

        do
        {
            minSize = difficulty + 2;
            maxSize = minSize + 3;

            GenerateMap();
            GenerateEnemies();

            solvable = Verifier.IsLevelSolvable(map, enemies);
            trivial = Verifier.IsLevelTrivial(map, enemies);
        } while (!solvable || trivial);

        PrintLevelInfo();

        instantiator.InstantiateLevel(map, enemies);
    }

    // Randomly draws M and N such that they are in range [minSize, maxSize] and M >= N
    // Randomly draws x-coordinate of start and end points
    // (their z-coordinate is always 0 and M-1 respectively)
    private void GenerateMap()
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
    }

    // Tries to generate nOfEnemies enemies and make the level non trivial
    private void GenerateEnemies()
    {
        enemies = new List<Enemy>();

        IEnemyFactory[] enemyFactories = new IEnemyFactory[]
        {
            new FixedEnemyFactory(),
            new Rotating90EnemyFactory(),
            new Rotating360EnemyFactory(),
            new PatrolingEnemyFactory(),
        };

        int nOfEnemies = Mathf.FloorToInt(map.M * map.N / 15f) + Random.Range(-map.N / 4, map.N / 4 + 1) + 1;
        nOfEnemies = Mathf.Clamp(nOfEnemies, difficulty - 1, difficulty * 2);

        int i = 0;
        int enemyAddFailures = 0;
        while (i < nOfEnemies || Verifier.IsLevelTrivial(map, enemies))
        {
            int factoryToUse = Random.Range(0, enemyFactories.Length);

            Enemy enemy = enemyFactories[factoryToUse].GenerateEnemy(map, enemies);

            if (enemy == null)
            {
                enemyAddFailures++;
                if (enemyAddFailures > 10) break;
                continue;
            }

            enemies.Add(enemy);
            enemyAddFailures = 0;

            i++;
        }
    }

    // Initialize with supplied seed (Random.state), if provided
    private void InitializeRandomState()
    {
        if (!string.IsNullOrEmpty(seed))
            Random.state = JsonUtility.FromJson<Random.State>(seed);

        gameController.SetRandomState(JsonUtility.ToJson(Random.state));
    }

    // Print info about the level (map dimensions, enemies, ...)
    private void PrintLevelInfo()
    {
        Debug.Log("<color=green>[Map information]</color>");
        Debug.Log("M=" + map.M + ", N=" + map.N + "(" + map.M + "x" + map.N + ")");
        Debug.Log("startPoint=" + map.StartPoint + ", endPoint=" + map.EndPoint);

        Debug.Log("<color=green>[Enemies information]</color>");
        Debug.Log(enemies.Count + " enemies");
        foreach (Enemy e in enemies)
            Debug.Log(e);

        Debug.Log("<color=green>[Seed information]</color>");
        Debug.Log("Copy the following Random.state:");
        Debug.Log(gameController.GetRandomState());
        Debug.Log("Then paste it in the 'seed' property of Generator " +
            "game object to replay this level in the future");
    }
}
