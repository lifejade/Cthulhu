using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour, EManager
{

   

    public void init()
    {
        Debug.Log("PlayerDataManager Init");
    }

    public void update_()
    {

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





    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
