using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        public static DialogueController instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            AudioManager.instance.ChangeBgm("Dialogue/OminousBGM");
            SceneChanger.instance.LoadScene("BloodFilled");
        }

        // Update is called once per frame
        void Update()
        {
            StartCoroutine( DialogueMethods.instance.ExecutePerFrame() );   
        }


    }
}