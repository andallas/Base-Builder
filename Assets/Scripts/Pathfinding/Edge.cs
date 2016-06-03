

namespace Pathfinding
{
    public class Edge<T>
    {
        // Cost to LEAVE T is fine/normal
        // Cost to ENTER T is higher
        public float Cost { get; protected set; }

        public Node<T> Node { get; protected set; }


        public Edge(float cost, Node<T> node)
        {
            Cost = cost;
            Node = node;
        }
    }
}
