using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Dialogue
{
    public delegate void Process();

    [Serializable]
    public class DialogueUnit
    {
        public string speaker;
        public string sentence;
        
        // public UnityAction<DialogueUnit> action;
        // public UnityAction<DialogueUnit> Action 
        // { 
        //     get { return action; } 
        //     set => action = value; 
        // }
        public List<Func<DialogueUnit, IEnumerator>> corList = new List<Func<DialogueUnit, IEnumerator>>();
        public List<Func<DialogueUnit, IEnumerator>> CorList 
        {
            get
                {
                    if(this.corList == null){
                        corList = new List<Func<DialogueUnit, IEnumerator>>();
                        this.AddCoroutine(DialogueMethods.instance.disableDialogueCircle)
                        .AddCoroutine(DialogueMethods.instance.GeneralDialogueCor)
                        .AddCoroutine(DialogueMethods.instance.enableDialogueCircle);

                    }
                    return this.corList;
                }
        }
        private Dictionary<string, object> etcInfo;
        public Dictionary<string, object> EtcInfo 
        { 
            get 
            {
                if (etcInfo == null)
                    etcInfo = new Dictionary<string, object>();
                return etcInfo;
            }
            set => etcInfo = value; 
        }
        //코루틴 병렬 실행 여부다. 병렬실행이 true라면 등록된 코루틴들이 한번에 실행되도록 한다.
        public bool parallelExe = false;


        public DialogueUnit(string speaker, string sentence)
        {
            this.speaker = speaker;
            this.sentence = sentence;
            this.corList = new List<Func<DialogueUnit, IEnumerator>>();
        }
        public DialogueUnit(string sentence)
        {
            this.speaker = "";
            this.sentence = sentence;
            this.corList = new List<Func<DialogueUnit, IEnumerator>>();
        }

        public DialogueUnit AddCoroutine(Func<DialogueUnit, IEnumerator> corToAdd)
        {
            if(corList == null)
                corList = new List<Func<DialogueUnit, IEnumerator>>();
            corList.Add(corToAdd);
            return this;
        }

        public DialogueUnit SwitchMode(){
            parallelExe = !parallelExe;
            return this;
        }
    }

    [Serializable]
    public class DialogueUnits
    {
        public List<DialogueUnit> data;
    }
}