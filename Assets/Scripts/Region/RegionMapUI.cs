using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegionMapUI : MonoBehaviour
{
    public GameObject SceneMoveParents;
    public TMP_Text restTurn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updaterestTurn(int var)
    {
        restTurn.text = "≥≤¿∫ ≈œ : " + var;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
