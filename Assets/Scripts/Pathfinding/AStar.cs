using Priority_Queue;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Pathfinding
{
    public class AStar
    {
        private Queue<Tile> path;


        public AStar(World world, Tile tileStart, Tile tileEnd)
        {
            Dictionary<Tile, Node<Tile>> nodes = world.TileGraph.Nodes;

            if (!nodes.ContainsKey(tileStart))
            {
                Debug.LogError("AStar: The starting tile isn't in the list of nodes!");
                return;
            }

            if (!nodes.ContainsKey(tileEnd))
            {
                Debug.LogError("AStar: The ending tile isn't in the list of nodes!");
                return;
            }

            Node<Tile> start = nodes[tileStart];
            Node<Tile> goal = nodes[tileEnd];

            var ClosedSet = new List<Node<Tile>>();

            var OpenSet = new SimplePriorityQueue<Node<Tile>>();
            OpenSet.Enqueue(start, 0);

            var CameFrom = new Dictionary<Node<Tile>, Node<Tile>>();

            var g_score = new Dictionary<Node<Tile>, float>();
            var f_score = new Dictionary<Node<Tile>, float>();

            foreach (Node<Tile> node in nodes.Values)
            {
                g_score[node] = Mathf.Infinity;
                f_score[node] = Mathf.Infinity;
            }

            g_score[start] = 0;
            f_score[goal] = HeuristictCostEstimate(start, goal);

            while (OpenSet.Count > 0)
            {
                Node<Tile> current = OpenSet.Dequeue();

                if (current == goal)
                {
                    reconstruct_path(CameFrom, goal);
                    return;
                }

                ClosedSet.Add(current);

                foreach (Edge<Tile> edgeNeighbor in current.Edges)
                {
                    Node<Tile> neighbor = edgeNeighbor.Node;

                    if (ClosedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    float movementCostToNeighbor = neighbor.Data.MovementCost * DistanceBetween(current, neighbor);

                    float tentative_g_score = g_score[current] + movementCostToNeighbor;

                    if (OpenSet.Contains(neighbor) && tentative_g_score >= g_score[neighbor])
                    {
                        continue;
                    }

                    CameFrom[neighbor] = current;
                    g_score[neighbor] = tentative_g_score;
                    f_score[neighbor] = g_score[neighbor] + HeuristictCostEstimate(neighbor, goal);

                    if (!OpenSet.Contains(neighbor))
                    {
                        OpenSet.Enqueue(neighbor, f_score[neighbor]);
                    }
                    else
                    {
                        OpenSet.UpdatePriority(neighbor, f_score[neighbor]);
                    }
                }
            }

            // TODO: Handle this failure state, this.path will be null in this case.
            return;
        }

        private float HeuristictCostEstimate(Node<Tile> a, Node<Tile> b)
        {
            return DistanceBetween(a, b);
        }

        private float DistanceBetween(Node<Tile> a, Node<Tile> b)
        {
            if ((Mathf.Abs(a.Data.X - b.Data.X) + Mathf.Abs(a.Data.Y - b.Data.Y) == 1))
            {
                return 1f;
            }

            if (((Mathf.Abs(a.Data.X - b.Data.X) == 1 && Mathf.Abs(a.Data.Y - b.Data.Y) == 1)))
            {
                return 1.41421356237f;
            }

            //return  ((a.Data.X - b.Data.X) * (a.Data.X - b.Data.X)) + ((a.Data.Y - b.Data.Y) * (a.Data.Y - b.Data.Y));

            return Mathf.Sqrt(Mathf.Pow(a.Data.X - b.Data.X, 2) + Mathf.Pow(a.Data.Y - b.Data.Y, 2));
        }

        private void reconstruct_path(Dictionary<Node<Tile>, Node<Tile>> cameFrom, Node<Tile> goal)
        {
            var total_path = new Queue<Tile>();
            total_path.Enqueue(goal.Data);

            while (cameFrom.ContainsKey(goal))
            {
                goal = cameFrom[goal];
                total_path.Enqueue(goal.Data);
            }

            path = new Queue<Tile>(total_path.Reverse());
        }


        public Tile Dequeue()
        {
            return path.Dequeue();
        }

        public int Length
        {
            get { return (path == null) ? 0 : path.Count; }
        }
    }
}