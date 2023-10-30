using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RegionManager : MonoBehaviour, EManager
{
    public int playerMaxTurn = 18;
    public int playerTurn { get { return _playerTurn; } set { _playerTurn = value;RegionUI.updaterestTurn(_playerTurn); } }
    private int _playerTurn = 18;

    public int dayTurn = 3;
    public int nightTurn = 5;
    public int monsterMaxNum = 1;


    public int sight = 2;
    public GameObject PlayerIcon;
    public GameObject Monsterprefab = null;
    public List<RegionMonster> Monsters = new List<RegionMonster>();
    public int dead_count = 0;

    private TutorialComponent tutorial;
    private string last_move_scene;
    
    private bool isRegionMap = true;

    public RegionGraphEdit graph { 
        get {
            
            if(_graph == null)
            {
                _graph = GameObject.FindObjectOfType<RegionGraphEdit>();
            }
            return _graph; 
        } 
    }
    [SerializeField]
    private RegionGraphEdit _graph = null;


    public RegionMapUI RegionUI
    {
        get
        {
            if(_regionUI== null)
            {
                _regionUI = GameObject.Find("Canvas").GetComponent<RegionMapUI>();
            }
            _regionUI.updaterestTurn(playerTurn);
            return _regionUI;
        }
        
    }
    private RegionMapUI _regionUI;

    public GameObject EnterUI { 
        get {
            if(_enterUI == null)
            {
                _enterUI = RegionUI.SceneMoveParents.transform.Find("SceneMove").gameObject;
                _enterUI.transform.GetChild(0).Find("yes").GetComponent<Button>().onClick.AddListener(delegate() { isRegionMap = false; SceneChanger.instance.ChangeScene(last_move_scene); });
            }
                
            return _enterUI;
        }
    }
    private GameObject _enterUI;

    public GameObject ClearEnterUI
    {
        get
        {
            if (_clearenterUI == null)
            {
                _clearenterUI = RegionUI.SceneMoveParents.transform.Find("ClearSceneMove").gameObject;
                _clearenterUI.transform.GetChild(0).Find("yes").GetComponent<Button>().onClick.AddListener(delegate () { isRegionMap = false; PlayerInNameLast = null; SceneChanger.instance.ChangeScene("Lobby 2"); });
            }

            return _clearenterUI;
        }
    }
    private GameObject _clearenterUI;


    [HideInInspector]
    public string PlayerInNameLast = null;
    public Dictionary<string,string> MonsterInNamesLast = new Dictionary<string,string>();
    

    public RegionNode PlayerIsIn
    {
        get { 
            if (_playerIsIn == null) 
            {
                _playerIsIn = graph.regions.First(value => { return value.board_State == RegionNode.Board_State.start; });
                if(_playerIsIn == null)
                    _playerIsIn = graph.regions[0]; 
                _playerIsIn.PlayerIsHere = true;
                PlayerInNameLast = _playerIsIn.gameObject.name;
                isInSight();
            } return _playerIsIn; }
        set
        {
            if (_playerIsIn != null)
                _playerIsIn.PlayerIsHere = false;
            _playerIsIn = value;
            _playerIsIn.PlayerIsHere = true;
            PlayerInNameLast = _playerIsIn.gameObject.name;

            isInSight();
        }
    }
    private RegionNode _playerIsIn;

    public void init()
    {
        if(PlayerIcon == null) PlayerIcon = GameObject.Find("PlayerIcon");
        if (Monsterprefab == null) Monsterprefab = Resources.Load<GameObject>("Prefab/Region/MonsterIcon");
        if (tutorial == null) tutorial = GameObject.Find("Canvas").GetComponent<TutorialComponent>();
        playerTurn = playerMaxTurn;
        StartCoroutine(CheckRegionNodeInitOver());
    }

    IEnumerator CheckRegionNodeInitOver()
    {
        yield return new WaitUntil(() => graph.regions.All(value => { return value.initializeOver; }));
        createMonster();
        isInSight();
        isRegionMap = true;
        tutorial.SetTutorialActive("Tutorial-Basic step");
    }

    public static RegionManager createInstance()
    {
        GameObject go = GameObject.Find("@RegionManager");
        if (go == null)
        {
            go = new GameObject { name = "@RegionManager" };
            //Todo : remove
            go.transform.parent = Managers.MGRS.transform;
        }

        RegionManager result = go.GetComponent<RegionManager>();
        if (result == null)
            result = go.AddComponent<RegionManager>();
        result.init();
        return result;
    }
    EManager EManager.createInstance()
    {
        return RegionManager.createInstance();
    }

    public void update_() { }
    public void Update()
    {
        if (!isRegionMap)
            return;


        if (playerTurn <= 0 && PlayerIsIn.board_State != RegionNode.Board_State.end)
        {
            //if(조건만족)
            PlayerDead();
            Debug.Log("GameOver");
            isRegionMap = false;
            PlayerInNameLast = null; SceneChanger.instance.ChangeScene("Lobby 2");
            return;
        }

        if((playerMaxTurn - playerTurn) % (dayTurn + nightTurn) >= dayTurn)
        {
            foreach(RegionMonster m in Monsters)
            {
                if (m.IsActive && m.here == PlayerIsIn)
                {
                    PlayerDead();
                    PlayerIsIn = graph.regions.First(value => { return value.board_State == RegionNode.Board_State.start; }); 
                    return;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 m_vecMouseDownPos = Input.mousePosition;
            Vector2 pos = Camera.main.ScreenToWorldPoint(m_vecMouseDownPos);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit.collider != null)
            {
                //Debug.Log(hit.transform.gameObject.name);
                RegionNode region = hit.collider.gameObject.GetComponent<RegionNode>();
                if (region != null)
                {
                    if(region.board_State == RegionNode.Board_State.ban)
                    {
                        tutorial.SetTutorialActive("Tutorial-Ban Region");
                        return;
                    }



                    if (region.adj_list.Any(e => { return e.isIn(PlayerIsIn); }))
                    {
                        if(region != PlayerIsIn)
                        {
                            if ((playerMaxTurn - playerTurn) % (dayTurn + nightTurn) < dayTurn)
                            {
                                Debug.Log("지금은 낮입니다");
                                sight = 2;
                            }
                            else
                            {
                                tutorial.SetTutorialActive("Tutorial-Day and Night");
                                Debug.Log("지금은 밤입니다");
                                sight = 1;
                            }

                            PlayerIsIn = region;
                            playerTurn--;

                            if (PlayerIsIn.board_State == RegionNode.Board_State.end)
                            {
                                tutorial.SetTutorialActive("Tutorial-Clear Check");
                                ClearEnterUI.SetActive(true);
                                //if(clear)
                                //ClearEnterUI.~~
                                return;
                            }

                            chaseMonster();
                            if ((playerMaxTurn - playerTurn) % (dayTurn + nightTurn) == dayTurn)
                            {
                                activeMonster();
                            }
                            else if ((playerMaxTurn - playerTurn) % (dayTurn + nightTurn) == 0)
                            {
                                deleteMonster();
                            }
                        }

                        if (region.board_State == RegionNode.Board_State.enter)
                        {
                            tutorial.SetTutorialActive("Tutorial-Research Region");
                            EnterUI.SetActive(true);
                            last_move_scene = _playerIsIn.SceneName;
                        }
                        if (region.board_State == RegionNode.Board_State.heroine)
                        {
                            tutorial.SetTutorialActive("Tutorial-HeroinSelect");
                            EnterUI.SetActive(true);
                            last_move_scene = "SelectHerionStory";
                        }

                    }



                }
            }
        }
    }
    public void activeMonster()
    {
        MonsterInNamesLast.Clear();

        Dictionary<RegionNode, int> adjdic = new Dictionary<RegionNode, int>();
        Dictionary<RegionNode, int> result = new Dictionary<RegionNode, int>();

        foreach (GraphEdgeNode n in PlayerIsIn.adj_list)
        {
            adjdic.Add((RegionNode)n.opposite(PlayerIsIn), 1);
            result.Add((RegionNode)n.opposite(PlayerIsIn), 1);
            //Debug.Log($"{n.opposite(PlayerIsIn).gameObject.name} {1}");
        }

        while (adjdic.Count != 0)
        {
            var first = adjdic.OrderBy(i => i.Value).First();
            adjdic.Remove(first.Key);

            foreach (GraphEdgeNode n in first.Key.adj_list)
            {
                RegionNode temp = (RegionNode)n.opposite(first.Key);
                if (result.ContainsKey(temp))
                {
                    result[temp] = result[temp] > first.Value + 1 ? first.Value + 1 : result[temp];
                }
                else
                {
                    adjdic.Add(temp, first.Value + 1);
                    result.Add(temp, first.Value + 1);
                }
                
            }
        }
        int idx = 0;
        foreach(var v in result.OrderBy(i => i.Value))
        {
            if (v.Key.board_State != RegionNode.Board_State.monster || v.Key == PlayerIsIn)
                continue;

            if (Monsters.Count <= idx || idx >= monsterMaxNum)
                break;
            Monsters[idx].IsActive = true;
            Monsters[idx].origin = v.Key;
            Monsters[idx].here = v.Key;
            idx++;
            
        }


    }
    public void chaseMonster()
    {
        foreach(RegionMonster m in Monsters)
        {
            if (m.IsActive && m.here != null)
                m.chasePlayer();
        }
    }

    public void deleteMonster()
    {
        foreach (RegionMonster m in Monsters)
        {
            m.IsActive = false;
        }
        MonsterInNamesLast.Clear();
    }

    public void createMonster()
    {
        Monsters.Clear();
        for(int i = 0; i < monsterMaxNum; i++)
        {
            GameObject go = Instantiate(Monsterprefab);
            RegionMonster m = go.GetComponent<RegionMonster>();
            m.IsActive = false;
            Monsters.Add(m);
        }
    }

    public void isInSight()
    {
        
        Dictionary<RegionNode, int> adjdic = new Dictionary<RegionNode, int>();
        List<RegionNode> solution = new List<RegionNode>();
        solution.Add(PlayerIsIn);
        

        foreach (GraphEdgeNode n in PlayerIsIn.adj_list)
        {
            adjdic.Add((RegionNode)n.opposite(PlayerIsIn), 1);
            solution.Add((RegionNode)n.opposite(PlayerIsIn));
            //Debug.Log($"{n.opposite(PlayerIsIn).gameObject.name} {1}");
        }

        while (adjdic.Count != 0)
        {
            var first = adjdic.OrderBy(i => i.Value).First();
            adjdic.Remove(first.Key);

            if (first.Value >= sight)
            {
                continue;
            }

            foreach (GraphEdgeNode n in first.Key.adj_list)
            {
                RegionNode temp = (RegionNode)n.opposite(first.Key);
                if (solution.Contains(temp))
                {
                    continue;
                }
                adjdic.Add(temp, first.Value + 1);
                solution.Add(temp);
            }
        }

        foreach(RegionNode n in graph.regions)
        {
            if (!solution.Contains(n))
                n.isOpen = false;
            else
                n.isOpen = true;
        }

    }

    public void ComeBackRegionMap()
    {
        StartCoroutine(CheckInitOverBackToScene());
    }

    public void PlayerDead()
    {
        tutorial.SetTutorialActive("Tutorial-Return");
        playerTurn = playerMaxTurn;
        dead_count++;
        deleteMonster();
    }

    IEnumerator CheckInitOverBackToScene()
    {
        yield return new WaitUntil(() => graph != null);
        yield return new WaitUntil(() => graph.regions.All(value => { return value.initializeOver; }));

        createMonster();
        bool monsteractive = (playerMaxTurn - playerTurn) % (dayTurn + nightTurn) >= dayTurn;

        int idx = -1;
        foreach(string name in MonsterInNamesLast.Keys)
        {
            if (++idx > monsterMaxNum)
                break;
            RegionMonster m = Monsters[idx];
            m.origin = graph.regions.First(value => { return value.gameObject.name == name; });
            m.here = graph.regions.First(value => { return value.gameObject.name == MonsterInNamesLast[m.origin.gameObject.name]; });
            m.IsActive = monsteractive;
        }

        if (PlayerIcon == null) PlayerIcon = GameObject.Find("PlayerIcon");
        PlayerIsIn = graph.regions.First(value => { return value.gameObject.name == PlayerInNameLast; });
        if (Monsterprefab == null) Monsterprefab = Resources.Load<GameObject>("Prefab/Region/MonsterIcon");
        if (tutorial == null) tutorial = GameObject.Find("Canvas").GetComponent<TutorialComponent>();


        if (RegionUI == null)
            Debug.Log("error");
        isInSight();
        isRegionMap = true;
    }

}
