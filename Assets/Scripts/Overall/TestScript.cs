using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(i%20 == 0){
        if(InputManager.inst.GetInput("space"))
            Debug.Log(1);
        if(InputManager.inst.GetInput("space"))
            Debug.Log(2);
        
        }
        i++;
    }
}
