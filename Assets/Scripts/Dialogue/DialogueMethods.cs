using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Cinemachine;
using UnityEngine.UI;

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
        protected int dialogueIndex = 0;

        protected DialogueUnits dialogues;
        protected DialogueUnit prsntDialogue;
        protected bool forceToNextDialogue;

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
            /*
            코루틴이 끝날때마다 실행되는 메소드 모든 코루틴들의 실행이 완료되었다면 
            isprinting을 false로 만들고 다음 대화를 불러오기 위해 dialogueIndex를 증가시킨다.
            */
            if (prsntDialogue.CorList.Count == ++prsntDialogue.dialogueCorCompCount)
            {
                IsPrinting = false;
                prsntDialogue.dialogueCorCompCount = 0;
                dialogueIndex++;


                if (forceToNextDialogue)
                {
                    forceToNextDialogue = false;
                    InputManager.inst.PressSpace();
                }
            }
        }

        public IEnumerator ExecutePerFrame()
        {
            /*
            Coroutines that executed every frame
            매 프레임마다 실행되는 코루틴
            현재 출력중이 아니고 스페이스입력이 들어오면 내부 로직 실행
            */
            if (!IsPrinting && InputManager.inst.GetInput("space"))
            {
                IsPrinting = true;

                if (dialogueIndex >= dialogues.data.Count)
                {
                    //대본이 끝났다면
                    prsntDialogue = new DialogueUnit("대화의 끝");
                    foreach (var cor in prsntDialogue.CorList)
                    {
                        if (cor.parallelExe)
                        {
                            StartCoroutine(cor.func.Invoke(prsntDialogue));
                        }
                        else
                        {
                            yield return StartCoroutine(cor.func.Invoke(prsntDialogue));
                        }
                    }
                    dialogueIndex = 1;
                }
                else
                {
                    //대본이 끝나지 않았다면
                    prsntDialogue = dialogues.data[dialogueIndex];
                    foreach (var cor in prsntDialogue.CorList)
                    {
                        if (cor.parallelExe)
                        {
                            StartCoroutine(cor.func.Invoke(prsntDialogue));
                        }
                        else
                        {
                            yield return StartCoroutine(cor.func.Invoke(prsntDialogue));
                        }
                    }
                }
            }
        }

        public IEnumerator PlayAudio(DialogueUnit unit)
        {
            string audioName = (string)unit.EtcInfo["AudioName"];
            if (audioName == null)
            {
                EndOfUnitCor();
                yield break;
            }

            AudioManager.instance.PlayAudio(audioName);
            EndOfUnitCor();
        }

        public IEnumerator GeneralDialogue(DialogueUnit unit)
        {
            //If Dialogue Circle is fading in and out. it stops and disappear.
            Image circle = GameObject.Find("DialogueCircle").GetComponent<Image>();
            if (circle.enabled == true)
            {
                circle.enabled = false;
                if (coroutineDict.ContainsKey("FadeInAndOutCircle"))
                {
                    StopCoroutine(coroutineDict["FadeInAndOutCircle"]);
                    coroutineDict.Remove("FadeInAndOutCircle");
                }
            }
            circle = null;

            TextMeshProUGUI speakerTMPro = GameObject.Find("SpeakerText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dialogueTMPro = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();
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


                if (InputManager.inst.GetInput("space") && !forceToNextDialogue)
                {
                    dialogueTMPro.text = sentence;
                    break;
                }
            }

            EndOfUnitCor();
        }

        public IEnumerator WaitForSec(DialogueUnit unit)
        {
            yield return new WaitForSeconds((float)unit.EtcInfo["WaitForSeconds"]);
            EndOfUnitCor();
        }

        public IEnumerator DisableDialogueCircle(DialogueUnit unit)
        {
            /*
            DialogueCircle 비활성화 코루틴
            DialogueCircle을 깜빡거리게 하는 코루틴을 중지시키고 coroutineDict로부터 반복자를 제거시킨다
            */
            GameObject.Find("DialogueCircle").GetComponent<Image>().enabled = false;
            if (coroutineDict.ContainsKey("FadeInAndOut"))
            {
                StopCoroutine(coroutineDict["FadeInAndOut"]);
                coroutineDict.Remove("FadeInAndOut");
            }

            EndOfUnitCor();
            yield break;
        }

        public IEnumerator EnableDialogueCircle(DialogueUnit unit)
        {
            /*
            DialogueCircle 활성화 코루틴 보통 대화창의 모든 글자가 출력된 후 실행된다.
            
            DialogueCircle을 enable시키고 DialogueCircle을 깜빡거리게 하는 코루틴을 실행 후 
            추후에 코루틴 중지를 위해 코루틴 반복자를 coroutineDict에 저장  
            */
            Image image = GameObject.Find("DialogueCircle").GetComponent<Image>();
            image.enabled = true;
            IEnumerator coroutine = FadeInAndOutImage(image);
            coroutineDict.Add("FadeInAndOutCircle", coroutine);

            StartCoroutine(coroutine);
            EndOfUnitCor();
            yield break;
        }

        public IEnumerator ShakeCamera(DialogueUnit unit)
        {
            /*
            DialogueController의 Component인 CinemachineImpulseSource로 impulse 생성
            */
            CinemachineImpulseSource source = DialogueController.instance.GetComponent<CinemachineImpulseSource>();

            source.GenerateImpulse();

            EndOfUnitCor();
            yield break;
        }

        public IEnumerator ForceToNextDialogue(DialogueUnit unit)
        {
            /*
            This Coroutine should be added before Dialogue Coroutine.
            As Dialogue Coroutine executed, It checks forcetoNextDialogue for whether it could be skip or not
            */
            forceToNextDialogue = true;

            EndOfUnitCor();
            yield break;
        }

        public IEnumerator GenerateCharacter(DialogueUnit unit)
        {
            /*
            필요정보
            CharacterPrefab 프리팹 경로 string
            CharacterPos 캐릭터가 나타날 위치 Vector2

            CharacterPrefab을 생성시키고 CharacterPos로 캐릭터 위치를 이동시킨다.
            */
            Dictionary<string, object> info = unit.EtcInfo;

            if (!info.ContainsKey("CharacterPrefab") || !info.ContainsKey("CharacterPos"))
            {
                throw new Exception("CharacterPrefab or CharacterPos missed on EtcInfo");
            }

            GameObject character = Instantiate(GameManager.LoadPrefab((string)info["CharacterPrefab"]));
            character.transform.position = (Vector2)info["CharacterPos"];
            yield return null;

            EndOfUnitCor();
        }

        public IEnumerator MakeCharacterWalk(DialogueUnit unit)
        {
            /*
            필요정보
            CharacterName : Character GameObject 이름
            int WalkCount : 캐릭터가 몇번 걸을지
            WalkDirection : 걸어갈 방향
            */
            Dictionary<string, object> info = unit.EtcInfo;
            Transform chT = GameObject.Find(info["CharacterName"] + "(Clone)").transform;
            int walkCount = (int)info["WalkCount"];
            for (int i = 0; i < walkCount; i++)
            {
                Vector2[] points =
                                {
                                new Vector2(chT.position.x, chT.position.y),
                                new Vector2(chT.position.x, chT.position.y+1),
                                new Vector2(chT.position.x + 1.5f, chT.position.y+1),
                                new Vector2(chT.position.x + 1.5f, chT.position.y)
                            };

                float t = 0;
                while (t < 1)
                {
                    chT.position = DrawTrajectory(points, t);

                    t += Time.deltaTime;

                    yield return null;
                }

            }

            EndOfUnitCor();
            yield break;
        }

        private Vector2 DrawTrajectory(Vector2[] point, float t)
        {
            if (point.Length > 4)
                throw new Exception("Bezier Exception");

            return
            new Vector2(
                FourPointBezier(point[0].x, point[1].x, point[2].x, point[3].x, t),
                FourPointBezier(point[0].y, point[1].y, point[2].y, point[3].y, t)
            );
        }

        private float FourPointBezier(float a, float b, float c, float d, float t)
        {
            return Mathf.Pow((1 - t), 3) * a
                    + Mathf.Pow((1 - t), 2) * 3 * t * b
                    + Mathf.Pow(t, 2) * 3 * (1 - t) * c
                    + Mathf.Pow(t, 3) * d;
        }

        public IEnumerator EnableDialogueWrapper(DialogueUnit unit)
        {
            string wrapperName = "DialogueWrapperUI";
            if (GameObject.Find(wrapperName) == null)
            {
                GameObject wrapper =
                    Instantiate(
                        GameManager.LoadPrefab("Dialogue/" + wrapperName),
                        GameObject.Find("Canvas").transform
                    );
                wrapper.name = wrapperName;
            }
            EndOfUnitCor();
            yield break;
        }

        public IEnumerator DisableDialogueWrapper(DialogueUnit unit)
        {
            string wrapperName = "DialogueWrapperUI";
            if (GameObject.Find(wrapperName) != null)
            {
                Destroy(GameObject.Find(wrapperName));
            }
            EndOfUnitCor();
            yield break;
        }

        public IEnumerator ChangeBackGround(DialogueUnit unit)
        {
            GameObject oldBackgroundImage = GameObject.Find("BackgroundImage(Clone)");
            GameObject bgrWrapper = GameObject.Find("BgrWrapper");
            bool isOldExist = oldBackgroundImage != null;

            if (isOldExist)
            {
                oldBackgroundImage.GetComponent<SpriteRenderer>().sortingLayerName = "OldBackground";
            }
            else
            {
                oldBackgroundImage =
                    Instantiate(GameManager.LoadPrefab("Dialogue/BackgroundImage"), bgrWrapper.transform);
                oldBackgroundImage.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                oldBackgroundImage.GetComponent<SpriteRenderer>().sortingLayerName = "OldBackground";
            }

            GameObject backgroundImage = GameManager.LoadPrefab("Dialogue/BackgroundImage");

            //배경이미지 프리팹으로 오브젝트를생성하고 스프라이트 변경
            backgroundImage = Instantiate(backgroundImage, bgrWrapper.transform);
            SpriteRenderer image = backgroundImage.GetComponent<SpriteRenderer>();
            image.sprite = GameManager.LoadImage(unit.EtcInfo["BackgroundImage"] as string);
            int oldBackgrIndex =
                isOldExist ?
                oldBackgroundImage.transform.GetSiblingIndex() : -1;
            backgroundImage.transform.SetSiblingIndex(oldBackgrIndex + 1);


            //이전 배경이미지를 페이드아웃 시킨다.
            object backOption = 0;
            oldBackgroundImage.GetComponent<SpriteRenderer>().material = GameManager.LoadMaterial("FadeMaterial");
            unit.EtcInfo.TryGetValue("BackOption", out backOption);
            
            
            // if (backOption != null)
            // {
                oldBackgroundImage.GetComponent<SpriteRenderer>().material.SetInteger("_Option", 5);
                
            // }
            

            Animator animator = oldBackgroundImage.AddComponent<Animator>();
            animator.runtimeAnimatorController = GameManager.LoadResource("Animator/Dialogue/FadeInAndOutImage") as RuntimeAnimatorController;
            animator.SetTrigger("FadeOut");

            yield return null;
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer"));
            float duration = animState.length;
            yield return new WaitForSeconds(duration);
            Destroy(oldBackgroundImage);

            EndOfUnitCor();
            yield break;
        }

        public IEnumerator FadeInAndOut(SpriteRenderer element)
        {
            float duration = 1f;
            while (true)
            {
                float deltaTimeDivDur = Time.deltaTime / duration;

                if (element.color.a < 1.0f)
                {
                    while (element.color.a <= 1f)
                    {
                        element.color = new Color(element.color.r, element.color.g, element.color.b, element.color.a + deltaTimeDivDur);
                        yield return new WaitForSeconds(deltaTimeDivDur);
                    }
                }
                else if (element.color.a >= 1.0f)
                {
                    while (element.color.a >= 0.5f)
                    {
                        element.color = new Color(element.color.r, element.color.g, element.color.b, element.color.a - deltaTimeDivDur);
                        yield return new WaitForSeconds(deltaTimeDivDur);
                    }
                }

                yield return new WaitForSeconds(deltaTimeDivDur);
            }
        }

        public IEnumerator FadeInAndOutImage(Image element)
        {
            float duration = 1f;
            while (true)
            {
                float deltaTimeDivDur = Time.deltaTime / duration;

                if (element.color.a < 1.0f)
                {
                    while (element.color.a <= 1f)
                    {
                        element.color = new Color(element.color.r, element.color.g, element.color.b, element.color.a + deltaTimeDivDur);
                        yield return new WaitForSeconds(deltaTimeDivDur);
                    }
                }
                else if (element.color.a >= 1.0f)
                {
                    while (element.color.a >= 0.5f)
                    {
                        element.color = new Color(element.color.r, element.color.g, element.color.b, element.color.a - deltaTimeDivDur);
                        yield return new WaitForSeconds(deltaTimeDivDur);
                    }
                }

                yield return new WaitForSeconds(deltaTimeDivDur);
            }
        }



    }
}