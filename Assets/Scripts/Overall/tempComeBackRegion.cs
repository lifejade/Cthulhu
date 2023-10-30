using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempComeBackRegion : MonoBehaviour
{

    public void temp(){
        SceneChanger.instance.ChangeScene("SubChapter1");
        Managers.Region.ComeBackRegionMap();
    }
}
