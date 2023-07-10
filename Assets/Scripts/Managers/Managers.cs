using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _mgrs = null;
    public static Managers MGRS { get { if (_mgrs == null) init(); return _mgrs; }}

    private static DataManager _data = null;
    public static DataManager Data {  get { if (_data == null) _data = DataManager.createInstance(); return _data; } }
    private static UIManager _ui = null;
    public static UIManager UI { get { if (_ui == null) _ui = UIManager.createInstance(); return _ui; } }
    private static PlayerDataManager _playerdata = null;
    public static PlayerDataManager PlayerData { get { if (_playerdata == null) _playerdata = PlayerDataManager.createInstance(); return _playerdata; } }
    private static RegionManager _region = null;
    public static RegionManager Region { get { if (_region == null) _region = RegionManager.createInstance(); return _region; } }
    public static ResearchAreaManager ResearchArea { get { if (_researchArea == null) _researchArea = ResearchAreaManager.createInstance(); return _researchArea; } }
    private static ResearchAreaManager _researchArea = null;
    public static SceneMoveManager SceneMove { get { if (_scenemove == null) _scenemove = SceneMoveManager.createInstance(); return _scenemove; } }
    private static SceneMoveManager _scenemove = null;


    static void init()
    {


        if(_mgrs == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            _mgrs = go.GetComponent<Managers>();
            DontDestroyOnLoad(go);
        }

        if(_data == null)
            _data = DataManager.createInstance();
        if(_ui == null)
            _ui = UIManager.createInstance();
        if(_playerdata == null)
            _playerdata = PlayerDataManager.createInstance();
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (_mgrs != null)
            Destroy(gameObject);

        init();
    }

    // Update is called once per frame
    void Update()
    {
        Data.update_();
        UI.update_();
        PlayerData.update_();
    }
}
