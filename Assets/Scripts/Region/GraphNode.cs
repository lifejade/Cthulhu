using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GraphNode : MonoBehaviour
{
    public List<GraphEdgeNode> adj_list;

    protected abstract void UpdateBoardState();
}
