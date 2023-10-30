using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchAreaGraphNode : GraphNode
{
    public bool initializeOver = false;


    //public List<RegionNode> adj_RegionNode = new List<RegionNode>();
    public bool PlayerIsHere
    {
        get { return _playerIsHere; }
        set
        {
            _playerIsHere = value;
            UpdateBoardState();
        }
    }

    private bool _playerIsHere = false;



    void Start()
    {
        foreach (GraphEdgeNode e in Managers.ResearchArea.graph.edges)
        {
            if (e.node1 == this || e.node2 == this)
                adj_list.Add(e);
        }
        initializeOver = true;
    }

    public override void UpdateBoardState()
    {
        Image render = GetComponent<Image>();
        if (PlayerIsHere)
        {
            render.color = Color.yellow;
        }
        else
        {
            render.color = Color.white;
        }

    }
}
