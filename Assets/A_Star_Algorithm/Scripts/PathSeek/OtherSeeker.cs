using System;
using UnityEngine;

public class OtherSeeker : PathSeeker
{
    public override bool FindPath(Graph graph, ref Vector3[] path)
    {
        path = new Vector3[2];
        path[0] = graph.GetNode((ushort)graph.indexStartNode);
        path[1] = graph.GetNode((ushort)graph.indexDestinationNode); 
        return true;
    }
    public override void Bake(Graph graph)
    {
    }
}
