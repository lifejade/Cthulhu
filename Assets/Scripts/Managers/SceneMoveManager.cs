using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoveManager : EManager
{
    public void init()
    {


    }

    public void update_()
    {

    }

     EManager EManager.createInstance()
    {
        return SceneMoveManager.createInstance();
    }

    public static SceneMoveManager createInstance()
    {
        SceneMoveManager result = new SceneMoveManager();
        result.init();
        return result;
    }


}
