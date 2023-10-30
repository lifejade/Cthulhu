using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyExit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneChanger.instance.ChangeScene("Lobby 2");
            Destroy(this);
        }
    }
}
