using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dialogue
{
    public class DialogueUnit
    {
        public float id = 0;
        public string action = "none";
        public string param = "none";
        public string speaker = "none";
        public string sentence = "none";
        private Dictionary<string, object> etcInfo = new();
        public List<UnitCor> corList = new();
        public Dictionary<string, Coroutine> corExecuting = new();
        public List<DialogueUnits> choices = new();
        public DialogueUnits DUnitsOfThis = null;
        //count of completed coroutines 
        public int dialogueCorCompCount = 0;
        protected bool readied = false;
        public bool Readied
        {
            get { return readied; }
            set { readied = value; }
        }
        public List<DialogueUnits> Choices
        {
            get { return choices; }
        }
        public Dictionary<string, object> EtcInfo
        {
            get
            {
                return etcInfo;
            }
            set => etcInfo = value;
        }

        public List<UnitCor> CorList
        {
            get
            {
                return this.corList;
            }
        }

        public DialogueUnit AddCoroutine(Func<DialogueUnit, IEnumerator> corToAdd)
        {
            corList.Add(new UnitCor(this, corToAdd));
            return this;
        }

        public DialogueUnit AddParallelCoroutine(Func<DialogueUnit, IEnumerator> corToAdd)
        {
            corList.Add(new UnitCor(this, corToAdd, true));
            return this;
        }

        public DialogueUnit AddInfo(string key, object obj)
        {
            EtcInfo.Add(key, obj);
            return this;
        }


        public override string ToString()
        {

            string newL = Environment.NewLine;
            StringBuilder sb = new(25);
            sb.Append(newL).Append(id).Append(newL).Append(action).Append(newL).Append(param).Append(newL).Append(speaker).Append(newL).Append(sentence).Append(newL);

            if (choices is not null)
                foreach (var item in choices)
                {
                    sb.Append(item.ToString());
                }

            return sb.ToString();

        }

    }


    //Unit Coroutine Class
    public class UnitCor
    {
        //DialogueUnit that executes this UnitCor
        public DialogueUnit unit;
        //Delegate will contain coroutine to execute
        public Func<DialogueUnit, IEnumerator> cor;
        //Whether to execute in parallel. If it is true, Added another UnitCor after this will execute in parallel.
        public bool parallelExe;

        // public delegate void AfterFunc() = ;
        //Whether to run in parallel, and structure with coroutine reference
        public UnitCor(DialogueUnit unit, Func<DialogueUnit, IEnumerator> cor, bool parallelExe)
        {
            this.unit = unit;
            this.cor = cor;
            this.parallelExe = parallelExe;
        }
        public UnitCor(DialogueUnit unit, Func<DialogueUnit, IEnumerator> cor)
        {
            this.unit = unit;
            this.cor = cor;
            this.parallelExe = false;
        }

        public IEnumerator InvokeCor()
        {
            if (parallelExe)
            {
                DialogueController.instance.StartCoroutine(ParallelAction());
            }
            else
            {
                yield return DialogueController.instance.StartCoroutine(this.cor.Invoke(unit));
                Debug.Log("afterunitcoroutine execute");
                unit.DUnitsOfThis.AfterUnitCoroutine();
            }

            yield break;
        }

        public IEnumerator ParallelAction()
        {
            yield return DialogueController.instance.StartCoroutine(this.cor.Invoke(this.unit));
            unit.DUnitsOfThis.AfterUnitCoroutine();
        }

    }

    public class DialogueUnits
    {
        public string name = "none";
        public string sentence = "none";
        protected float textSpeed = 0.03f;
        protected bool isExecuting = false;
        protected bool forceToNextDialogue;
        public LinkedList<DialogueUnit> uList = null;
        protected LinkedListNode<DialogueUnit> prNode = null;
        protected DialogueUnit prUnit = null;
        public LinkedListNode<DialogueUnit> PrNode
        {
            get { return prNode; }
        }
        public DialogueUnit PrUnit
        {
            get { return prUnit; }
        }

        public float TextSpeed
        {
            get => textSpeed;
            set => textSpeed = value;
        }

        public bool ForceToNextDialogue
        {
            get => forceToNextDialogue;
            set => forceToNextDialogue = value;
        }

        
        public IEnumerator InvokeIfGotSpace()
        {
            if (false)
            {

            }

            if (!isExecuting && InputManager.inst.GetInputNextText())
            {

                if (prNode is null)
                {
                    //Logics after all DialogueUnit completed
                    Debug.Log("DialogueUnits all completed");
                    DialogueController.instance.DialogueEnd();
                }
                else
                {
                    Debug.Log("trued--------");
                    Debug.Log(prUnit);
                    isExecuting = true;
                    //Logics while DialogueUnits not completed
                    //Present DialogueUnit
                    foreach (var cor in prUnit.CorList)
                    {
                        yield return DialogueController.instance.StartCoroutine(cor.InvokeCor());
                    }
                    Debug.Log("all coroutine completed");
                    Debug.Log("isExecuting : " + isExecuting);//왜 4번끝나고 falsed
                }
            }
            yield break;
        }


        public void AfterUnitCoroutine()
        {
            /*
            Check all Coroutines of DialogoueUnit completed.
            if it is move on to next DialogueUnit
            */
            //present DialogueUnit
            Debug.Log("--------------");
            Debug.Log(prUnit);
            Debug.Log(prUnit.CorList.Count);
            Debug.Log(prUnit.dialogueCorCompCount);
            Debug.Log("--------------");
            if (prUnit.CorList.Count == ++prUnit.dialogueCorCompCount)
            {//왜 id 4번 falsed 출력안됬는가
                this.isExecuting = false;
                // Debug.Log("falsed");
                // Debug.Log(this.isExecuting);
                prUnit.dialogueCorCompCount = 0;
                prNode = prNode.Next;
                if (prNode is not null)
                    prUnit = prNode.Value;
                else
                    prUnit = null;

                if (forceToNextDialogue)
                {
                    forceToNextDialogue = false;
                    InputManager.inst.PressSpace();
                }
            }
        }


        public void GetReadyToUse(Dictionary<string, Func<DialogueUnit, IEnumerator>> cDictToUse)
        {
            /*
            cDictToUse : coroutine dictionary to use in this DialogueUnits.
            */
            if (prNode is null)
            {
                prNode = uList.First;
                prUnit = prNode.Value;
            }
            foreach (DialogueUnit unit in uList)
            {
                if (unit.Readied)
                    continue;
                unit.DUnitsOfThis = this;
                unit.Readied = true;
                string[] splitedSentence = unit.sentence.Split(" : ");
                if (splitedSentence.Length >= 2)
                {
                    unit.speaker = splitedSentence[0];
                    unit.sentence = splitedSentence[1];
                }

                string actionString = unit.action;
                actionString = actionString.Replace(" ", "");
                string[] actionArr = actionString.Split("|");

                string paramString = unit.param;
                paramString = paramString.Replace(" ", "");
                string[] paramArr = paramString.Split("|");

                foreach (string action in actionArr)
                {
                    if (action == "")
                    {
                        unit.AddCoroutine(cDictToUse["GeneralDialogue"]).AddCoroutine(cDictToUse["EnableDialogueCircle"]);
                        continue;
                    }
                    if (action.Contains("%"))
                    {
                        unit.AddParallelCoroutine(cDictToUse[action.Substring(1)]);
                    }
                    else
                    {
                        unit.AddCoroutine(cDictToUse[action]);
                    }
                }

                foreach (string param in paramArr)
                {
                    if (param == "")
                    {
                        break;
                    }
                    string[] splited = param.Split(":");
                    string key = splited[0];
                    string type = splited[1];
                    string value = splited[2];
                    switch (type)
                    {
                        case "string":
                            unit.AddInfo(key, value);
                            break;
                        case "float":
                            unit.AddInfo(key, float.Parse(value));
                            break;
                        case "ControlCharacter":
                            string[] splitedControl = value.Split(",");
                            if (splitedControl.Length != 5 && splitedControl.Length != 6)
                                throw new Exception("ControlCharacter param is not valid form");

                            string name = (string)splitedControl[0];
                            string emotion = (string)splitedControl[1];
                            string voice = (string)splitedControl[2];
                            Vector2 position =
                            new(float.Parse(splitedControl[3]), float.Parse(splitedControl[4]));

                            ControlCharacter control = null;
                            if (splitedControl.Length == 5)
                            {
                                control = new ControlCharacter(name, emotion, voice, position);
                            }
                            else if (splitedControl.Length == 6)
                            {
                                if (!Boolean.TryParse(splitedControl[5], out bool temp))
                                {
                                    throw new Exception("ControlCharacter param is not valid form");
                                }
                                control = new ControlCharacter(name, emotion, voice, position, temp);
                            }
                            unit.AddInfo(key, control);
                            break;
                        default:
                            throw new Exception("param type exception. " + type + " is not valid type");
                    }
                }

            }

            return;
        }


        public override string ToString()
        {
            string newL = Environment.NewLine;
            StringBuilder sb = new(25);
            sb.Append(newL).Append(name).Append(newL).Append(sentence).Append(newL);
            foreach (var item in uList)
            {
                sb.Append(item.ToString());
            }
            return sb.ToString();
        }
    }
}