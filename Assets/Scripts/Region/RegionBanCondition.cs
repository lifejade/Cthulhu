using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RegionBanCondition : MonoBehaviour
{
    Dictionary<RegionNode,IEnumerator> corDic= new Dictionary<RegionNode,IEnumerator>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDicOnEditorMode(RegionNode[] arr)
    {
        foreach(RegionNode node in arr) {
            corDic.TryAdd(node, null);
        }
    }
}