using System.Collections.Generic;
using UnityEngine;

public class Orchestrator : MonoBehaviour
{
    public float stateDuration;

    private bool orchestrating = false;

    private List<Enemy> enemies;
    private List<GameObject> enemiesGameObjects;

    private int nOfStates;
    private int currentState;

    private float t;

    void Start()
    {
        nOfStates = 0;
        currentState = 0;
        t = 0f;
    }

    void Update()
    {
        if (!orchestrating) return;

        t += Time.deltaTime;

        if (t >= stateDuration)
        {
            t -= stateDuration;
            currentState = (currentState + 1) % nOfStates;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            int enemyStateN = enemies[i].Pattern.Count;

            //enemiesGameObjects[i].transform.position = Vector2.Lerp(
            //    enemies[i].Pattern[currentState % enemyStateN].Position,
            //    enemies[i].Pattern[(currentState + 1) % enemyStateN].Position,
            //    t
            //);

            enemiesGameObjects[i].transform.rotation = Quaternion.AngleAxis(
                Mathf.LerpAngle(
                    enemies[i].Pattern[currentState % enemyStateN].Rotation,
                    enemies[i].Pattern[(currentState + 1) % enemyStateN].Rotation,
                    t / stateDuration
                ),
                Vector3.up);
        }
    }

    public void StartOrchestrating(List<Enemy> enemies, List<GameObject> enemiesGameObjects)
    {
        this.enemies = enemies;
        this.enemiesGameObjects = enemiesGameObjects;

        foreach (Enemy e in enemies)
            nOfStates = (e.Pattern.Count > nOfStates) ? e.Pattern.Count : nOfStates;

        orchestrating = true;
    }
}
