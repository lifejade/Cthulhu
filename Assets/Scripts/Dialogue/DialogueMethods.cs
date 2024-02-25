using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using System.Linq;

namespace Dialogue
{
    public class DialogueMethods : MonoBehaviour
    {
       
        //Dictionary to save a coroutines that might be stopped
        private Dictionary<string, IEnumerator> coroutineDict = new Dictionary<string, IEnumerator>();

        protected DialogueController controller;
        protected bool isDWrapperEnabled = false;
        
        public Dictionary<string, Func<DialogueUnit, IEnumerator>> corDict = new Dictionary<string, Func<DialogueUnit, IEnumerator>>();

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

        protected void JsonToDicts(string name)
        {
            DialogueParsing parsing = new DialogueParsing();
            parsing.stringTalk = JsonConvert.DeserializeObject<DialogueParsing>(GameManager.LoadResource<TextAsset>("Dialogues/" + "StringTalk").text).stringTalk;
            parsing.select = JsonConvert.DeserializeObject<DialogueParsing>(GameManager.LoadResource<TextAsset>("Dialogues/" + "Select").text).select;
            parsing.graphicResources = JsonConvert.DeserializeObject<DialogueParsing>(GameManager.LoadResource<TextAsset>("Dialogues/" + "GraphicResources").text).graphicResources;
            parsing.soundResource = JsonConvert.DeserializeObject<DialogueParsing>(GameManager.LoadResource<TextAsset>("Dialogues/" + "SoundResources").text).soundResource;
            parsing.localTable = JsonConvert.DeserializeObject<DialogueParsing>(GameManager.LoadResource<TextAsset>("Dialogues/" + "LocalTable").text).localTable;


            dialogues = new DialogueUnits();

            dialogues.name = name;
            dialogues.uList = new LinkedList<DialogueUnit>();
            dialogues.TextSpeed = GameManager.instance.TextSpeed;

            foreach (DialogueParsing.StringTalk it in parsing.stringTalk)
            {
                DialogueUnit unit = new DialogueUnit();
                unit.id = it.id;
                unit.action = it.action;
                unit.param = it.param;

                //sentence
                if(it.script != "")
                {
                    DialogueParsing.LocalTable localTable = parsing.localTable.Find(value => value.key == it.script);
                    //TODO : 만일 한국어, 경우 넣을것
                    if(localTable != null)
                        unit.sentence = localTable.ko;
                }

                //speaker
                if (it.speaker != "")
                {
                    DialogueParsing.LocalTable localTable = parsing.localTable.Find(value => value.key == it.speaker);
                    //TODO : 만일 한국어, 경우 넣을것
                    if (localTable != null)
                        unit.speaker = localTable.ko;
                }
                else
                    unit.speaker = it.speaker;

                //cg
                if(it.cg1 != "")
                {
                    DialogueParsing.GraphicResources path = parsing.graphicResources.Find(value => value.key == it.cg1);
                    if (path != null)
                    {
                        if (unit.action != "")
                            unit.action += " | ";
                        else
                            unit.action += "GeneralDialogue | EnableDialogueCircle |";

                        unit.action += "%CG1";
                        if (unit.param != "")
                            unit.param += " | ";

                        unit.param += $"CG1 : ControlCharacter : {path.image_path}, {""}, {""}, -4, -1, true";
                    }
                }

                if (it.cg2 != "")
                {
                    DialogueParsing.GraphicResources path = parsing.graphicResources.Find(value => value.key == it.cg2);
                    if (path != null)
                    {
                        if (unit.action != "")
                            unit.action += " | ";
                        else
                            unit.action += "GeneralDialogue | EnableDialogueCircle |";

                        unit.action += "%CG2";
                        if (unit.param != "")
                            unit.param += " | ";
                        unit.param += $"CG2 : ControlCharacter : {path.image_path}, {""},{""}, 0, -1, true";
                    }
                }

                if (it.cg3 != "")
                {
                    DialogueParsing.GraphicResources path = parsing.graphicResources.Find(value => value.key == it.cg3);
                    if (path != null)
                    {
                        if (unit.action != "")
                            unit.action += " | ";
                        else
                            unit.action += "GeneralDialogue | EnableDialogueCircle |";
                        unit.action += "%CG3";
                        if (unit.param != "")
                            unit.param += " | ";
                        unit.param += $"CG3 : ControlCharacter : {path.image_path}, {""}, {""}, 4, -1, true";
                    }
                }

                if(it.sound1 != "")
                {
                    DialogueParsing.SoundResource path = parsing.soundResource.Find(value => value.key == it.sound1);
                    if(path != null)
                    {
                        if (unit.action != "")
                            unit.action += " | ";
                        else
                            unit.action += "GeneralDialogue | EnableDialogueCircle |";
                        unit.action += "Sound1";
                        if (unit.param != "")
                            unit.param += " | ";
                        unit.param += $"Sound1P : string : {path.sound_path}";
                    }
                }

                if(it.sound2 != "")
                {
                    DialogueParsing.SoundResource path = parsing.soundResource.Find(value => value.key == it.sound2);
                    if (path != null)
                    {
                        if (unit.action != "")
                            unit.action += " | ";
                        else
                            unit.action += "GeneralDialogue | EnableDialogueCircle |";
                        unit.action += "Sound2";
                        if (unit.param != "")
                            unit.param += " | ";
                        unit.param += $"Sound2P : string : {path.sound_path}";
                    }
                }


                dialogues.uList.AddLast(unit);
            }
        }

