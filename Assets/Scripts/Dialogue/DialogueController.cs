using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        public static DialogueController instance;

        public List<DialogueMethods> methodlist;

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
            Managers.PlayerData.syncLoadData = true;
            AudioManager.instance.ChangeBgm("Dialogue/1번_theme_48-24");
            SceneChanger.instance.LoadScene("BloodFilled");
            Methods = DialogueMethodsCp1.instance;
        }

        // Update is called once per frame
        void Update()
        { Methods.ExecutePerFrame(); }

        public void DialogueEnd()
        {
            int chapternum = Methods.chapternum + 1;
            if(!Managers.PlayerData.Clear_MainChapter.ContainsKey(chapternum))
                Managers.PlayerData.Clear_MainChapter.Add(chapternum, true);

            SceneChanger.instance.ChangeScene("Lobby");
        }

        //자주 사용하지 말것
        public void MoveDialogue(int idx)
        {
            Methods.MoveDialogue(idx);
        }

    }
}