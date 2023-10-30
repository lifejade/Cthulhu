using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckClearChapter1 : MonoBehaviour
{
    public GameObject main;
    public GameObject reserch;

    public GameObject tutorial;

    // Start is called before the first frame update
    void Start()
    {
        if (Managers.PlayerData.Clear_MainChapter.ContainsKey(1) && Managers.PlayerData.Clear_MainChapter[1])
        {
            main.SetActive(false); 
            reserch.SetActive(false);


            if (Managers.PlayerData.Clear_Tutorial.Contains(tutorial.name))
            {
                tutorial.SetActive(false);
            }
            else
            {
                tutorial.SetActive(true);
                Managers.PlayerData.Clear_Tutorial.Add(tutorial.name);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