        protected class DialogueParsing
        {
            [JsonProperty("StringTalk")]
            public List<StringTalk> stringTalk;

            [JsonProperty("SoundResources")]
            public List<SoundResource> soundResource;

            [JsonProperty("Select")]
            public List<Select> select;

            [JsonProperty("LocalTable")]
            public List<LocalTable> localTable;

            [JsonProperty("GraphicResources")]
            public List<GraphicResources> graphicResources;


            public class StringTalk
            {
                public int id;
                public string cg1;
                public string cg2;
                public string cg3;
                
                public string sound1;
                public string sound2;

                public string action;
                public string param;
                public string select;
                public string speaker;
                public string script;
            }
            public class SoundResource
            {
                public string key;
                public string sound_path;
            }
            public class Select
            {
                public string key;
                public string select_1;
                public string select_2;
                public string select_3;
                public string select_1_script;
                public string select_2_script;
                public string select_3_script;
            }
            public class LocalTable
            {
                public string key;
                public string ko;
                public string en;
            }
            public class GraphicResources
            {
                public string key;
                public string image_path;
            }
        }


        public void ExecutePerFrame()
        {
            StartCoroutine(dialogues.InvokeIfGotSpace());
        }

        public void MoveDialogue(int idx)
        {
            dialogues.SetDialogue(idx);
        }

        public IEnumerator PlayAudio(DialogueUnit unit)
        {
            string audioName = (string)unit.EtcInfo["AudioName"];
            if (audioName == null)
            {
                yield break;
            }

            AudioManager.instance.PlayAudio(audioName);

            yield break;
        }

        //
        public IEnumerator Sound1(DialogueUnit unit)
        {
            string audioName = (string)unit.EtcInfo["Sound1P"];
            if (audioName == null)
            {
                yield break;
            }

            AudioManager.instance.PlayVoice(audioName);

            yield break;
        }
        public IEnumerator Sound2(DialogueUnit unit)
        {
            string audioName = (string)unit.EtcInfo["Sound2P"];
            if (audioName == null)
            {
                yield break;
            }

            AudioManager.instance.PlayVoice(audioName);

            yield break;
        }

