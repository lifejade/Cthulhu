using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckClearChapterSeq : MonoBehaviour
{
    //if main = true, sub = false
    public bool isMain;
    public List<GameObject> chapter_blackout;
    private void OnEnable()
    {
        
        Dictionary<int, bool> check;
        int maxCleared = 0;
        if (isMain)
        {
            check = Managers.PlayerData.Clear_MainChapter;
        }
        else
        {
            check = Managers.PlayerData.Clear_ResearchChapter;
        }
        foreach (var it in check)
        {
            if (it.Key > maxCleared && it.Value)
                maxCleared = it.Key;

            if (chapter_blackout.Count > it.Key && chapter_blackout[it.Key] != null)
                chapter_blackout[it.Key - 1].SetActive(!it.Value);
        }
        chapter_blackout[maxCleared].SetActive(false);
    }
}
