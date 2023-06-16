using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;
using System;
using Limitless;


namespace Dialogue
{
    public class DialogueMethodsCp1 : DialogueMethods
    {
        public Dictionary<string, Func<DialogueUnit, IEnumerator>> corList;
        public AnimationCurve animCurve;
        public AnimationCurve animCurve2;
        // Start is called before the first frame update
        private void Start()
        {
            corList = new Dictionary<string, Func<DialogueUnit, IEnumerator>>();
            controller = DialogueController.instance;

            corList.Add("SuckIntoOtherSpace", SuckIntoOtherSpace);
            corList.Add("EnableGlitch14", EnableGlitch14);
            corList.Add("DisableGlitch14", DisableGlitch14);
            corList.Add("CenterTextBox", CenterTextBox);
            corList.Add("PlayAudio", PlayAudio);
            corList.Add("GeneralDialogue", GeneralDialogue);
            corList.Add("DisableDialogueCircle", DisableDialogueCircle);
            corList.Add("EnableDialogueCircle", EnableDialogueCircle);
            corList.Add("ShakeCamera", ShakeCamera);
            corList.Add("ForceToNextDialogue", ForceToNextDialogue);
            corList.Add("GenerateCharacter", GenerateCharacter);
            corList.Add("MakeCharacterWalk", MakeCharacterWalk);
            corList.Add("EnableDialogueWrapper", EnableDialogueWrapper);
            corList.Add("DisableDialogueWrapper", DisableDialogueWrapper);
            corList.Add("ChangeBackGround", ChangeBackGround);
            corList.Add("WaitForSec", WaitForSec);

            dialogues = GameManager.instance.LoadJsonFile<DialogueUnits>("Assets/Resources/Dialogues", "dialogue1");

            RegisterActions();
        }

        // Update is called per frame
        void Update()
        {

        }


        private void RegisterActions()
        {
            List<DialogueUnit> list = dialogues.data;
            
            foreach (DialogueUnit unit in list)
            {
                string actionString = unit.action;
                actionString = actionString.Replace(" ", "");
                string[] actionArr = actionString.Split("|");

                string paramString = unit.param;
                paramString = paramString.Replace(" ", "");
                string[] paramArr = paramString.Split("|");

                foreach(string action in actionArr)
                {
                    if(action == ""){
                        unit.AddCoroutine(GeneralDialogue).AddCoroutine(EnableDialogueCircle);
                        continue;
                    }
                    unit.AddCoroutine(corList[action]);
                }
                foreach (string param in paramArr)
                {
                    if (param == "")
                    {
                        break;
                    }
                    Debug.Log(param);
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
                        default:
                            throw new Exception("param type exception. " + type + "is not valid type");
                    }
                }
                
            }
            
            // list[0]
            //     .AddCoroutine(CenterTextBox)
            //     .AddCoroutine(ForceToNextDialogue);

            // list[1]
            //     .AddCoroutine(EnableDialogueWrapper)
            //     .AddCoroutine(DisableDialogueCircle)
            //     .AddCoroutine(GeneralDialogue)
            //     .AddCoroutine(EnableDialogueCircle);

            // list[2]
            //     .AddInfo("BackgroundImage", "Dialogue/bizarrecity")
            //     .AddCoroutine(DisableDialogueCircle)
            //     .AddCoroutine(EnableGlitch14)
            //     .AddCoroutine(ChangeBackGround)
            //     .AddCoroutine(GeneralDialogue);




            // // list[2]
            // //     .AddCoroutine(DisableDialogueCircle)
            // //     .AddCoroutine(ChangeBackGroundCor)
            // //     .AddInfo("BackgroundImage", "DialogueExample")
            // //     .AddCoroutine(GeneralDialogueCor)
            // //     .AddCoroutine(GenerateCharacter)
            // //     .AddInfo("CharacterPrefab", "Heroines/heroine1")
            // //     .AddInfo("CharacterPos", new Vector2(-4, -2))
            // //     .AddCoroutine(EnableDialogueCircle);


