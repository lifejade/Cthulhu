using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class DialogueMethodsCp1 : DialogueMethods
    {
        // Start is called before the first frame update
        private void Start()
        {
            controller = DialogueController.instance;

            dialogues = GameManager.instance.LoadJsonFile<DialogueUnits>("Assets/Resources/Dialogues", "dialogue1");
            
            RegisterActions();
        }
        
        // Update is called once per frame
        void Update()
        {

        }

        
        private void RegisterActions()
        {
            List<DialogueUnit> list = dialogues.data;
            list[0]
                .AddCoroutine(ToggleDialogueWrapperCor)
                .AddCoroutine(disableDialogueCircle)
                .AddCoroutine(GeneralDialogueCor)
                .AddCoroutine(enableDialogueCircle);

            list[1]
                .AddCoroutine(disableDialogueCircle)
                .AddCoroutine(ChangeBackGroundCor)
                .AddCoroutine(GeneralDialogueCor)
                .AddCoroutine(enableDialogueCircle)
                .EtcInfo.Add("BackgroundImage", "DialogueExample");

            list[4]
                .AddCoroutine(disableDialogueCircle)
                .AddCoroutine(ChangeBackGroundCor)
                .AddCoroutine(GeneralDialogueCor)
                .AddCoroutine(enableDialogueCircle)
                .EtcInfo.Add("BackgroundImage","School");

            list[10]
                .SwitchMode()
                .AddCoroutine(disableDialogueCircle)
                .AddCoroutine(ChangeBackGroundCor)
                .AddCoroutine(GeneralDialogueCor)
                .EtcInfo.Add("BackgroundImage", "BloodFilled/FadeOut_00111");
        }
    }

}
