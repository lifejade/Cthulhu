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
}
