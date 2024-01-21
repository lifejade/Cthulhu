using Dialogue;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour, EManager
{
    public bool syncLoadData = false;

    [SerializeField]
    public class PlayerSaveData
    {
        public Dictionary<int, bool> Clear_MainChapter;
        public Dictionary<int, bool> Clear_ResearchChapter;
        public Dictionary<int, bool> HaveItem;
        public List<string> Clear_Tutorial;

        public string lastSceneName;
        
        //if scene == MainStory Scene
        public int lastStoryIdx;
        //if scene == SubStory Scene
        public int lastTurn;
        
    }

    public static PlayerSaveData playerSaveData;


    public Dictionary<int, bool> Clear_MainChapter {
        get
        {
            if (playerSaveData == null) return null; 
            return playerSaveData.Clear_MainChapter;
        }
        set
        {
            if (playerSaveData == null) return;
            playerSaveData.Clear_MainChapter = value;
        }
    }

    public Dictionary<int, bool> Clear_ResearchChapter
    {
        get
        {
            if(playerSaveData == null) return null;
            return playerSaveData.Clear_ResearchChapter;
        }
        set
        {
            if (playerSaveData == null) return;
            playerSaveData.Clear_ResearchChapter = value;
        }
    }
    public Dictionary<int, bool> HaveItem
    {
        get
        {
            if(playerSaveData== null) { return null; }
            return playerSaveData.HaveItem;
        }
        set { if (playerSaveData== null) { return; } 
        playerSaveData.HaveItem = value;
        }
    }
    public List<string> Clear_Tutorial
    {
        get { if(playerSaveData == null) return null;
        return playerSaveData.Clear_Tutorial;}
        set { if(playerSaveData== null) { return; }
        playerSaveData.Clear_Tutorial = value;}
    }
    public string lastSceneName
    {
        get
        {
            if(playerSaveData==null)   return null;
            return playerSaveData.lastSceneName;
        }
        set
        {
            if(playerSaveData == null) return;
            playerSaveData.lastSceneName = value;
        }
    }
    public int lastStoryIdx
    {
        get 
        {
            if(playerSaveData==null) return 0;
            return playerSaveData.lastStoryIdx;
        }
        set
        {
            if (playerSaveData == null) return;
            playerSaveData.lastStoryIdx = value;
        }
    }



    public void init()
    {
        Debug.Log("PlayerDataManager Init");
        if(playerSaveData == null)
            playerSaveData = new PlayerSaveData();

        Clear_MainChapter = new Dictionary<int, bool>();
        Clear_ResearchChapter = new Dictionary<int, bool>();
        HaveItem = new Dictionary<int, bool>();
        Clear_Tutorial = new List<string>();
        lastSceneName = SceneManager.GetActiveScene().name;
    }

    public void update_()
    {
        if (Input.GetKeyDown(KeyCode.T))
            savePlayerData();
        if (Input.GetKeyDown(KeyCode.K))
            loadPlayerData();
    }

    public static PlayerDataManager createInstance()
    {
        GameObject go = GameObject.Find("@PlayerDataManager");
        if (go == null)
        {
            go = new GameObject { name = "@PlayerDataManager" };
            go.transform.parent = Managers.MGRS.transform;
        }

        PlayerDataManager result = go.GetComponent<PlayerDataManager>();
        if (result == null)
            result = go.AddComponent<PlayerDataManager>();
        result.init();
        return result;
    }

    EManager EManager.createInstance()
    {
        return PlayerDataManager.createInstance();
    }

    public void savePlayerData()
    {
        lastSceneName = SceneManager.GetActiveScene().name;
        string str = JsonConvert.SerializeObject(playerSaveData);
        GameManager.instance.CreateJsonFile(Application.persistentDataPath, "autosave", str);
    }

    public void loadPlayerData()
    {
        string str = File.ReadAllText(Application.persistentDataPath + "/autosave.json");
        Debug.Log(str);
        playerSaveData = JsonConvert.DeserializeObject<PlayerDataManager.PlayerSaveData>(str);
        syncLoadData = false;
        SceneManager.LoadScene(lastSceneName, LoadSceneMode.Single);
        if(lastSceneName == "Dialogue")
        {
            StartCoroutine(SyncLoadData(lastStoryIdx));
        }
    }

    IEnumerator SyncLoadData(int lastStoryIdx)
    {
        yield return new WaitUntil(() => !syncLoadData);
        DialogueController.instance.MoveDialogue(playerSaveData.lastStoryIdx);
    }

    public void UnexpectedErr_AutoSave(string condition, string stackTrace, LogType type)
    {
        if(type == LogType.Error || type == LogType.Exception)
        {
            savePlayerData();
        }
    }

    private void Awake()
    {
        Application.logMessageReceived += UnexpectedErr_AutoSave;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= UnexpectedErr_AutoSave;
    }
}
