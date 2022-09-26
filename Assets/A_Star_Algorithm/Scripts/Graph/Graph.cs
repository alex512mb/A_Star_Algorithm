using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Graph
{
    public Graph(Vector3[] _nodes, Edge[] _edges)
    {
        nodes = _nodes;
        edges = _edges;

        OnRemoveEdge = null;
        OnAddEdge = null;
        OnRemoveNode = null;
        OnAddNode = null;
        OnChangePosNode = null;

        ushort[] abstracles = new ushort[0];
        indexStartNode = 0;
        indexDestinationNode = nodes.Length -1;
        obstacles = new ushort[0];
    }
    public override int GetHashCode()
    {
        int hc = edges.Length;
        for (int i = 0; i < edges.Length; ++i)
        {
            hc = unchecked(hc * 17 + edges[i].GetHashCode());
        }

        int hc2 = nodes.Length;
        for (int i = 0; i < nodes.Length; ++i)
        {
            hc2 = unchecked(hc2 * 17 + nodes[i].GetHashCode());
        }

        int hc3 = obstacles.Length;
        for (int i = 0; i < obstacles.Length; ++i)
        {
            hc3 = unchecked(hc3 * 17 + obstacles[i]); 
        }
        //int result = unchecked(hc + hc2 + hc3 + indexStartNode - indexDestinationNode);
        int result = unchecked(hc + hc2 + hc3);

        return result;
    }

    public int indexStartNode;
    public int indexDestinationNode;

    //data
    [SerializeField] Vector3[] nodes;
    public ushort countNodes
    {
        get
        {
            return (ushort)nodes.Length;
        }
    }
    [SerializeField] Edge[] edges;
    public ushort countEdges
    {
        get
        {
            return (ushort)edges.Length;
        }
    }
    ushort[] obstacles;
    public ushort countObstacles
    {
        get
        {
            return (ushort)obstacles.Length;
        }
    }

    //events
    public event Action<ushort> OnRemoveEdge;
    public event Action<ushort> OnAddEdge;
    public event Action<ushort> OnRemoveNode;
    public event Action<ushort> OnAddNode;
    public event Action<ushort, Vector3> OnChangePosNode;

    //Obstacle
    public bool ExistObstacle(ushort index)
    {
        ushort count = (ushort)obstacles.Length;
        for (ushort i = 0; i < count; i++)
        {
            if (obstacles[i] == index)
                return true;
        }
        return false;
    }
    public ushort GetObstacle(ushort index)
    {
        return obstacles[index];
    }
    public void AddObstacle(ushort index)
    {
        Array.Resize(ref obstacles, obstacles.Length + 1);
        obstacles[obstacles.Length - 1] = index;
    }
    public bool TryRemoveObstacle(ushort index)
    {
        for (ushort i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] == index)
            {
                Array.Copy(obstacles, i + 1, obstacles, i, obstacles.Length - i - 1);
                Array.Resize(ref obstacles, obstacles.Length - 1);
                return true;
            }
        }
        return false;
    }

    //edges
    public Vector3 GetFirstPointOfEdge(ushort indexEdge)
    {
        return nodes[edges[indexEdge].a];
    }
    public Vector3 GetSecondPointOfEdge(ushort indexEdge)
    {
        return nodes[edges[indexEdge].b];
    }
    public bool AddRemoveEdge(Edge edge)
    {
        for (ushort i = 0; i < edges.Length; i++)
        {
            if (edges[i].Equals(edge))
            {
                RemoveEdgeAt(i);
                return false;
            }
        }

        //add new edge
        Array.Resize(ref edges, edges.Length + 1);
        edges[edges.Length - 1] = edge;

        if (OnAddEdge != null)
            OnAddEdge((ushort)(edges.Length - 1));

        return true;
    }
    public Edge GetEdge(ushort indexEdge)
    {
        return edges[indexEdge];
    }
    void RemoveEdgeAt(ushort index)
    {
        if (OnRemoveEdge != null)
            OnRemoveEdge(index);

        Array.Copy(edges, index + 1, edges, index, edges.Length - index - 1);
        Array.Resize(ref edges, edges.Length - 1);
    }

    //nodes
    public void AddNode(Vector3 pos)
    {
        //add new node
        Array.Resize(ref nodes, nodes.Length + 1);
        nodes[nodes.Length - 1] = pos;

        if (OnAddNode != null)
            OnAddNode((ushort)(nodes.Length - 1));
    }
    public void RemoveNode(ushort index)
    {
        if (OnRemoveNode != null)
            OnRemoveNode(index);

        //find depended edges
        List<int> dependedEdges = new List<int>();
        for (int i = 0; i < edges.Length; i++)
        {
            if (edges[i].a == index || edges[i].b == index)
                if(dependedEdges.Contains(i) == false)
                    dependedEdges.Add(i);
        }
        //remove depended edges
        for (int i = 0; i < dependedEdges.Count; i++)
            RemoveEdgeAt((ushort)(dependedEdges[i]-i));


        //change pointer to node in edges
        for (int i = 0; i < edges.Length; i++)
        {
            if (edges[i].a > index)
                edges[i].a--;
            if (edges[i].b > index)
                edges[i].b--;
        }
        if (indexDestinationNode > index)
            indexDestinationNode--;
        if (indexStartNode > index)
            indexStartNode--;

        //remove node
        Array.Copy(nodes, index + 1, nodes, index, nodes.Length - index - 1);
        Array.Resize(ref nodes, nodes.Length - 1);

    }
    public Vector3 GetNode(ushort indexNode)
    {
        return nodes[indexNode];
    }
    public void SetPositionNode(ushort index, Vector3 pos)
    {
        nodes[index] = pos;

        if (OnChangePosNode != null)
            OnChangePosNode(index, pos);
    }
    public ushort[] GetIndexesLinkedNode(ushort indexNode)
    {
        ushort[] arr = new ushort[0];
        for (int i = 0; i < edges.Length; i++)
        {
            if (edges[i].a == indexNode)
            {
                Array.Resize(ref arr, arr.Length + 1);
                arr[arr.Length - 1] = edges[i].b;
            }
            else if (edges[i].b == indexNode)
            {
                Array.Resize(ref arr, arr.Length + 1);
                arr[arr.Length - 1] = edges[i].a;
            }
        }

        return arr;
    }

    public void Clear()
    {
        nodes = new Vector3[0];
        edges = new Edge[0];

        indexStartNode = 0;
        indexDestinationNode = 0;
        obstacles = new ushort[0];
    }
}

