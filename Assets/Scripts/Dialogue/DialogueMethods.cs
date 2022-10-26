using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace Dialogue
{
    public class DialogueMethods : MonoBehaviour
    {
        private Dictionary<string, IEnumerator> coroutineDict = new Dictionary<string, IEnumerator>();

        protected DialogueController controller;
        protected float textSpeed = 0.03f;
        protected bool isPrinting = false;
        public bool IsPrinting
        {
            get { return isPrinting; }
            set
            {
                isPrinting = value;
            }
        }
        protected int dialogueLength = 0;
        protected int dialogueIndex = 0;
        protected int dialogueCorCompCount = 0;

        protected DialogueUnits dialogues;

        public static DialogueMethods instance;

        private void Awake()
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


        public void EndOfUnitCor()
        {
            if (dialogues.data[dialogueIndex].CorList.Count <= ++dialogueCorCompCount)
            {
                IsPrinting = false;
                dialogueCorCompCount = 0;
                dialogueIndex++;
            }
        }

        public IEnumerator ExecutePerFrame()
        {
            if (!IsPrinting && InputManager.inst.GetInput("space"))
            {
                IsPrinting = true;
                if (dialogueIndex >= dialogues.data.Count)
                {
                    yield return StartCoroutine(GeneralDialogueCor(new DialogueUnit("대화의 끝")));
                    dialogueIndex = 0;
                }
                else
                {
                    DialogueUnit unit = dialogues.data[dialogueIndex];
                    foreach (var cor in unit.CorList)
                    {
                        if (unit.parallelExe)
                        {
                            StartCoroutine(cor.Invoke(unit));
                        }
                        else
                        {
                            yield return StartCoroutine(cor.Invoke(unit));
                        }
                    }
                }
            }
        }

        public IEnumerator ExecuteEndOfDialoguesCor(DialogueUnit unit)
        {
            yield return null;
        }


        public IEnumerator GeneralDialogueCor(DialogueUnit unit)
        {
            TextMeshPro speakerTMPro = getGObjFromController("SpeakerText").GetComponent<TextMeshPro>();
            TextMeshPro dialogueTMPro = getGObjFromController("DialogueText").GetComponent<TextMeshPro>();
            string speaker = unit.speaker;
            string sentence = unit.sentence;

            speakerTMPro.text = speaker;

            for (int i = 1; i <= sentence.Length; i++)
            {
                string slicedSentence = sentence.Substring(0, i);
                string lastchar = sentence.Substring(i - 1, 1);

                dialogueTMPro.text = slicedSentence;

                if (lastchar == "\n" || lastchar == "," || lastchar == ".")
                    yield return new WaitForSeconds(textSpeed * 5);
                else
                    yield return new WaitForSeconds(textSpeed);


                if (InputManager.inst.GetInput("space"))
                {
                    dialogueTMPro.text = sentence;
                    break;
                }
            }

            SpriteRenderer image = getGObjFromController("DialogueCircle").GetComponent<SpriteRenderer>();
            image.enabled = true;
            IEnumerator coroutine = FadeInAndOut(image);
            coroutineDict.Add("FadeInAndOut", coroutine);

            StartCoroutine(coroutine);
            getGObjFromController("DialogueCircle").GetComponent<SpriteRenderer>().enabled = false;
            if (coroutineDict.ContainsKey("FadeInAndOut"))
            {
                StopCoroutine(coroutineDict["FadeInAndOut"]);
            }
            coroutineDict.Remove("FadeInAndOut");

            this.SendMessage("EndOfUnitCor");
        }

        public IEnumerator disableDialogueCircle(DialogueUnit unit)
        {
            getGObjFromController("DialogueCircle").GetComponent<SpriteRenderer>().enabled = false;
            if (coroutineDict.ContainsKey("FadeInAndOut"))
            {
                StopCoroutine(coroutineDict["FadeInAndOut"]);
                coroutineDict.Remove("FadeInAndOut");
            }

            yield return null;
            this.SendMessage("EndOfUnitCor");
        }

        public IEnumerator enableDialogueCircle(DialogueUnit unit)
        {
            SpriteRenderer image = getGObjFromController("DialogueCircle").GetComponent<SpriteRenderer>();
            image.enabled = true;
            IEnumerator coroutine = FadeInAndOut(image);
            coroutineDict.Add("FadeInAndOut", coroutine);

            StartCoroutine(coroutine);
            yield return null;
            this.SendMessage("EndOfUnitCor");
        }

        public IEnumerator ToggleDialogueWrapperCor(DialogueUnit unit)
        {
            GameObject wrapper = Instantiate(GameManager.LoadPrefab("Dialogue/DialogueWrapper"));
            yield return null;
            controller.gameObjects.Add("SpeakerText", GameObject.Find("SpeakerText"));
            controller.gameObjects.Add("DialogueText", GameObject.Find("DialogueText"));
            controller.gameObjects.Add("DialogueCircle", GameObject.Find("DialogueCircle"));
            yield return null;

            this.SendMessage("EndOfUnitCor");
        }

        public IEnumerator ChangeBackGroundCor(DialogueUnit unit)
        {
            GameObject oldBackgroundImage = GameObject.Find("BackgroundImage(Clone)");
            bool isOldExist = oldBackgroundImage != null;

            if (isOldExist)
            {
                oldBackgroundImage.GetComponent<SpriteRenderer>().sortingLayerName = "OldBackground";
            }
            else
            {
                oldBackgroundImage =
                    Instantiate(GameManager.LoadPrefab("Dialogue/BackgroundImage"), getGObjFromController("BgrWrapper").transform);
                oldBackgroundImage.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                oldBackgroundImage.GetComponent<SpriteRenderer>().sortingLayerName = "OldBackground";
            }

            GameObject backgroundImage = GameManager.LoadPrefab("Dialogue/BackgroundImage");

            //배경이미지 프리팹으로 오브젝트를생성하고 스프라이트 변경
            backgroundImage = Instantiate(backgroundImage, getGObjFromController("BgrWrapper").transform);
            SpriteRenderer image = backgroundImage.GetComponent<SpriteRenderer>();
            image.sprite = GameManager.LoadImage(unit.EtcInfo["BackgroundImage"] as string);
            int oldBackgrIndex =
                isOldExist ?
                oldBackgroundImage.transform.GetSiblingIndex() : -1;
            backgroundImage.transform.SetSiblingIndex(oldBackgrIndex + 1);


            //이전 배경이미지를 페이드아웃 시킨다.
            oldBackgroundImage.GetComponent<SpriteRenderer>().material = GameManager.LoadMaterial("FadeMaterial");
            oldBackgroundImage.GetComponent<SpriteRenderer>().material.SetInteger("_Option", 5);
            Animator animator = oldBackgroundImage.AddComponent<Animator>();
            animator.runtimeAnimatorController = GameManager.LoadResource("Animator/Dialogue/FadeInAndOutImage") as RuntimeAnimatorController;
            animator.SetTrigger("FadeOut");

            yield return null;
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer"));
            float duration = animState.length;
            yield return new WaitForSeconds(duration);
            Destroy(oldBackgroundImage);

            this.SendMessage("EndOfUnitCor");
        }

        public IEnumerator FadeInAndOut(SpriteRenderer element)
        {
            float duration = 1f;
            while (true)
            {
                float deltaTimeDivDur = Time.deltaTime / duration;

                //element�� �������� 0.5�̸��̶��
                if (element.color.a < 1.0f)
                {
                    //element�� �������� 1���� ���̰�
                    while (element.color.a <= 1f)
                    {
                        element.color = new Color(element.color.r, element.color.g, element.color.b, element.color.a + deltaTimeDivDur);
                        yield return new WaitForSeconds(deltaTimeDivDur);
                    }
                }
                //element�� �������� 1�̻��̶��
                else if (element.color.a >= 1.0f)
                {
                    //element�� �������� 0.5���� �����
                    while (element.color.a >= 0.5f)
                    {
                        element.color = new Color(element.color.r, element.color.g, element.color.b, element.color.a - deltaTimeDivDur);
                        yield return new WaitForSeconds(deltaTimeDivDur);
                    }
                }

                yield return new WaitForSeconds(deltaTimeDivDur);
            }
        }

        protected GameObject getGObjFromController(string keyName)
        {
            if (controller.gameObjects.ContainsKey(keyName))
            {
                return controller.gameObjects[keyName];
            }
            throw new Exception("error from get GameObject of controller");
        }
    }
}