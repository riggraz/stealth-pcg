using UnityEngine;

public class Generator : MonoBehaviour
{
    public int minSize, maxSize;
    public Instantiator instantiator;

    private int M; // rows
    private int N; // columns

    void Start() { GenerateLevel(); }

    void GenerateLevel()
    {
        DrawLevelSize();

        instantiator.InstantiateLevel(M, N);
    }

    // Draw M and N such that they are in range [minSize, maxSize] and M >= N
    void DrawLevelSize()
    {
        int maxDiffBetweenDims = 5; // max difference in size between M and N

        N = Random.Range(minSize, maxSize + 1);
        M = N + Random.Range(0, maxDiffBetweenDims + 1);
        M = Mathf.Clamp(M, N, maxSize);
    }
}
