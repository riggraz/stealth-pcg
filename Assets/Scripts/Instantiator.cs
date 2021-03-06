using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instantiator : MonoBehaviour
{
    public GameObject tileGameObject, wallGameObject;
    public GameObject startPointGameObject, endPointGameObject;
    public GameObject enemyGameObject;
    public GameObject player;
    public Text levelStatusText;
    public Orchestrator orchestrator;

    private Map map;
    private List<Enemy> enemies;

    private GameObject mapGameObject;
    private GameObject playerGameObject;
    private List<GameObject> enemiesGameObjects;

    public void InstantiateLevel(Map map, List<Enemy> enemies)
    {
        this.map = map;
        this.enemies = enemies;
        this.enemiesGameObjects = new List<GameObject>();

        InstantiateMap();
        InstantiateEnemies();
        InstantiatePlayer();

        orchestrator.StartOrchestrating(enemies, enemiesGameObjects);

        levelStatusText.text = "Playing...";
    }

    public void DestroyLevel()
    {
        Destroy(mapGameObject);
        Destroy(playerGameObject);
        foreach (GameObject enemyGO in enemiesGameObjects) Destroy(enemyGO);
    }

    // Instantiates an MxN map and a border of thickness 1 around it
    // Also places start and end points
    void InstantiateMap()
    {
        mapGameObject = new GameObject("Map");

        for (int i = -1; i <= map.M; i++)
        {
            for (int j = -1; j <= map.N; j++)
            {
                Instantiate(tileGameObject, new Vector3Int(j, 0, i), Quaternion.identity, mapGameObject.transform);

                if (i == -1 || j == -1 || i == map.M || j == map.N)
                    Instantiate(wallGameObject, new Vector3Int(j, 1, i), Quaternion.identity, mapGameObject.transform);
            }
        }

        Instantiate(startPointGameObject, To3DVect(map.StartPoint, 1), Quaternion.identity, mapGameObject.transform);
        Instantiate(endPointGameObject, To3DVect(map.EndPoint, 1), Quaternion.identity, mapGameObject.transform);
    }

    // Instantiates the whole list of enemies
    void InstantiateEnemies()
    {
        foreach(Enemy e in enemies)
        {
            GameObject enemy = Instantiate(
                enemyGameObject,
                To3DVect(e.Pattern[0].Position, 1),
                Quaternion.AngleAxis(e.Pattern[0].Rotation, Vector3.up)
            );

            // Adjust vision range of enemy
            Transform enemyVision = enemy.transform.GetChild(0);
            float visionLengthWorld = e.Pattern[0].VisionLength / enemy.transform.localScale.z;
            enemyVision.localScale = new Vector3(0.75f, 0.75f, visionLengthWorld);
            enemyVision.localPosition = new Vector3(0, 0, -0.5f - visionLengthWorld / 2f);

            enemiesGameObjects.Add(enemy);
        }
    }

    void InstantiatePlayer()
    {
        playerGameObject = Instantiate(player, To3DVect(map.StartPoint, 1), Quaternion.identity);
        playerGameObject.GetComponent<PlayerController>().SetMap(map);
    }

    // Converts the internal 2D representation of points/vectors to the 3D representation
    private Vector3Int To3DVect(Vector2Int v, int thirdDim = 0)
    {
        return new Vector3Int(v.x, thirdDim, v.y);
    }
}
