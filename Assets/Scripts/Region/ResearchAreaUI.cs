using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchAreaUI : MonoBehaviour
{
    [Header("- Move Button")]
    public Button up;
    public Button down;
    public Button left;
    public Button right;

    public Button exit;

    // Start is called before the first frame update
    void Start()
    {
        left.onClick.AddListener(() => { Managers.ResearchArea.pressButton(Arrow.left); checkMoveButtonAvailable(); });
        right.onClick.AddListener(() => { Managers.ResearchArea.pressButton(Arrow.right); checkMoveButtonAvailable(); });
        up.onClick.AddListener(() => { Managers.ResearchArea.pressButton(Arrow.up); checkMoveButtonAvailable(); });
        down.onClick.AddListener(() => { Managers.ResearchArea.pressButton(Arrow.down); checkMoveButtonAvailable(); });
        
        exit.onClick.AddListener(() => {
            SceneChanger.instance.ChangeScene("SubChapter1");
            Managers.Region.ComeBackRegionMap();
        });

        checkMoveButtonAvailable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void checkMoveButtonAvailable()
    {
        if (Managers.ResearchArea.getENodeWithArrow(Arrow.left) == null)
            left.gameObject.SetActive(false);
        else
            left.gameObject.SetActive(true);

        if (Managers.ResearchArea.getENodeWithArrow(Arrow.right) == null)
            right.gameObject.SetActive(false);
        else
            right.gameObject.SetActive(true);

        if (Managers.ResearchArea.getENodeWithArrow(Arrow.up) == null)
            up.gameObject.SetActive(false);
        else
            up.gameObject.SetActive(true);

        if (Managers.ResearchArea.getENodeWithArrow(Arrow.down) == null)
            down.gameObject.SetActive(false);
        else
            down.gameObject.SetActive(true);
    }

    public enum Arrow
    {
        up,
        down,
        left,
        right
    }
}
