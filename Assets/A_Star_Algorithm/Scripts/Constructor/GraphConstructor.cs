using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class GraphConstructor : MonoBehaviour
{
    public static GraphConstructor s_instance;

    public GraphEditMode editMode;

    [SerializeField] NodeBehaviour prefabNode;
    [SerializeField] EdgeBehaviour prefabEdge;
    [SerializeField] GraphData graphData;

    [SerializeField] Color colorNormalNode = Color.grey;
    [SerializeField] Color colorObstacleNode = Color.red;
    [SerializeField] Color colorStartNode = Color.blue;
    [SerializeField] Color colorDestinationNode = Color.green;
    const string namePropertyColor = "_Color";

    int indexNodeUnderMouse = -1;
    int indexSelectedNodeA = -1;

    List<NodeBehaviour> nodes;
    List<EdgeBehaviour> lines;

    Camera cam;
    Vector3 mousePos
    {
        get
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            return mouseWorldPos;
        }
    }
    bool isDragNode;
    ushort indexDragedNode;

    public event Action OnChangeGraph;


    void Awake()
    {
        s_instance = this;
        nodes = new List<NodeBehaviour>();
        lines = new List<EdgeBehaviour>();
        cam = Camera.main;
    }
    void Start ()
    {
        graphData.graph.OnRemoveNode += OnRemoveNode;
        graphData.graph.OnAddNode += CreateNodeObj;
        graphData.graph.OnChangePosNode += OnChangePosNode;

        graphData.graph.OnAddEdge += CreateLineObj;
        graphData.graph.OnRemoveEdge += OnRemoveEdge;

        for (ushort i = 0; i < graphData.graph.countNodes; i++)
        {
            CreateNodeObj(i);
        }
        for (ushort i = 0; i < graphData.graph.countEdges; i++)
        {
            CreateLineObj(i);
        }
    }
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (editMode == GraphEditMode.SetObstacleNodes)
        {
            if (indexNodeUnderMouse != -1 && Input.GetMouseButtonDown(0))
            {
                //remove obstacle
                if (graphData.graph.TryRemoveObstacle((ushort)indexNodeUnderMouse))
                {
                    UpdateColorOfNode(nodes[indexNodeUnderMouse], colorNormalNode);
                }
                //add obstacle
                else
                {
                    graphData.graph.AddObstacle((ushort)indexNodeUnderMouse);
                    UpdateColorOfNode(nodes[indexNodeUnderMouse], colorObstacleNode);
                }
            }
        }
        else if (editMode == GraphEditMode.SetStart)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (indexNodeUnderMouse != -1)
                {
                    if (indexNodeUnderMouse == graphData.graph.indexDestinationNode)
                    {
                        graphData.graph.indexDestinationNode = -1;
                    }
                    //revert color of previous node
                    if (graphData.graph.indexStartNode != -1)
                        UpdateColorOfNode(nodes[graphData.graph.indexStartNode], colorNormalNode);

                    //remove current node from obstracles
                    graphData.graph.TryRemoveObstacle((ushort)indexNodeUnderMouse);

                    //set node as star
                    graphData.graph.indexStartNode = indexNodeUnderMouse;
                    UpdateColorOfNode(nodes[indexNodeUnderMouse], colorStartNode);
                }
            }

        }
        else if (editMode == GraphEditMode.SetFinish)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (indexNodeUnderMouse != -1)
                {
                    if (indexNodeUnderMouse == graphData.graph.indexStartNode)
                    {
                        graphData.graph.indexStartNode = -1;
                    }
                    //revert color of previous node
                    if (graphData.graph.indexDestinationNode != -1)
                        UpdateColorOfNode(nodes[graphData.graph.indexDestinationNode], colorNormalNode);

                    //remove current node from obstracles
                    graphData.graph.TryRemoveObstacle((ushort)indexNodeUnderMouse);

                    //set node as destination
                    graphData.graph.indexDestinationNode = indexNodeUnderMouse;
                    UpdateColorOfNode(nodes[indexNodeUnderMouse], colorDestinationNode);
                }
            }

        }
        else if (editMode == GraphEditMode.Move)
        {
            if (indexNodeUnderMouse != -1 && Input.GetMouseButtonDown(0))
            {
                isDragNode = true;
                indexDragedNode = (ushort)indexNodeUnderMouse;
            }
            if (Input.GetMouseButtonUp(0))
                isDragNode = false;

            if (isDragNode)
                graphData.graph.SetPositionNode(indexDragedNode, mousePos);
        }
        else if (editMode == GraphEditMode.Link)
        {
            //select node A
            if (Input.GetMouseButtonDown(0) && indexNodeUnderMouse != -1)
            {
                indexSelectedNodeA = indexNodeUnderMouse;
            }
            if (Input.GetMouseButtonUp(0))
            {
                //deselect all
                if (indexNodeUnderMouse == -1)
                {
                    indexSelectedNodeA = -1;
                }
                //apply a new setup
                else if(indexSelectedNodeA != -1)
                {
                    Debug.Log("try add edge: " + indexSelectedNodeA + " -- " + indexNodeUnderMouse);
                    graphData.graph.AddRemoveEdge(new Edge((ushort)indexSelectedNodeA, (ushort)indexNodeUnderMouse));

                    //reset all
                    indexSelectedNodeA = -1;
                }

            }
        }
        else if (editMode == GraphEditMode.Create)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (indexNodeUnderMouse == -1)
                {
                    graphData.graph.AddNode(mousePos);
                }
                else
                {
                    graphData.graph.RemoveNode((ushort)indexNodeUnderMouse);
                    indexNodeUnderMouse = -1;
                    if (indexNodeUnderMouse == graphData.graph.indexStartNode)
                        graphData.graph.indexStartNode = -1;
                    if (indexNodeUnderMouse == graphData.graph.indexDestinationNode)
                        graphData.graph.indexDestinationNode = -1;
                }
            }

        }


    }


    //Commands
    void CreateLineObj(ushort index)
    {
        EdgeBehaviour edge = Instantiate<EdgeBehaviour>(prefabEdge, Vector3.zero, Quaternion.identity);
        edge.SetPosition(graphData.graph.GetFirstPointOfEdge(index), graphData.graph.GetSecondPointOfEdge(index));
        lines.Add(edge);
    }
    void CreateNodeObj(ushort indexNode)
    {
        NodeBehaviour node = Instantiate<NodeBehaviour>(prefabNode, graphData.graph.GetNode(indexNode), Quaternion.identity);
        node.indexNode = indexNode;
        node.On_MouseEnter += MouseEnterOverNode;
        node.On_MouseExit += MouseExitOverNode;
        nodes.Add(node);
        UpdateColorOfNode((ushort)(nodes.Count - 1));

    }
    Color GetColorForNode(ushort indexNode)
    {
        if (indexNode == graphData.graph.indexStartNode)
            return colorStartNode;
        else if (indexNode == graphData.graph.indexDestinationNode)
            return colorDestinationNode;
        else if (graphData.graph.ExistObstacle(indexNode))
            return colorObstacleNode;
        else
            return colorNormalNode;
    }
    void UpdateColorOfNode(ushort indexNode)
    {
        UpdateColorOfNode(nodes[indexNode], GetColorForNode(indexNode));
    }
    void UpdateColorOfNode(NodeBehaviour node, Color color)
    {
        node.m_renderer.material.SetColor(namePropertyColor, color);
    }


    //event handlers
    void MouseEnterOverNode(NodeBehaviour node)
    {
        indexNodeUnderMouse = node.indexNode;
    }
    void MouseExitOverNode(NodeBehaviour node)
    {
        indexNodeUnderMouse = -1;
    }
    void OnRemoveNode(ushort index)
    {
        graphData.graph.TryRemoveObstacle(index);

        Destroy(nodes[index].gameObject);
        nodes.RemoveAt(index);

        //update indexes
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].indexNode = i;
        }
    }
    void OnChangePosNode(ushort index, Vector3 pos)
    {
        nodes[index].transform.position = pos;

        //change position for depended edges
        for (ushort i = 0; i < graphData.graph.countEdges; i++)
        {
            if (graphData.graph.GetEdge(i).a == index)
                lines[i].SetPositionA(pos);
            else if (graphData.graph.GetEdge(i).b == index)
                lines[i].SetPositionB(pos);
        }


    }
    void OnRemoveEdge(ushort index)
    {
        Destroy(lines[index].gameObject);
        lines.RemoveAt(index);
    }


    public enum GraphEditMode
    {
        Create,
        Link,
        Move,
        SetStart,
        SetFinish,
        SetObstacleNodes,
    }
}





