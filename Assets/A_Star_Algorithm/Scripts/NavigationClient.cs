using UnityEngine;

public class NavigationClient : MonoBehaviour
{
    [SerializeField] bool realTimeUpdate;
    [SerializeField] Navigation navigation;
    [SerializeField] LineRenderer line;
    [Range(1,30)]
    [SerializeField] float updatePerSecond = 5;
    float deleyUpdate;
    float lastUpdatePoint = -1;


    Vector3[] path = new Vector3[0];

    private void Awake()
    {
        deleyUpdate = 1f / updatePerSecond;
    }
    void Update()
    {
        if(realTimeUpdate && Time.time > lastUpdatePoint + deleyUpdate)
        {
            lastUpdatePoint = Time.time;
            FindAndShowPath();
        }
    }
    public void FindAndShowPath()
    {
        if (navigation.FindPath(ref path))
        {
            ApplyPathToLineRenderer(path);
        }
        else
            Debug.Log("path not exist in the graph");
    }
    public void ClearPath()
    {
        line.positionCount = 0;
    }

    void ApplyPathToLineRenderer(Vector3[] path)
    {
        line.positionCount = path.Length;
        for (int i = 0; i < path.Length; i++)
            line.SetPosition(i, new Vector3(path[i].x, path[i].y, -1));
    }

}
