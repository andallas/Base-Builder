using UnityEngine;
using System.Collections.Generic;


namespace Pathfinding
{
    // TODO: We need to make this class more generic and use as a base class, or turn this into an IGraph or
    //       something that makes sense once this class is complete.

    // This class constructs a simple path-finding graph of our world. Each WALKABLE neighbor from a Node<T> is
    // linked via an Edge<T> connection.
    public class Graph
    {
        public Dictionary<Tile, Node<Tile>> Nodes { get; protected set; }


        public Graph(World world)
        {
            Nodes = new Dictionary<Tile, Node<Tile>>();

            // Create nodes
            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    Tile tile = world.GetTileAt(x, y);

                    Nodes.Add(tile, new Node<Tile>(tile));
                    //if (tile.MovementCost > 0)
                    //{
                    //    Nodes.Add(tile, new Node<Tile>(tile));
                    //}
                }
            }


            // Create edges
            foreach (Tile tile in Nodes.Keys)
            {
                Node<Tile> node = Nodes[tile];

                List<Edge<Tile>> edges = new List<Edge<Tile>>();

                Tile[] neighbors = tile.GetNeighbors(true, false);

                for (int i = 0; i < neighbors.Length; i++)
                {
                    Tile neighbor = neighbors[i];
                    if (neighbor != null && neighbor.MovementCost > 0)
                    {
                        if (IsClippingCorner(tile, neighbor))
                        {
                            continue;
                        }

                        Edge<Tile> edge = new Edge<Tile>(neighbor.MovementCost, Nodes[neighbor]);
                        edges.Add(edge);
                    }
                }

                node.SetEdges(edges.ToArray());
            }
        }

        private bool IsClippingCorner(Tile current, Tile neighbor)
        {
            int dX = current.X - neighbor.X;
            int dY = current.Y - neighbor.Y;

            return  Mathf.Abs(dX) + Mathf.Abs(dY) == 2 &&
                    (WorldController.WorldData.GetTileAt(current.X - dX, current.Y).MovementCost == 0 ||
                    WorldController.WorldData.GetTileAt(current.X, current.Y - dY).MovementCost == 0);
        }
    }
}
