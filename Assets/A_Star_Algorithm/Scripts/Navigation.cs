using UnityEngine;

public class Navigation : MonoBehaviour
{
    [SerializeField] PathSeeker seeker;
    [SerializeField] GraphData graph;

    public bool FindPath(ref Vector3[] path)
    {
        return seeker.FindPath(graph.graph ,ref path);
    }
}
