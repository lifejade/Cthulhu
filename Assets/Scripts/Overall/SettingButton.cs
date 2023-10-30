using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingButton : MonoBehaviour
{
    bool isactive = false;

    public GameObject setting1;
    public GameObject setting2;
    public GameObject setting3;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonClick()
    {
        isactive = !isactive;


        setting1.SetActive(isactive);
        setting2.SetActive(isactive);
        setting3.SetActive(isactive);
    }
}
