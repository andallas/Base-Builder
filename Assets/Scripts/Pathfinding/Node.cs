

namespace Pathfinding
{
    public class Node<T>
    {
        public T Data { get; protected set; }

        // TODO: Consider using an enum to determine directionality of edges.
        // Nodes leading OUT from this node
        public Edge<T>[] Edges { get; protected set; }


        public Node(T data)
        {
            Data = data;
        }


        public void SetEdges(Edge<T>[] edges)
        {
            // TODO: What kind of safety checking do we need to do here?
            Edges = edges;
        }
    }
}
