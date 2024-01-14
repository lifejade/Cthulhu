using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        public static DialogueController instance;
        public DialogueMethods Methods;
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
            AudioManager.instance.ChangeBgm("Dialogue/1번_theme_48-24");
            SceneChanger.instance.LoadScene("BloodFilled");
            Methods = DialogueMethodsCp1.instance;
        }

        // Update is called once per frame
        void Update()
        { Methods.ExecutePerFrame(); }

        public void DialogueEnd()
        {
            if(!Managers.PlayerData.Clear_MainChapter.ContainsKey(1))
                Managers.PlayerData.Clear_MainChapter.Add(1, true);

            SceneChanger.instance.ChangeScene("Lobby");
        }

    }
}