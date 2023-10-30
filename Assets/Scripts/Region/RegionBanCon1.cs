using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RegionBanCon1 : RegionBanCondition
{
    public RegionNode n1;
    public RegionNode n2;
    public List<Func<IEnumerator>> corList;

    [SerializeField]
    public Dictionary<RegionNode, Func<IEnumerator>> BanConDic= new Dictionary<RegionNode, Func<IEnumerator>>();
    private TutorialComponent tutorial;


    delegate void check();
    check check_update;

    // Start is called before the first frame update
    void Start()
    {
        corList = new List<Func<IEnumerator>>();
        corList.Add(BanCheckMethode1);
        if (tutorial == null) tutorial = GameObject.Find("Canvas").GetComponent<TutorialComponent>();

        check_update += check1;
        check_update += check2;
    }

    // Update is called once per frame
    void Update()
    {
        if(check_update!= null)
            check_update();
    }

    public void check1()
    {
        if(Managers.PlayerData.HaveItem.ContainsKey(2004) && Managers.PlayerData.HaveItem[2004])
        {
            tutorial.SetTutorialActive("Tutorial-Item");
            n1.board_State = RegionNode.Board_State.normal;
            check_update -= check1;
        }
    }

    public void check2()
    {
        if(Managers.Region.dead_count >= 1)
        {
            n2.board_State = RegionNode.Board_State.normal;
            check_update -= check2;
        }
    }



    public IEnumerator BanCheckMethode1()
    {

        yield return null;
    }
}
