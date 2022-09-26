using System;

[Serializable]
public struct Edge
{
    public Edge(ushort indexNodeA, ushort indexNodeB)
    {
        a = indexNodeA;
        b = indexNodeB;
    }
    public ushort a;
    public ushort b;

    public override bool Equals(object obj)
    {
        Edge edge = (Edge)obj;
        return (edge.a == a && edge.b == b) || (edge.b == a && edge.a == b);
    }
    public override int GetHashCode()
    {
        return a + b;
    }
}
