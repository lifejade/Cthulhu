using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundResizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Resize();

    }

    void Resize()
    {
        StartCoroutine(test2());

        Sprite sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        float spriteWidth = sprite.bounds.size.x * 100;
        float spriteHeight = sprite.bounds.size.y * 100;
        GameManager m = GameManager.instance;
        float xScale =
             m.matchToWidth ?
            m.cameraWidth / spriteWidth :
             ((m.cameraHeight / m.targetScreenHeightRatio) * m.targetScreenWidthRatio) / spriteWidth;
         float yScale =
             !m.matchToWidth ?
             m.cameraHeight / spriteHeight :
             ((m.cameraWidth / m.targetScreenWidthRatio) * m.targetScreenHeightRatio) / spriteHeight;

         gameObject.transform.localScale = new Vector2(xScale * 1.05f, yScale * 1.05f);

    }

    public void test()
    {
        GameManager.instance.screenBottomLeftInWorld = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        GameManager.instance.screenTopRightInWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        GameManager.instance.cameraWidth =
        (GameManager.instance.screenTopRightInWorld.x - GameManager.instance.screenBottomLeftInWorld.x) * 100;
        GameManager.instance.cameraHeight =
        (GameManager.instance.screenTopRightInWorld.y - GameManager.instance.screenBottomLeftInWorld.y) * 100;

        Resolution curResol = Screen.currentResolution;

        Debug.Log("resolWidth" + curResol.width);
        Debug.Log("\n");
        Debug.Log("resolHeight" + curResol.height);
        Debug.Log("\n");
        Debug.Log("screenwidth" + Screen.width);
        Debug.Log("\n");
        Debug.Log("screenheight" + Screen.height);
        Debug.Log("\n");
        Debug.Log("camerawidth" + GameManager.instance.cameraWidth);
        Debug.Log("\n");
        Debug.Log("cameraheight" + GameManager.instance.cameraHeight);
        Debug.Log("\n");
        return;
    }

    public IEnumerator test2(){
        while (true)
        {
            test();
            yield return new WaitForSeconds(5f);
        }
    }
}
