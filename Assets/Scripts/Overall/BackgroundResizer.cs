using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundResizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    private IEnumerator ShakeCamera()
    {
        Vector2 vec1 = new Vector2(0.1f, 0.1f);
        Vector2 vec2 = new Vector2(-0.1f, -0.1f);
        Vector2 vec3 = new Vector2(0.1f, -0.1f);
        Vector2 vec4 = new Vector2(-0.1f, 0.1f);
        List<Vector2> list = new List<Vector2>();
        list.Add(vec1); list.Add(vec2); list.Add(vec3); list.Add(vec4);
        
        foreach (var moveTo in list)
        {
            Vector2 camP = Camera.current.transform.position;
            while (GameManager.NearlyEqual(camP.x, moveTo.x) && GameManager.NearlyEqual(camP.y, moveTo.y))
            {
                if (camP.x < moveTo.x)
                {
                    camP.x += 0.02f;
                }
                else()
                {
                    camP.x += 0.02f;
                }
                if (camP.y < moveTo.y)
                {
                    camP.y += 0.02f;
                }
                yield return null;
            }
        }
        
    }
        */
}
