using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GraphNode : MonoBehaviour
{
    public List<GraphEdgeNode> adj_list;

    public abstract void UpdateBoardState();
}
