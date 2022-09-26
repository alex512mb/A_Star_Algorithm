using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;

public class Tests : MonoBehaviour {
    public GraphData graph;

    public int previousHash;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        method1();

    }

    [ContextMenu("method1")]
    void method1()
    {
        //var s1 = Stopwatch.StartNew();
        int currentHash = graph.graph.GetHashCode();
        if (previousHash != currentHash)
        {
            UnityEngine.Debug.Log("hash changed");
        }
        previousHash = currentHash;

        //s1.Stop();
        //UnityEngine.Debug.Log(((double)s1.Elapsed.TotalMilliseconds * 1000).ToString() + " ns");
    }
    [ContextMenu("GetPerfomance")]
    void GetPerfomance()
    {
        List<double> list = new List<double>();
        for (int i = 0; i < 10; i++)
        {
            Stopwatch s1 = Stopwatch.StartNew();
            int currentHash = graph.graph.GetHashCode();
            s1.Stop();
            double ns = s1.Elapsed.TotalMilliseconds * 1000;
            list.Add(ns);
        }

        UnityEngine.Debug.Log("Average time : " + (list.Average()).ToString() + " ns");
    }
}


