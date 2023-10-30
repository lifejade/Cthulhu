using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewButton : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text contents;
    public Button Next;

    private Button[] contentButtons;
    private RectTransform content;
    private string toMoveStage;

    string[][] stage;
    // Start is called before the first frame update
    void Start()
    {
        content = GetComponent<ScrollRect>().content;

        //json loading
        stage = new string[][] { new string[] { "탐색 스토리 #1", "서은설을 죽인다...", "SubChapter1" }, 
            new string[] { "탐색 스토리 #2", "서은설을 두번 죽인다...", "SubChapter2" },
            new string[] { "탐색 스토리 #3", "서은설을 많이 죽인다...", "SubChapter3" }
        };
        GameObject prefab = Resources.Load<GameObject>("Prefab/Region/StoryButton");
        //end
        for (int i = 0; i < stage.Length; i++)
        {
            GameObject go = Instantiate(prefab,Vector3.zero ,Quaternion.identity,content);
            go.transform.localScale = Vector3.one;
            go.name = stage[i][0];
            go.GetComponentInChildren<TMP_Text>().text = stage[i][0];

            int temp = i;
            go.GetComponent<Button>().onClick.AddListener(() => {
                
                Title.text = stage[temp][0];
                contents.text = stage[temp][1];
                toMoveStage = stage[temp][2];
            });
        }
        Title.text = stage[0][0];
        contents.text = stage[0][1];
        toMoveStage = stage[0][2];


        Next.onClick.AddListener(() =>
        {
            SceneChanger.instance.ChangeScene(toMoveStage);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}