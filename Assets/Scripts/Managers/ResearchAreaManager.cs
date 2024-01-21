using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResearchAreaManager : MonoBehaviour, EManager
{
    TutorialComponent tutorial;

    public ResearchAreaGraphEdit graph
    {
        get
        {
            if (_graph == null)
            {
                _graph = GameObject.FindObjectOfType<ResearchAreaGraphEdit>();
            }
            return _graph;
        }
    }
    private ResearchAreaGraphEdit _graph = null;

    ResearchAreaGraphNode PlayerIsIn {
        get
        {
            if (_playerIsIn == null)
            {
                _playerIsIn = graph.regions[0];
                _playerIsIn.PlayerIsHere = true;
            }
            return _playerIsIn;
        }
        set
        {
            if (_playerIsIn != null)
                _playerIsIn.PlayerIsHere = false;
            _playerIsIn = value;
            _playerIsIn.PlayerIsHere = true;
        }
    }
    ResearchAreaGraphNode _playerIsIn = null;

    public void init()
    {
        StartCoroutine(CheckNodeInitOver());
    }

    IEnumerator CheckNodeInitOver()
    {
        yield return new WaitUntil(() => graph.regions.All(value => { return value.initializeOver; }));
        ResearchAreaGraphNode tmp = PlayerIsIn;
        tutorial = GameObject.Find("Canvas").GetComponent<TutorialComponent>();
        tutorial.SetTutorialActive("Tutorial-Research Basic");
    }

    public void update_(){}

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 m_vecMouseDownPos = Input.mousePosition;
            Vector2 pos = Camera.main.ScreenToWorldPoint(m_vecMouseDownPos);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit.collider != null)
            {
                ResearchAreaItemListener listener = hit.collider.gameObject.GetComponent<ResearchAreaItemListener>();
                if (listener != null)
                {
                    if (!Managers.PlayerData.HaveItem.ContainsKey(listener.id))
                    {
                        Managers.PlayerData.HaveItem.Add(listener.id, true);
                    }
                    else
                    {
                        Managers.PlayerData.HaveItem[listener.id] = true;
                    }
                    Managers.PlayerData.savePlayerData();
                }
            }
        }
        
    }


    EManager EManager.createInstance()
    {
        return ResearchAreaManager.createInstance();
    }

    public static ResearchAreaManager createInstance()
    {
        GameObject go = GameObject.Find("@ResearchAreaManager");
        if (go == null)
        {
            go = new GameObject { name = "@ResearchAreaManager" };
        }

        ResearchAreaManager result = go.GetComponent<ResearchAreaManager>();
        if (result == null)
            result = go.AddComponent<ResearchAreaManager>();
        result.init();
        return result;
    }

    public void pressButton(ResearchAreaUI.Arrow arrow)
    {
        GraphEdgeNode e = getENodeWithArrow(arrow);
        if (e == null)
            return;

        Vector3 vec = getVecWithArrow(arrow);
        PlayerIsIn = (ResearchAreaGraphNode)e.opposite(PlayerIsIn);
        Camera.main.transform.position += vec;
    }


    public GraphEdgeNode getENodeWithArrow(ResearchAreaUI.Arrow arrow)
    {
        GraphEdgeNode e = null;
        switch (arrow)
        {
            case ResearchAreaUI.Arrow.left:
                e = PlayerIsIn.adj_list.Find(value => value.opposite(PlayerIsIn).gameObject.transform.position.x < PlayerIsIn.transform.position.x);
                break;
            case ResearchAreaUI.Arrow.right:
                e = PlayerIsIn.adj_list.Find(value => value.opposite(PlayerIsIn).gameObject.transform.position.x > PlayerIsIn.transform.position.x);
                break;
            case ResearchAreaUI.Arrow.up:
                e = PlayerIsIn.adj_list.Find(value => value.opposite(PlayerIsIn).gameObject.transform.position.y > PlayerIsIn.transform.position.y);
                break;
            case ResearchAreaUI.Arrow.down:
                e = PlayerIsIn.adj_list.Find(value => value.opposite(PlayerIsIn).gameObject.transform.position.y < PlayerIsIn.transform.position.y);
                break;
        }

        return e;
    }


    public Vector3 getVecWithArrow(ResearchAreaUI.Arrow arrow)
    {
        Vector3 vec = Vector3.zero;
        switch (arrow)
        {
            case ResearchAreaUI.Arrow.left:
                vec.x = -100;
                break;
            case ResearchAreaUI.Arrow.right:
                vec.x = 100;
                break;
            case ResearchAreaUI.Arrow.up:
                vec.y = 100;
                break;
            case ResearchAreaUI.Arrow.down:
                vec.y = -100;
                break;
        }

        return vec;
    }

    void OnDestroy()
    {

    }


}
