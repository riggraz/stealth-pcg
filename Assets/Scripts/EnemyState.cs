using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    public Vector2Int Position { get; set; }
    public int Rotation { get; set; }
    public int VisionLength { get; set; }

    public HashSet<Vector2Int> SurveilledTiles { get; set; }
}
