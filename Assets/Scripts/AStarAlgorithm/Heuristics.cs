using UnityEngine;

public static class Heuristics
{
    public static float EuclideanEstimator(Node from, Node to)
    {
        return (from.Description - to.Description).magnitude;
    }

    public static float DiagonalEstimator(Node from, Node to)
    {
        float D = 1f;
        float D2 = Mathf.Sqrt(2);
        float dx = Mathf.Abs(from.Description.x - to.Description.x);
        float dy = Mathf.Abs(from.Description.y - to.Description.y);

        return D * (dx + dy) + (D2 - 2 * D) * Mathf.Min(dx, dy);
    }

    public static float ManhattanEstimator(Node from, Node to)
    {
        return (
            Mathf.Abs(from.Description.x - to.Description.x) +
            Mathf.Abs(from.Description.y - to.Description.y)
            );
    }

    public static float ZeroEstimator(Node from, Node to)
    {
        return 0f;
    }
}
