using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : EManager
{
    public void init()
    {
        Debug.Log("DataManager Init");
    }
    public void update_()
    {

    }

    public static DataManager createInstance()
    {
        DataManager result = new DataManager();
        result.init();
        return result;
    }

    EManager EManager.createInstance()
    {
        return createInstance();
    }
}
