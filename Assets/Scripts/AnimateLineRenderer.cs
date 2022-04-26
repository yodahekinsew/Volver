using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateLineRenderer : MonoBehaviour
{
    public Color color;
    public LineRenderer line;

    void Update()
    {
        line.startColor = color;
        line.endColor = color;
    }
}
