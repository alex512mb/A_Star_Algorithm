using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GraphGridBuilder : MonoBehaviour
{
    [SerializeField] float width;
    [SerializeField] float height;
    [SerializeField] ushort nodesX;
    [SerializeField] ushort nodesY;
    [SerializeField] GraphData graph;
    HashSet<Edge> set;

    const ushort one = 1;

    [ContextMenu("Generate")]
    void Generate()
    {
        graph.graph = Generate(width, height, nodesX, nodesY);
    }
    public Graph Generate(float _width, float _height, ushort _nodesX, ushort nodesY)
    {
        Vector3 startPos = transform.position;
        Vector3[] nodes = new Vector3[_nodesX * nodesY];
        float stepX = _width / _nodesX;
        float stepY = _height / nodesY;
        List<Edge> edges = new List<Edge>();
        set = new HashSet<Edge>();
        for (ushort i = 0; i < nodes.Length; i++)
        {
            ushort x = (ushort)(i % _nodesX);
            ushort y = (ushort)(i / _nodesX);
            nodes[i] = new Vector3(startPos.x + stepX * x, startPos.y + stepY * y, 0);

            bool isLeftSide = x == 0;
            bool isRightSide = x == _nodesX - one;
            bool isBottonSide = y == 0;
            bool isTop = y == nodesY - 1;

            //try add left neighbour
            ushort secondEdgeSide = (ushort)(i - one);
            if (isLeftSide == false && CheackIndexOfNeighbour(nodes, secondEdgeSide))
                TryAddAdge(i, secondEdgeSide);

            //try add left-up neighbour
            secondEdgeSide = (ushort)(i + _nodesX - one);
            if (isLeftSide == false && isTop == false && CheackIndexOfNeighbour(nodes, secondEdgeSide))
                TryAddAdge(i, secondEdgeSide);

            //try add up neighbour
            secondEdgeSide = (ushort)(i + _nodesX);
            if (isTop == false && CheackIndexOfNeighbour(nodes, secondEdgeSide))
                TryAddAdge(i, secondEdgeSide);

            //try add right-up neighbour
            secondEdgeSide = (ushort)(i + _nodesX + one);
            if (isRightSide == false && isTop == false && CheackIndexOfNeighbour(nodes, secondEdgeSide))
                TryAddAdge(i, secondEdgeSide);

            //try add right neighbour
            secondEdgeSide = (ushort)(i + one);
            if (isRightSide == false && CheackIndexOfNeighbour(nodes, secondEdgeSide))
                TryAddAdge(i, secondEdgeSide);

            //try add right-down neighbour
            secondEdgeSide = (ushort)(i - _nodesX + one);
            if (isRightSide == false && isBottonSide == false && CheackIndexOfNeighbour(nodes, secondEdgeSide))
                TryAddAdge(i, secondEdgeSide);

            //try add down neighbour
            secondEdgeSide = (ushort)(i - _nodesX);
            if (isBottonSide == false && CheackIndexOfNeighbour(nodes, secondEdgeSide))
                TryAddAdge(i, secondEdgeSide);

            //try add left-down neighbour
            secondEdgeSide = (ushort)(i - _nodesX - one);
            if (isLeftSide == false && isBottonSide == false && CheackIndexOfNeighbour(nodes, secondEdgeSide))
                TryAddAdge(i, secondEdgeSide);
        }
        return new Graph(nodes, set.ToArray());
    }

    void TryAddAdge(ushort indexA, ushort indexB)
    {
        Edge edge = new Edge(indexA, indexB);
        if (set.Contains(edge) == false)
            set.Add(edge);
    }
    bool CheackIndexOfNeighbour(Vector3[] nodes, ushort index)
    {
        return index < nodes.Length && index > 0;
    }
}
