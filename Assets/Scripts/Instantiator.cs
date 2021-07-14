using UnityEngine;

public class Instantiator : MonoBehaviour
{
    public GameObject tile;

    private int M, N;

    public void InstantiateLevel(int M, int N)
    {
        this.M = M;
        this.N = N;

        InstantiateMap();
    }

    // Instantiate an MxN map + a border of thickness 1 around it
    void InstantiateMap()
    {
        Debug.Log("Instantiating map with size M=" + M + ", N=" + N);

        GameObject map = new GameObject("Map");

        for (int i = -1; i <= M; i++)
        {
            for (int j = -1; j <= N; j++)
            {
                Instantiate(tile, new Vector3Int(j, 0, i), Quaternion.identity, map.transform);

                if (i == -1 || j == -1 || i == M || j == N)
                    Instantiate(tile, new Vector3Int(j, 1, i), Quaternion.identity, map.transform);
            }
        }
    }
}
