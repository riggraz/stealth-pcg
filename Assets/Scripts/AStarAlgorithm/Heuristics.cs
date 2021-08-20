using System;
public static class Heuristics
{
    public static float EuclideanEstimator(Node from, Node to)
    {
        return (from.Description - to.Description).magnitude;
    }
}
