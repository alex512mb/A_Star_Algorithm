using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EdgeBehaviour : MonoBehaviour
{
    LineRenderer m_line;
    private void Awake()
    {
        m_line = GetComponent<LineRenderer>();
    }
    public void SetPosition(Vector2 pointA, Vector2 pointB)
    {
        SetPositionA(pointA);
        SetPositionB(pointB);
    }
    public void SetPositionA(Vector2 pos)
    {
        m_line.SetPosition(0, pos);
    }
    public void SetPositionB(Vector2 pos)
    {
        m_line.SetPosition(1, pos);
    }
}
