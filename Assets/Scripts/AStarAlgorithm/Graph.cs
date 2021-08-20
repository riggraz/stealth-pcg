using System.Collections.Generic;
using System.Linq;

public class Graph
{
    // given a Node as index, returns all outgoing Edges of that Node
    private Dictionary<Node, List<Edge>> data;

    public Graph()
    {
        data = new Dictionary<Node, List<Edge>>();
    }

    // Adds the provided Node to the Graph (if it doesn't exist yet)
    private void AddNode(Node n)
    {
        if (!data.ContainsKey(n)) data.Add(n, new List<Edge>());
    }

    // Adds the provided Edge to the Graph
    public void AddEdge(Edge e)
    {
        AddNode(e.From);
        AddNode(e.To);

        if (!data[e.From].Contains(e)) data[e.From].Add(e);
    }

    // Returns outgoing Edges from Node n
    public Edge[] GetEdges(Node n)
    {
        if (!data.ContainsKey(n)) return new Edge[0];
        return data[n].ToArray();
    }

    // Returns all Nodes of the Graph
    public Node[] GetNodes()
    {
        return data.Keys.ToArray();
    }

    public override string ToString()
    {
        string s = "[Graph]\n";

        s += GetNodes().Length + " nodes:\n";

        foreach (Node n in GetNodes()) {
            s += "[Node x=" + n.Description.x + ", y=" + n.Description.y + "]\n";
            s += GetEdges(n).Length + " edges:\n";

            foreach(Edge e in GetEdges(n))
                s += "[Edge from=" + e.From.Description + ", to=" + e.To.Description + ", weight=" + e.Weight + "]\n";
        }

        return s;
    }

}
