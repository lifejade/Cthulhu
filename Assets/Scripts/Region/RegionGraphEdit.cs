using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RegionGraphEdit : MonoBehaviour
{
    public RegionNode[] regions;
    [SerializeField]
    public GraphEdgeNode[] edges;

    public Material lineMaterial;

    private void Update()
    {
        regions = GameObject.FindObjectsOfType<RegionNode>();

        if (edges.Length == 0)
            return;

        foreach (GraphEdgeNode e in edges)
        {
            if (e.node1 == null || e.node2 == null)
                continue;

            if (e.Line == null)
            {

                GameObject go = new GameObject { name = $"{e.node1.name} To {e.node2.name}" };
                go.transform.parent = this.gameObject.transform;
                e.Line = go.AddComponent<LineRenderer>();

                LineInit(e);
            }
            else
            {
                LineUpdate(e);
            }
        }
    }

    void LineInit(GraphEdgeNode e)
    {
        e.Line.positionCount = 2;
        e.Line.startWidth = 0.2f; e.Line.endWidth = 0.2f;
        e.Line.startColor = Color.blue; e.Line.endColor = Color.blue;
        e.Line.material = lineMaterial;
        LineUpdate(e);
    }

    void LineUpdate(GraphEdgeNode e)
    {
        Vector3 vec1 = e.node1.gameObject.transform.position;
        Vector3 vec2 = e.node2.gameObject.transform.position;
        vec1.z++;
        vec2.z++;

        e.Line.SetPositions(new Vector3[2] { vec1, vec2 });
    }

}
