using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        public static DialogueController instance;

        public Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                gameObjects.Add("Canvas", GameObject.Find("Canvas"));
                gameObjects.Add("BgrWrapper", GameObject.Find("BackgroundWrapper"));
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            BGMManager.instance.ChangeBgm("pressure");
            // SceneChanger.instance.loadScene("BloodFilled");
        }

        // Update is called once per frame
        void Update()
        {
            StartCoroutine( DialogueMethods.instance.ExecutePerFrame() );   
        }


    }
}