using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq;

public class A_Star_Seeker : PathSeeker
{
    [SerializeField] bool printPerformance;
    HashSet<ushort> indexesObstacle = new HashSet<ushort>();
    SortedArray indexesOpenNodes = new SortedArray();
    HashSet<ushort> indexesCloseNodes = new HashSet<ushort>();
    Dictionary<ushort, ushort[]> dictEdges = new Dictionary<ushort, ushort[]>();
    ushort[] indexesLinkedNodes = new ushort[0];
    NodePath[] nodes = new NodePath[0];

    int previousHashCode = 0;
    ushort previousCountNodes;
    Stopwatch s1;

    public SortedArray arr1 = new SortedArray();
    public ushort value1;
    public ushort keytest;
    [ContextMenu("asasaa")]
    void test()
    {
        arr1.Add(keytest, value1);
    }
    [Serializable]
    public class SortedArray
    {
        public SortedArray()
        {
            arr = new KeyValuePair<ushort, double>[0];
        }
        public KeyValuePair<ushort,double>[] arr;
        public KeyValuePair<ushort, double> this[int index]
        {
            get { return arr[index]; }
            set { arr[index] = value; }
        }
        public int Count
        {
            get
            {
                return arr.Length;
            }
        }
        public void Add(ushort key, double value)
        {
            if (arr.Length == 0)
            {
                arr = new KeyValuePair<ushort, double>[1]{ new KeyValuePair<ushort, double>(key, value) };
                return;
            }

            for (int i = 0; i < arr.Length; i++)
            {
                if (value <= arr[i].Value)
                {
                    AddAt(new KeyValuePair<ushort, double>(key, value), i);
                    return;
                }
            }

            //add to the end
            Array.Resize(ref arr, arr.Length + 1);
            arr[arr.Length - 1] = new KeyValuePair<ushort, double>(key, value);
        }
        void AddAt(KeyValuePair<ushort, double> item, int index)
        {
            if (index < arr.Length || arr.Length == 0)
            {
                Array.Resize(ref arr, arr.Length + 1);
                Array.Copy(arr, index, arr, index + 1, arr.Length - index - 1);
                arr[index] = item;
            }
            else
                UnityEngine.Debug.LogError("try add new element but index out of range");
        }
        public void Remove(ushort key)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (key == arr[i].Key)
                {
                    //remove element
                    Array.Copy(arr, i + 1, arr, i, arr.Length - i - 1);
                    Array.Resize(ref arr, arr.Length - 1);
                    return;
                }
            }
            UnityEngine.Debug.LogError("key not fonded");
        }
        public bool ContainsKey(ushort key)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (key == arr[i].Key)
                    return true;
            }
            return false;
        }
        public void Clear()
        {
            Array.Resize(ref arr, 0);
        }
    }



    public override bool FindPath(Graph graph, ref Vector3[] path)
    {
        if(printPerformance)
            s1 = Stopwatch.StartNew();

        if (graph.indexStartNode == -1 || graph.indexDestinationNode == -1 || graph.indexDestinationNode == graph.indexStartNode)
            return false;


        //if graph was change rebake
        int currentHashCode = graph.GetHashCode();
        if (previousHashCode != currentHashCode)
        {
            UnityEngine.Debug.Log("need rebake");
            Bake(graph);
            previousHashCode = currentHashCode;
        }

        //Init
        indexesOpenNodes.Add((ushort)graph.indexStartNode, 0);
        bool isWorkCompete = false;
        ushort indexCurrentNode = (ushort)graph.indexStartNode;
        double bestCost = double.MaxValue;
        ushort indexBestNode = indexCurrentNode;
        for (int i = 0; i < nodes.Length; i++)
            nodes[i].Clear();

        

        //try find path
        while (indexesOpenNodes.Count > 0)
        {
            //find best node
            indexCurrentNode = indexesOpenNodes[0].Key;

            indexesCloseNodes.Add(indexCurrentNode);
            indexesOpenNodes.Remove(indexCurrentNode);


            //find linked nodes
            indexesLinkedNodes = dictEdges[indexCurrentNode];
            int countLinks = indexesLinkedNodes.Length;
            for (int i1 = 0; i1 < countLinks; i1++)
            {
                ushort indexLinkedNode = indexesLinkedNodes[i1];

                //ignore obstracles and closed nodes
                if (indexesObstacle.Contains(indexLinkedNode) || indexesCloseNodes.Contains(indexLinkedNode))
                    continue;

                //if it is a new unknown node
                if (indexesOpenNodes.ContainsKey(indexLinkedNode) == false)
                {
                    nodes[indexLinkedNode].SetDestination(nodes[graph.indexDestinationNode]);
                    nodes[indexLinkedNode].SetParent(nodes[indexCurrentNode]);
                    indexesOpenNodes.Add(indexLinkedNode, nodes[indexLinkedNode].pathCost);

                    if (nodes[indexLinkedNode].pathCost < bestCost)
                    {
                        indexBestNode = indexLinkedNode;
                        bestCost = nodes[indexLinkedNode].pathCost;
                    }

                }
                else
                {
                    if (nodes[indexLinkedNode].TryOverrideParent(nodes[indexCurrentNode]))
                    {
                        if (nodes[indexLinkedNode].pathCost < bestCost)
                        {
                            indexBestNode = indexLinkedNode;
                            bestCost = nodes[indexLinkedNode].pathCost;
                        }
                    }

                }

                //if we came to the destination
                if (indexLinkedNode == graph.indexDestinationNode)
                    isWorkCompete = true;
            }
        }

        if (printPerformance)
        {
            s1.Stop();
            double ms = s1.Elapsed.TotalMilliseconds;
            if(ms >= 3)
                UnityEngine.Debug.Log(string.Format("time operation find path: {0} milli-second ", ms));
            else
                UnityEngine.Debug.Log(string.Format("time operation find path: {0} nano-seconds ", ms * 1000));
        }

        indexesOpenNodes.Clear();
        indexesCloseNodes.Clear();

        //work done
        if (isWorkCompete)
        {
            path = nodes[graph.indexDestinationNode].GetPath();
            return true;
        }

        return isWorkCompete;
    }
    public override void Bake(Graph graph)
    {
        Array.Resize(ref nodes, graph.countNodes);
        for (ushort i = 0; i < nodes.Length; i++)
        {
            if (i >= previousCountNodes)
                nodes[i] = new NodePath(graph.GetNode(i), i);
            else
                nodes[i].SetValues(graph.GetNode(i), i);
        }
        previousCountNodes = (ushort)nodes.Length;

        dictEdges = new Dictionary<ushort, ushort[]>();
        for (ushort i = 0; i < graph.countNodes; i++)
            dictEdges.Add(i, graph.GetIndexesLinkedNode(i));

        indexesObstacle = new HashSet<ushort>();
        for (ushort i = 0; i < graph.countObstacles; i++)
        {
            indexesObstacle.Add(graph.GetObstacle(i));
        }
    }
    

    [Serializable]
    public class NodePath
    {
        public NodePath(Vector3 pos, ushort _index)
        {
            SetValues(pos, _index);
        }

        //variables
        public ushort index;
        Vector3 position;
        double distanceToRoot;
        double straightDistance;
        public double pathCost;
        NodePath parent;

        // overload operator <
        public static bool operator <(NodePath a, NodePath b)
        {
            return a.pathCost < b.pathCost;
        }
        // overload operator <
        public static bool operator >(NodePath a, NodePath b)
        {
            return a.pathCost > b.pathCost; ;
        }

        //public func
        public void SetDestination(NodePath node)
        {
            straightDistance = GetStraightDistance(node.position, position); // Vector3.Distance(node.position, position);
        }
        public void SetParent(NodePath newParent)
        {
            parent = newParent;
            distanceToRoot =  Vector3.Distance(parent.position, position) + newParent.distanceToRoot;
            UpdatePathCost();
        }
        public bool TryOverrideParent(NodePath supposedParent)
        {
            //for this node we suppose the new parent
            double supposedDistance = supposedParent.distanceToRoot + Vector3.Distance(supposedParent.position, position);
            if (supposedDistance < distanceToRoot)
            {
                SetParent(supposedParent);
                return true;
            }
            else
                return false;
        }
        public Vector3[] GetPath()
        {
            List<Vector3> path = new List<Vector3>();
            NodePath node = this;

            bool isRootNode = false;
            while (isRootNode == false)
            {
                path.Add(node.position);
                node = node.parent;
                isRootNode = node.parent == null;
            }

            path.Add(node.position);
            path.Reverse();
            return path.ToArray();
        }
        public void SetValues(Vector3 pos, ushort _index)
        {
            position = pos;
            index = _index;
            parent = null;
        }
        public void Clear()
        {
            parent = null;
        }


        double GetStraightDistance(Vector3 A, Vector3 B)
        {
            return Math.Abs(A.x - B.x) + Math.Abs(A.y - B.y) + Math.Abs(A.z - B.z);
        }
        void UpdatePathCost()
        {
            pathCost = distanceToRoot + straightDistance;
        }
    }
}





