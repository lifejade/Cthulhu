using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResearchAreaManager : MonoBehaviour, EManager
{
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
    }

    public void update_()
    {

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
            go.transform.parent = Managers.MGRS.transform;
        }

        ResearchAreaManager result = go.GetComponent<ResearchAreaManager>();
        if (result == null)
            result = go.AddComponent<ResearchAreaManager>();
        result.init();
        return result;
    }

    public void pressButton(int arrow)
    {
        GraphEdgeNode e = null;
        ResearchAreaGraphNode dest = null;
        Vector3 vec = Vector3.zero;
        switch (arrow)
        {
            case 0:
                e = PlayerIsIn.adj_list.Find(value => value.opposite(PlayerIsIn).gameObject.transform.position.x < PlayerIsIn.transform.position.x);
                vec.x = -100;
                break;
            case 1:
                e = PlayerIsIn.adj_list.Find(value => value.opposite(PlayerIsIn).gameObject.transform.position.x > PlayerIsIn.transform.position.x);
                vec.x = 100;
                break;
            case 2:
                e = PlayerIsIn.adj_list.Find(value => value.opposite(PlayerIsIn).gameObject.transform.position.y > PlayerIsIn.transform.position.y);
                vec.y = 100;
                break;
            case 3:
                e = PlayerIsIn.adj_list.Find(value => value.opposite(PlayerIsIn).gameObject.transform.position.y < PlayerIsIn.transform.position.y);
                vec.y = -100;
                break;
        }

        if (e == null)
            return;
        dest = (ResearchAreaGraphNode)e.opposite(PlayerIsIn);
        PlayerIsIn = dest;
        Camera.main.transform.position += vec;
    }
}