            // list[7]
            //     .AddInfo("WaitForSeconds", 1f)
            //     .AddInfo("AudioName", "Dialogue/Swoosh")
            //     .AddInfo("BackgroundImage", "Dialogue/Hastur")
            //     .AddCoroutine(DisableDialogueCircle)
            //     .AddCoroutine(GeneralDialogue)
            //     .AddCoroutine(DisableDialogueWrapper)
            //     .AddCoroutine(PlayAudio)
            //     .AddCoroutine(ShakeCamera)
            //     .AddCoroutine(WaitForSecCor)
            //     .AddCoroutine(SuckIntoOtherSpace);

            // list[8]
            //     .AddCoroutine(EnableDialogueWrapper)
            //     .AddCoroutine(EnableGlitch14)
            //     .AddCoroutine(GeneralDialogue)
            //     .AddCoroutine(EnableDialogueCircle);

            // list[21]
            //     .AddCoroutine(ShakeCamera)
            //     .AddCoroutine(DisableDialogueCircle)
            //     .AddCoroutine(GeneralDialogue)
            //     .AddCoroutine(EnableDialogueCircle);


            // list[26]
            //     .AddInfo("BackgroundImage", "Dialogue/bizarrecity")
            //     .AddCoroutine(DisableDialogueCircle)
            //     .AddCoroutine(DisableGlitch14)
            //     .AddCoroutine(ChangeBackGround)
            //     .AddCoroutine(GeneralDialogue)
            //     .AddCoroutine(EnableDialogueCircle);

        }

        private IEnumerator CenterTextBox(DialogueUnit unit)
        {
            string preName = "CenterTextBox";
            if (GameObject.Find(preName) != null)
                throw new Exception("CenterTextBox Already Exist");


            float slowTxtSpd = textSpeed * 2f;
            float deltaTimeDivDur = 0f;
            float duration = 4f;
            //center textbox prefab name
            GameObject centerTextBox = null;
            Image box = null;
            TextMeshProUGUI text = null;
            Color boxColor;
            Color textColor;

            GameObject txtBoxPre = GameManager.LoadPrefab("Dialogue/" + preName);
            Image imgPre = txtBoxPre.GetComponent<Image>();
            TextMeshProUGUI textPre = txtBoxPre.GetComponentInChildren<TextMeshProUGUI>();

            imgPre.color = new Color(imgPre.color.r, imgPre.color.g, imgPre.color.b, 0f);
            textPre.color = new Color(textPre.color.r, textPre.color.g, textPre.color.b, 0f);
            centerTextBox =
                Instantiate(
                    GameManager.LoadPrefab("Dialogue/" + preName),
                    GameObject.Find("Canvas").transform
                );
            centerTextBox.name = preName;

            box = centerTextBox.GetComponent<Image>();
            text = centerTextBox.GetComponentInChildren<TextMeshProUGUI>();
            boxColor = new Color(box.color.r, box.color.g, box.color.b, box.color.a);
            textColor = new Color(text.color.r, text.color.g, text.color.b, text.color.a);

            while (box.color.a <= 0.6f)
            {
                deltaTimeDivDur = Time.deltaTime / duration;

                boxColor.a = boxColor.a + deltaTimeDivDur;
                textColor.a = textColor.a + (deltaTimeDivDur * 1f / 0.6f);
                box.color = boxColor;
                text.color = textColor;


                yield return new WaitForSeconds(deltaTimeDivDur);
            }

            string sentence = unit.sentence;
            for (int i = 1; i <= sentence.Length; i++)
            {
                string slicedSentence = sentence.Substring(0, i);
                string lastchar = sentence.Substring(i - 1, 1);

                text.text = slicedSentence;

                if (lastchar == "\n" || lastchar == "," || lastchar == ".")
                    yield return new WaitForSeconds(slowTxtSpd * 10);
                else
                    yield return new WaitForSeconds(slowTxtSpd);


                if (InputManager.inst.GetInput("space"))
                {
                    text.text = sentence;
                    break;
                }
            }
            yield return new WaitForSeconds(1f);

            while (box.color.a > 0f)
            {
                deltaTimeDivDur = Time.deltaTime / duration;

                boxColor.a = boxColor.a - deltaTimeDivDur;
                textColor.a = textColor.a - (deltaTimeDivDur * 1f / 0.6f);
                box.color = boxColor;
                text.color = textColor;

                yield return new WaitForSeconds(deltaTimeDivDur);
            }

            Destroy(centerTextBox);

            EndOfUnitCor();
        }

