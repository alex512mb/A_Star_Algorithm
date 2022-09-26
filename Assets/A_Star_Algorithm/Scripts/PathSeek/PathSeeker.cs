using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathSeeker : MonoBehaviour
{
    public abstract bool FindPath(Graph graph, ref Vector3[] path);
    public abstract void Bake(Graph graph);
}
