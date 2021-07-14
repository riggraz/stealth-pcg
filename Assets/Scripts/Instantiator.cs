using UnityEngine;

public class Instantiator : MonoBehaviour
{
    public GameObject tile;
    public GameObject startPoint, endPoint;

    private int M, N;
    private Vector3Int startPointPos, endPointPos;

    public void InstantiateLevel(int M, int N, int startPointX, int endPointX)
    {
        this.M = M;
        this.N = N;
        this.startPointPos = new Vector3Int(startPointX, 1, 0);
        this.endPointPos = new Vector3Int(endPointX, 1, M - 1);

        InstantiateMap();
    }

    // Instantiate an MxN map + a border of thickness 1 around it
    // Also places start and end points
    void InstantiateMap()
    {
        Debug.Log("Map: M=" + M + ", N=" + N + ", start=" + startPointPos + ", end=" + endPointPos);

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

        Instantiate(startPoint, startPointPos, Quaternion.identity);
        Instantiate(endPoint, endPointPos, Quaternion.identity);
    }
}
