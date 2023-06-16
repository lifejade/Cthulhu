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
        public string action;
        public string param;
        public string sentence;
        private Dictionary<string, object> etcInfo;
        public List<CorStruct> corList;
        public int dialogueCorCompCount = 0;
        public bool parallelExe = false;
        //Whether to execute coroutines in parallel. If parallel execution is true, registered coroutines are executed at once.
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

        public struct CorStruct{
            //Whether to run in parallel, and structure with coroutine reference
            public CorStruct(Func<DialogueUnit, IEnumerator> func, bool parallelExe){
                this.func = func;
                this.parallelExe = parallelExe;
            }
            public CorStruct(Func<DialogueUnit, IEnumerator> func){
                this.func = func;
                this.parallelExe = false;
            }
            public Func<DialogueUnit, IEnumerator> func;
            public bool parallelExe;
        }
        public List<CorStruct> CorList
        {
            get
                {
                    if(this.corList == null){
                        corList = new List<CorStruct>();
                        this.AddCoroutine(DialogueMethods.instance.DisableDialogueCircle)
                        .AddCoroutine(DialogueMethods.instance.GeneralDialogue)
                        .AddCoroutine(DialogueMethods.instance.EnableDialogueCircle);

                    }
                    return this.corList;
                }
        }

        public DialogueUnit(string speaker, string sentence)
        {
            this.speaker = speaker;
            this.sentence = sentence;
        }
        public DialogueUnit(string sentence)
        {
            this.speaker = "";
            this.sentence = sentence;
        }

        public DialogueUnit AddCoroutine(Func<DialogueUnit, IEnumerator> corToAdd)
        {
            if(corList == null)
                corList = new List<CorStruct>();
            corList.Add(new CorStruct(corToAdd, parallelExe));
            return this;
        }

        public DialogueUnit AddInfo(string key, object obj){
            EtcInfo.Add(key, obj);
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