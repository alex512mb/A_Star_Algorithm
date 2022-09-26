using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphRenderer : MonoBehaviour
{
    [SerializeField] float sizeTile = 0.1f;
    [SerializeField] Vector2 offset;
    [SerializeField] Texture tileTexture;
    [SerializeField] GraphData graph;

    float offset_x;
    float offset_y;

    private void Awake()
    {
        offset_x = offset.x * Screen.width;
        offset_y = offset.y * Screen.height;
    }
    private void OnGUI()
    {

        for (ushort i = 0; i < graph.graph.countNodes; i++)
        {
            Vector3 pos = graph.graph.GetNode(i);
            Rect rect = new Rect(offset_x + pos.x, offset_y + pos.y, sizeTile, sizeTile);
            GUI.DrawTexture(rect, tileTexture);
        }
    }
}
