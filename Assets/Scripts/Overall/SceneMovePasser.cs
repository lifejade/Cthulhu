using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SceneMovePasser : MonoBehaviour
{
    string last_SceneName = null;
    
    public void recieve(string Scenename)
    {
        last_SceneName = Scenename;
    }

    public void changeScene()
    {
        if (last_SceneName == null)
            return;
        SceneChanger.instance.ChangeScene(last_SceneName);
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