        private IEnumerator SuckIntoOtherSpace(DialogueUnit unit)
        {
            Volume volume = null;
            VolumeProfile volumeProfile = null;
            LensDistortion lensDistortion = null;
            Vignette vignette = null;
            float duration = 0f;
            float passedTime = 0f;
            float curveValue = 0f;
            float curveValue2 = 0f;

            if (!GameObject.Find("Global Volume").TryGetComponent<Volume>(out volume))
            {
                Debug.LogError($"No {nameof(Volume)} component attached to this object!", this);
                EndOfUnitCor();
                yield break;
            }

            volumeProfile = volume.profile;
            volumeProfile.TryGet<LensDistortion>(out lensDistortion);
            volumeProfile.TryGet<Vignette>(out vignette);
            lensDistortion.active = true;
            vignette.active = true;
            duration = animCurve[animCurve.length - 1].time;
            while (passedTime < duration)
            {
                passedTime += Time.deltaTime;
                curveValue = animCurve.Evaluate(passedTime);
                curveValue2 = animCurve2.Evaluate(passedTime);

                lensDistortion.intensity.value = -curveValue;
                lensDistortion.scale.value = 1f - curveValue;
                vignette.intensity.value = curveValue2;

                yield return null;
            }

            //Change Background paragragh

            GameObject oldBackgroundImage = GameObject.Find("BackgroundImage(Clone)");
            GameObject bgrWrapper = GameObject.Find("BgrWrapper");
            bool isOldExist = oldBackgroundImage != null;

            GameObject backgroundImage = GameManager.LoadPrefab("Dialogue/BackgroundImage");

            backgroundImage = Instantiate(backgroundImage, bgrWrapper.transform);
            SpriteRenderer image = backgroundImage.GetComponent<SpriteRenderer>();
            image.sprite = GameManager.LoadImage((string)unit.EtcInfo["BackgroundImage"]);


            Destroy(oldBackgroundImage);
            //Change Background paragragh


            yield return new WaitForSeconds(3f);
            AudioManager.instance.PlayAudio((string)unit.EtcInfo["AudioName"]);

            yield return new WaitForSeconds(1f);

            passedTime = 0f;
            while (passedTime < duration)
            {

                passedTime += Time.deltaTime;
                curveValue = animCurve.Evaluate(passedTime);
                curveValue2 = animCurve2.Evaluate(passedTime);

                lensDistortion.intensity.value = curveValue - 1f;
                lensDistortion.scale.value = curveValue;
                vignette.intensity.value = -curveValue2 + 1;

                yield return null;
            }
            EndOfUnitCor();
            yield break;
        }

        private IEnumerator EnableGlitch14(DialogueUnit unit)
        {
            Volume volume = null;
            VolumeProfile volumeProfile = null;
            LimitlessGlitch14 glitch14 = null;


            if (!GameObject.Find("Global Volume").TryGetComponent<Volume>(out volume))
            {
                Debug.LogError($"No {nameof(Volume)} component attached to this object!", this);
                EndOfUnitCor();
                yield break;
            }
            volumeProfile = volume.profile;
            volumeProfile.TryGet<LimitlessGlitch14>(out glitch14);
            glitch14.active = true;

            EndOfUnitCor();
            yield break;
        }

        private IEnumerator DisableGlitch14(DialogueUnit unit)
        {
            Volume volume = null;
            VolumeProfile volumeProfile = null;
            LimitlessGlitch14 glitch14 = null;


            if (!GameObject.Find("Global Volume").TryGetComponent<Volume>(out volume))
            {
                Debug.LogError($"No {nameof(Volume)} component attached to this object!", this);
                EndOfUnitCor();
                yield break;
            }
            volumeProfile = volume.profile;
            volumeProfile.TryGet<LimitlessGlitch14>(out glitch14);
            glitch14.active = false;

            EndOfUnitCor();
            yield break;
        }


    }

}
