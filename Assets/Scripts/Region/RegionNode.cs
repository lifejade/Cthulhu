using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class RegionNode : GraphNode
{
    public enum Board_State { NULL, normal, start, end, enter, ban, heroine,monster,monsteronlyban };
    public Board_State board_State { get { return _board_state; }
        set {
            _board_state = value;
        } }
    public bool isOpen { get { return _isOpen; } set { _isOpen = value; UpdateBoardState(); } }
    private bool _isOpen = true;

    [HideInInspector]
    public bool initializeOver = false;

    [HideInInspector]
    [SerializeField]
    private Board_State _board_state = Board_State.normal;

    [HideInInspector]
    [SerializeField]
    public string SceneName;



    //public List<RegionNode> adj_RegionNode = new List<RegionNode>();
    public bool PlayerIsHere { get { return _playerIsHere; } 
        set {
            _playerIsHere = value;
            Vector3 vec = this.transform.position;
            vec.z--;
            Managers.Region.PlayerIcon.transform.position = vec;
        } }

    private bool _playerIsHere = false;

    private void Update()
    {
    }

    void Start()
    {
        isOpen = false;
        foreach (GraphEdgeNode e in Managers.Region.graph.edges)
        {
            if (e.node1 == this || e.node2 == this)
                adj_list.Add(e);
        }
        initializeOver = true;
        
    }

    public override void UpdateBoardState()
    {
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        if (isOpen)
        {
            switch (board_State)
            {
                case Board_State.normal:
                    render.color = Color.white;
                    break;
                case Board_State.start:
                    render.color = new Color(0,255 / 255,41f / 255);
                    break;
                case Board_State.heroine:
                    render.color = new Color(255 / 255f, 100 / 255f, 248 / 255f);
                    break;
                case Board_State.ban:
                    render.color = new Color(135 / 255f, 0, 0);
                    break;
                case Board_State.monster:
                    render.color = new Color(141 / 255f, 0, 255 / 255f);
                    break;
                case Board_State.enter:
                    render.color = new Color(255 / 255f, 225 / 255f, 0);
                    break;
                case Board_State.end:
                    render.color = new Color(0, 63 / 255f, 255 / 255f);
                    break;
                default:
                    render.color = Color.white;
                    break;
            }
        }
        else
        {
            render.color = Color.black;
        }
    }

    public void UpdateBoardStateOnEditorMode()
    {
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        switch (board_State)
        {
            case Board_State.normal:
                render.color = Color.white;
                break;
            case Board_State.start:
                render.color = new Color(0, 255 / 255, 41f / 255);
                break;
            case Board_State.heroine:
                render.color = new Color(255 / 255f, 100 / 255f, 248 / 255f);
                break;
            case Board_State.ban:
                render.color = new Color(135 / 255f, 0, 0);
                break;
            case Board_State.monster:
                render.color = new Color(141 / 255f, 0, 255 / 255f);
                break;
            case Board_State.enter:
                render.color = new Color(255 / 255f, 225 / 255f, 0);
                break;
            case Board_State.end:
                render.color = new Color(0, 63 / 255f, 255 / 255f);
                break;
            default:
                render.color = Color.white;
                break;
        }
    }
}

