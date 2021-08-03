using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public string seed = "";
    public int minSize, maxSize;
    public Instantiator instantiator;

    private Map map;
    private List<Enemy> enemies;

    void Start() {
        if (!System.String.IsNullOrEmpty(seed))
            Random.state = JsonUtility.FromJson<Random.State>(seed);
        Debug.Log("Copy the following Random.State if you want to save this level:");
        Debug.Log(JsonUtility.ToJson(Random.state));
        Debug.Log("Then, paste it in the seed property of Generator game object");

        GenerateLevel();
    }

    void GenerateLevel()
    {
        bool solvable, trivial;

        do
        {
            GenerateMap();
            GenerateEnemies();

            solvable = Verifier.VerifyLevel(map, enemies);
            //Debug.Log("LevelSolvable=" + solvable);

            trivial = Verifier.IsLevelTrivial(map, enemies);
            //Debug.Log("LevelTrivial=" + trivial);
        } while (!solvable || trivial);

        //Debug.Log("LevelSolvable=" + solvable);
        //Debug.Log("LevelTrivial=" + trivial);

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

        IEnemyFactory[] enemyFactories = new IEnemyFactory[]
        {
            new FixedEnemyFactory(),
            new Rotating90EnemyFactory(),
            new Rotating360EnemyFactory(),
        };

        //int factoryToUse = 0;
        //int nOfEnemies = 20;

        int nOfEnemies = Mathf.FloorToInt(map.M * map.N / 20f) + Random.Range(-map.N / 2, map.N / 2) + 1;

        int i = 0;
        while (i < nOfEnemies || Verifier.IsLevelTrivial(map, enemies))
        {
            int factoryToUse = Random.Range(0, enemyFactories.Length);

            Enemy enemy = enemyFactories[factoryToUse].GenerateEnemy(map, enemies);

            if (enemy == null)
            {
                Debug.Log("Enemy could not be created. Level may be trivial.");
                break;
            }

            enemies.Add(enemy);

            i++;
        }
    }
}
