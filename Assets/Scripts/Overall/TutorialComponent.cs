using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialComponent : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> tutorials;


    public Dictionary<string, GameObject> tutorialDic;


    // Start is called before the first frame update
    void Start()
    {
        tutorialDic = new Dictionary<string, GameObject>();
        foreach (GameObject g in tutorials)
        {
            g.SetActive(false);
            if (!Managers.PlayerData.Clear_Tutorial.Contains(g.name) && !tutorialDic.ContainsKey(g.name))
            {
                tutorialDic.Add(g.name, g);
            }
        }
    }

    public void SetTutorialActive(string name)
    {
        if (!tutorialDic.ContainsKey(name) || Managers.PlayerData.Clear_Tutorial.Contains(name))
            return;

        GameObject go = tutorialDic[name];
        go.SetActive(true);
        Button finish = go.transform.Find("Last").GetChild(0).gameObject.GetComponent<Button>();
        finish.onClick.AddListener(() => { Managers.PlayerData.Clear_Tutorial.Add(name); go.SetActive(false); });


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