        public IEnumerator CG1(DialogueUnit unit)
        {
            ControlCharacter control = (ControlCharacter)unit.EtcInfo["CG1"];


            GameObject character;
            character = GameObject.Find(control.name);
            if (character == null)
            {
                float appearSpeed = Time.deltaTime * 3;
                character = Instantiate(GameManager.LoadPrefab("Character"), control.position, new Quaternion(), GameObject.Find("BgrWrapper").transform);
                character.GetComponent<SpriteRenderer>().sprite = GameManager.LoadGraphicResources(control.name);
                character.name = control.name;

                SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();
                sprite.flipX = control.flip;
                Color color = sprite.color;

                float f = 0;
                while (f < 1)
                {
                    sprite.color = new Color(color.r, color.g, color.b, f);
                    f += appearSpeed;
                    yield return null;
                }
            }
            else
            {
                character.transform.position = control.position;
                SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();
                sprite.sprite = GameManager.LoadGraphicResources(control.name);
                sprite.flipX = control.flip;
            }

            yield break;
        }
        public IEnumerator CG2(DialogueUnit unit)
        {
            ControlCharacter control = (ControlCharacter)unit.EtcInfo["CG2"];


            GameObject character;
            character = GameObject.Find(control.name);
            if (character == null)
            {
                float appearSpeed = Time.deltaTime * 3;
                character = Instantiate(GameManager.LoadPrefab("Character"), control.position, new Quaternion(), GameObject.Find("BgrWrapper").transform);
                character.GetComponent<SpriteRenderer>().sprite =
                GameManager.LoadGraphicResources(control.name);
                character.name = control.name;

                SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();
                sprite.flipX = control.flip;
                Color color = sprite.color;

                float f = 0;
                while (f < 1)
                {
                    sprite.color = new Color(color.r, color.g, color.b, f);
                    f += appearSpeed;
                    yield return null;
                }
            }
            else
            {
                character.transform.position = control.position;
                SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();
                sprite.sprite = GameManager.LoadGraphicResources(control.name);
                sprite.flipX = control.flip;
            }

            yield break;
        }
        public IEnumerator CG3(DialogueUnit unit)
        {
            ControlCharacter control = (ControlCharacter)unit.EtcInfo["CG3"];


            GameObject character;
            character = GameObject.Find(control.name);
            if (character == null)
            {
                float appearSpeed = Time.deltaTime * 3;
                character = Instantiate(GameManager.LoadPrefab("Character"), control.position, new Quaternion(), GameObject.Find("BgrWrapper").transform);
                character.GetComponent<SpriteRenderer>().sprite =
                GameManager.LoadGraphicResources(control.name);
                character.name = control.name;

                SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();
                sprite.flipX = control.flip;
                Color color = sprite.color;

                float f = 0;
                while (f < 1)
                {
                    sprite.color = new Color(color.r, color.g, color.b, f);
                    f += appearSpeed;
                    yield return null;
                }
            }
            else
            {
                character.transform.position = control.position;
                SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();
                sprite.sprite = GameManager.LoadGraphicResources(control.name);
                sprite.flipX = control.flip;
            }

            yield break;
        }



