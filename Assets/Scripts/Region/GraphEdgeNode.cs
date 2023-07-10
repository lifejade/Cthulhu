using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GraphEdgeNode
{
    
    public GraphNode node1;
    public GraphNode node2;
    [HideInInspector]
    public LineRenderer Line;

    public bool isIn(GraphNode node)
    {
        return node == node1 || node == node2;
    }

    public GraphNode opposite(GraphNode n)
    {
        if (n == node1)
        {
            return node2;
        }
        else if (n == node2)
        {
            return node1;
        }
        return null;
    }
}
