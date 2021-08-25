using UnityEngine;

public class Node
{
    public Vector2Int Description { get; set; }

    public override bool Equals(object obj)
    {
        Node nodeObj = obj as Node;

        if (nodeObj == null)
            return false;
        else
            return this.Description.Equals(nodeObj.Description);
    }

    public override int GetHashCode()
    {
        return Description.ToString().GetHashCode();
    }
}
