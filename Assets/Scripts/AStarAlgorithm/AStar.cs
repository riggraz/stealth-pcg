using System.Collections.Generic;

public delegate float HeuristicFunction(Node from, Node to);

public static class AStar
{
    public static bool immediateStop = true;

    public static List<Node> visited;
    public static List<Node> unvisited;

    private struct NodeExtension
    {
        public float distance;
        public float estimate;
        public Edge predecessor;
    }

    static Dictionary<Node, NodeExtension> status;

    public static Edge[] Solve(Graph g, Node start, Node goal, HeuristicFunction heuristic)
    {
        visited = new List<Node>();
        unvisited = new List<Node>(g.GetNodes());

        if (!unvisited.Contains(start) || !unvisited.Contains(goal)) return null;

        // Set tentative distance for all nodes
        status = new Dictionary<Node, NodeExtension>();
        foreach(Node n in unvisited)
        {
            status[n] = new NodeExtension
            {
                distance = (n.Equals(start) ? 0f : float.MaxValue),
                estimate = (n.Equals(start) ? heuristic(start, goal) : float.MaxValue)
            };
        }

        //NodeExtension ne1 = status[start];
        //NodeExtension ne2 = status[goal];

        // Iterate until optimal path is found
        while (!IsSearchCompleted(goal, unvisited))
        {
            // Select current node
            Node current = GetNextNode();

            if (status[current].distance == float.MaxValue) break;

            // Update info of all neighbors of current node
            foreach (Edge e in g.GetEdges(current))
            {
                if (status[current].distance + e.Weight < status[e.To].distance)
                {
                    status[e.To] = new NodeExtension()
                    {
                        distance = status[current].distance + e.Weight,
                        estimate = heuristic(e.To, goal),
                        predecessor = e
                    };

                    // it may happen that e.To is already in visited nodes
                    // remove it from there, and add it to unvisited nodes
                    if (visited.Contains(e.To))
                    {
                        visited.Remove(e.To);
                        unvisited.Add(e.To);
                    }
                }
            }

            visited.Add(current);
            unvisited.Remove(current);
        }

        // If goal unreachable
        if (status[goal].distance == float.MaxValue) return null;

        // Search completed, now return the optimal path found
        List<Edge> result = new List<Edge>();
        Node walker = goal;

        while (walker != start)
        {
            result.Add(status[walker].predecessor);
            walker = status[walker].predecessor.From;
        }

        result.Reverse();

        return result.ToArray();
    }

    // Returns the node in the unvisited set with lowest estimated distance to goal
    private static Node GetNextNode()
    {
        Node candidate = null;

        foreach (Node n in unvisited)
            if (candidate == null || status[n].estimate < status[candidate].estimate)
                candidate = n;

        return candidate;
    }

    private static bool IsSearchCompleted(Node goal, List<Node> unvisitedNodes)
    {
        if (status[goal].distance == float.MaxValue) return false;

        if (immediateStop) return true;

        foreach (Node n in unvisitedNodes)
            if (status[goal].distance > status[n].distance) return false;

        return true;
    }
}
