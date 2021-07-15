using UnityEngine;

public class Map
{
    public int M { get; set; } // number of rows
    public int N { get; set; } // number of cols

    public Vector2Int StartPoint { get; set; } = new Vector2Int();
    public Vector2Int EndPoint { get; set; } = new Vector2Int();
}
