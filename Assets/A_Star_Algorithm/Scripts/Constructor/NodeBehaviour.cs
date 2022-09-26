using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class NodeBehaviour : MonoBehaviour
{
    public int indexNode;

    //events
    /// <summary>
    /// Called when the mouse enters the stock
    /// </summary>
    public event Action<NodeBehaviour> On_MouseEnter;
    /// <summary>
    /// Called when the mouse exits the stock
    /// </summary>
    public event Action<NodeBehaviour> On_MouseExit;

    [HideInInspector]
    public MeshRenderer m_renderer { get; private set; }

    private void Awake()
    {
        m_renderer = GetComponent<MeshRenderer>();
    }

    void OnMouseEnter()
    {
        if (On_MouseEnter != null)
            On_MouseEnter(this);
    }
    void OnMouseExit()
    {
        if (On_MouseExit != null)
            On_MouseExit(this);
    }
}
