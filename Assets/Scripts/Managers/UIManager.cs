using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : EManager
{
    public void init()
    {
        Debug.Log("UIManager Init");
    }
    public void update_()
    {

    }

    public static UIManager createInstance()
    {
        UIManager result = new UIManager();
        result.init();
        return result;
    }

    EManager EManager.createInstance()
    {
        return createInstance();
    }
}
