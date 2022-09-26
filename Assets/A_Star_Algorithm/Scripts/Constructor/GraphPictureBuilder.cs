using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPictureBuilder : MonoBehaviour
{
    [SerializeField] float width;
    [SerializeField] float height;
    [SerializeField] GraphGridBuilder builder;
    [SerializeField] GraphData graph;
    [SerializeField] Texture2D texture;
    [SerializeField] float minColorAlpha = 0.1f;

    [ContextMenu("Generate")]
    void Generate()
    {
        graph.graph = builder.Generate(width, height, (ushort)texture.width, (ushort)texture.height);
        Color[] pixels = texture.GetPixels();
        for (ushort i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > minColorAlpha)
                graph.graph.AddObstacle((i));
        }

    }
}