        public IEnumerator GeneralDialogue(DialogueUnit unit)
        {
            if (isDWrapperEnabled == false)
            {
                string wrapperName = "DialogueWrapperUI";
                GameObject wrapper =
                    Instantiate(
                        GameManager.LoadPrefab("Dialogue/" + wrapperName),
                        GameObject.Find("Canvas").transform
                    );
                wrapper.name = wrapperName;
                isDWrapperEnabled = true;
            }

            //If Dialogue Circle is fading in and out. it stops and disappear.
            Image circle = null;
            if (GameObject.Find("DialogueCircle").TryGetComponent<Image>(out circle) && circle.enabled == true)
            {
                circle.enabled = false;
                if (coroutineDict.ContainsKey("FadeInAndOutCircle"))
                {
                    StopCoroutine(coroutineDict["FadeInAndOutCircle"]);
                    coroutineDict.Remove("FadeInAndOutCircle");
                }
            }

            TextMeshProUGUI speakerTMPro = GameObject.Find("SpeakerText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dialogueTMPro = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();
            string speaker = unit.speaker;
            string sentence = unit.sentence;

            speakerTMPro.text = speaker;

            for (int i = 1; i <= sentence.Length; i++)
            {
                string slicedSentence = sentence.Substring(0, i);
                string lastchar = sentence.Substring(i - 1, 1);

                if (lastchar == "<")
                {
                    i += sentence.Substring(i).IndexOf('>');
                    continue;
                }
                dialogueTMPro.text = slicedSentence;

                if (lastchar == "\n" || lastchar == "," || lastchar == "." || lastchar == "…")
                    yield return new WaitForSeconds(dialogues.TextSpeed * 5);
                else
                    yield return new WaitForSeconds(dialogues.TextSpeed);


                if (InputManager.inst.GetInputNextText() && !dialogues.ForceToNextDialogue)
                {
                    dialogueTMPro.text = sentence;
                    break;
                }
            }

        }

        public IEnumerator WaitForSec(DialogueUnit unit)
        {
            yield return new WaitForSeconds((float)unit.EtcInfo["WaitForSeconds"]);
        }

        public IEnumerator DisableDialogueCircle(DialogueUnit unit)
        {
            /*
            DialogueCircle 비활성화 코루틴
            DialogueCircle을 깜빡거리게 하는 코루틴을 중지시키고 coroutineDict로부터 반복자를 제거시킨다
            */
            GameObject.Find("DialogueCircle").GetComponent<Image>().enabled = false;
            if (coroutineDict.ContainsKey("FadeInAndOutCircle"))
            {
                StopCoroutine(coroutineDict["FadeInAndOutCircle"]);
                coroutineDict.Remove("FadeInAndOutCircle");
            }

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
            yield break;
        }

        public IEnumerator ShakeCamera(DialogueUnit unit)
        {
            /*
            DialogueController의 Component인 CinemachineImpulseSource로 impulse 생성
            */
            CinemachineImpulseSource source = DialogueController.instance.GetComponent<CinemachineImpulseSource>();

            source.GenerateImpulse();

            yield break;
        }

        public IEnumerator ForceToNextDialogue(DialogueUnit unit)
        {
            /*
            This Coroutine should be added before Dialogue Coroutine.
            As Dialogue Coroutine executed, It checks forcetoNextDialogue for whether it could be skip or not
            */
            dialogues.ForceToNextDialogue = true;

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
            if (isDWrapperEnabled == false)
            {
                string wrapperName = "DialogueWrapperUI";
                GameObject wrapper =
                    Instantiate(
                        GameManager.LoadPrefab("Dialogue/" + wrapperName),
                        GameObject.Find("Canvas").transform
                    );
                wrapper.name = wrapperName;
                isDWrapperEnabled = true;
            }
            yield break;
        }

        public IEnumerator DisableDialogueWrapper(DialogueUnit unit)
        {
            string wrapperName = "DialogueWrapperUI";
            if (isDWrapperEnabled == true)
            {
                Destroy(GameObject.Find(wrapperName));
                isDWrapperEnabled = false;
            }

            yield break;
        }

        public IEnumerator ControlCharacter1(DialogueUnit unit)
        {
            /*
            ControlCharacter1 contains character's emotion, voiceSource, position
            ex) ControlCharacter1 : ControlCharacter : Shinoh, Sad, VoiceFileName, -4, -1

            */
            if (!unit.EtcInfo.ContainsKey("ControlCharacter1"))
            {
                Debug.LogError("ControlCharacter1 parameter missed");
                yield break;
            }
            ControlCharacter control = (ControlCharacter)unit.EtcInfo["ControlCharacter1"];


            GameObject character;
            character = GameObject.Find(control.name);
            if (character == null)
            {
                float appearSpeed = Time.deltaTime * 3;
                character = Instantiate(GameManager.LoadPrefab("Character"), control.position, new Quaternion(), GameObject.Find("BgrWrapper").transform);
                character.GetComponent<SpriteRenderer>().sprite =
                GameManager.LoadImage("Character/" + control.name + control.emotion);
                character.name = control.name;

                SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();
                sprite.flipX = control.flip;
                Color color = sprite.color;

                float f = 0;
                while (f < 1)
                {
                    sprite.color = new Color(color.r, color.g, color.b, f);
                    f += appearSpeed;
                    yield return null;
                }
            }
            else
            {
                character.transform.position = control.position;
                SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();
                sprite.sprite = GameManager.LoadImage("Character/" + control.name + control.emotion);
                sprite.flipX = control.flip;
            }

            if (control.voice != "")
            {
                AudioManager.instance.PlayVoice("Voice/" + control.voice);
            }

            yield break;
        }


        public IEnumerator ControlCharacter2(DialogueUnit unit)
        {
            /*
            ControlCharacter2 contains character's emotion, voiceSource, position
            ex) ControlCharacter2 : ControlCharacter : Shinoh, Sad, VoiceFileName, -4, -1

            */
            if (!unit.EtcInfo.ContainsKey("ControlCharacter2"))
            {
                Debug.LogError("ControlCharacter1 parameter missed");
                yield break;
            }
            ControlCharacter control = (ControlCharacter)unit.EtcInfo["ControlCharacter2"];


            GameObject character;
            character = GameObject.Find(control.name);
            if (character == null)
            {
                float appearSpeed = Time.deltaTime * 3;
                character = Instantiate(GameManager.LoadPrefab("Character"), control.position, new Quaternion(), GameObject.Find("BgrWrapper").transform);
                character.GetComponent<SpriteRenderer>().sprite =
                GameManager.LoadImage("Character/" + control.name + control.emotion);
                character.name = control.name;

                SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();
                sprite.flipX = control.flip;
                Color color = sprite.color;

                float f = 0;
                while (f < 1)
                {
                    sprite.color = new Color(color.r, color.g, color.b, f);
                    f += appearSpeed;
                    yield return null;
                }
            }
            else
            {
                character.transform.position = control.position;
                SpriteRenderer sprite = character.GetComponent<SpriteRenderer>();
                sprite.sprite = GameManager.LoadImage("Character/" + control.name + control.emotion);
                sprite.flipX = control.flip;
            }

            if (control.voice != "")
            {
                AudioManager.instance.PlayVoice("Voice/" + control.voice);
            }


            yield break;
        }

        public IEnumerator DisappearCharacter(DialogueUnit unit)
        {

            string names = (string)unit.EtcInfo["DisappearCharacter"];
            string[] namesArr = names.Trim().Split(",");
            List<GameObject> characterList = new List<GameObject>();

            foreach (string name in namesArr)
            {
                characterList.Add(GameObject.Find(name));
            }

            float appearSpeed;
            SpriteRenderer sprite;
            Color color;
            float f = 1;
            while (f > 0)
            {
                appearSpeed = Time.deltaTime * 3;
                foreach (GameObject item in characterList)
                {
                    sprite = item.GetComponent<SpriteRenderer>();
                    color = sprite.color;
                    sprite.color = new Color(color.r, color.g, color.b, f);
                }
                f -= appearSpeed;
                yield return null;
            }

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
            oldBackgroundImage.GetComponent<SpriteRenderer>().material = GameManager.LoadMaterial("FadeMaterial");
            if (unit.EtcInfo.ContainsKey("BackOption"))
                oldBackgroundImage.GetComponent<SpriteRenderer>().material.SetInt("option", 5);

            Animator animator = oldBackgroundImage.AddComponent<Animator>();
            animator.runtimeAnimatorController = GameManager.LoadResource<RuntimeAnimatorController>("Animator/Dialogue/FadeInAndOutImage") as RuntimeAnimatorController;
            animator.SetTrigger("FadeOut");

            yield return null;
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer"));
            float duration = animState.length;
            yield return new WaitForSeconds(duration);
            Destroy(oldBackgroundImage);

            yield break;
        }

        public IEnumerator ShowImage(DialogueUnit unit)
        {

            GameObject bgrWrapper = GameObject.Find("BgrWrapper");
            GameObject image = GameManager.LoadPrefab("Dialogue/Image");


            image = Instantiate(image, bgrWrapper.transform);

            SpriteRenderer sRenderer = image.GetComponent<SpriteRenderer>();
            Color sColor = sRenderer.color;
            sRenderer.sprite = GameManager.LoadImage((string)unit.EtcInfo["Image"]);
            float appearSpeed;
            float f = 0;
            while (f < 1)
            {
                appearSpeed = Time.deltaTime * 3;

                sRenderer.color = new Color(sColor.r, sColor.g, sColor.b, f);

                f += appearSpeed;
                yield return null;
            }

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