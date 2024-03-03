using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;
using System;
using Limitless;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.EventSystems;


namespace Dialogue
{
    public class DialogueMethodsCp1 : DialogueMethods
    {
        //List of coroutines using in this chapter.
        public AnimationCurve animCurve;
        public AnimationCurve animCurve2;
        public string stringTalkName;

        // Start is called before the first frame update
        private void Start()
        {
            controller = DialogueController.instance;

            corDict.Add("SuckIntoOtherSpace", SuckIntoOtherSpace);
            corDict.Add("EnableGlitch14", EnableGlitch14);
            corDict.Add("DisableGlitch14", DisableGlitch14);
            corDict.Add("CenterTextBox", CenterTextBox);
            corDict.Add("PlayAudio", PlayAudio);
            corDict.Add("GeneralDialogue", GeneralDialogue);
            corDict.Add("SelectChoices", SelectChoices);
            corDict.Add("DisableDialogueCircle", DisableDialogueCircle);
            corDict.Add("EnableDialogueCircle", EnableDialogueCircle);
            corDict.Add("ShakeCamera", ShakeCamera);
            corDict.Add("ForceToNextDialogue", ForceToNextDialogue);
            corDict.Add("GenerateCharacter", GenerateCharacter);
            corDict.Add("MakeCharacterWalk", MakeCharacterWalk);
            corDict.Add("EnableDialogueWrapper", EnableDialogueWrapper);
            corDict.Add("DisableDialogueWrapper", DisableDialogueWrapper);
            corDict.Add("ChangeBackGround", ChangeBackGround);
            corDict.Add("ShowImage", ShowImage);
            corDict.Add("WaitForSec", WaitForSec);
            corDict.Add("ControlCharacter1", ControlCharacter1);
            corDict.Add("ControlCharacter2", ControlCharacter2);
            corDict.Add("DisappearCharacter", DisappearCharacter);

            corDict.Add("Sound1", Sound1);
            corDict.Add("Sound2", Sound2);
            corDict.Add("CG1", CG1);
            corDict.Add("CG2", CG2);
            corDict.Add("CG3", CG3);

            JsonToDicts(stringTalkName, 0);
            //dialogues = JsonConvert.DeserializeObject<DialogueUnits>(GameManager.LoadResource<TextAsset>("Dialogues/" + "dialogue1").text);
            dialogues.GetReadyToUse(corDict);
        }

        
        public IEnumerator SelectChoices(DialogueUnit unit)
        {
            List<DialogueUnits> choices = unit.choices;
            float yPosToAdd = 125;
            float yPos = (choices.Count * 125 - 100) / 2 * -1;
            bool selected = false;

            //InnerMethod when choice button clicked
            void innerSelectChoice()
            {   
                GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

                if (clickedButton != null)
                {
                    Button button = clickedButton.GetComponent<Button>();
                    if (button != null)
                    {
                        List<DialogueUnits> unitsList = dialogues.PrUnit.Choices;
                        foreach (DialogueUnits units in unitsList)
                        {
                            if (units.name == button.name)
                            {
                                LinkedListNode<DialogueUnit> tempNextNode = dialogues.PrNode.Next;
                                foreach (DialogueUnit unit in units.uList)
                                {
                                    dialogues.uList.AddBefore(tempNextNode,unit);
                                }
                                dialogues.GetReadyToUse(this.corDict);
                                break;
                            }
                        }
                    }
                }
                selected = true;
            }
            //Coroutine awaits players select
            IEnumerator checkSelected()
            {
                while (true)
                {
                    if(selected)
                        break;
                    yield return null;
                }
            }
            List<GameObject> tempButtons = new();
            foreach (DialogueUnits choice in choices)
            {
                GameObject chButton = Instantiate(Resources.Load<GameObject>("Prefab/Dialogue/SelectChoicesButton"), GameObject.Find("Canvas").transform);
                tempButtons.Add(chButton);
                chButton.name = choice.name;
                chButton.GetComponent<RectTransform>().localPosition = new Vector2(0, yPos);
                chButton.GetComponent<Button>().onClick.AddListener(innerSelectChoice);

                yPos += yPosToAdd;
            }

            yield return StartCoroutine(checkSelected());
            foreach (var button in tempButtons)
            {
                Destroy(button);
            }
            // dialogues.ForceToNextDialogue = true;
            yield break;
        }



        private IEnumerator CenterTextBox(DialogueUnit unit)
        {
            string preName = "CenterTextBox";
            if (GameObject.Find(preName) != null)
                throw new Exception("CenterTextBox Already Exist");


            float slowTxtSpd = dialogues.TextSpeed * 2f;
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


                if (InputManager.inst.GetInputNextText())
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
                yield break;
            }
            volumeProfile = volume.profile;
            volumeProfile.TryGet<LimitlessGlitch14>(out glitch14);
            glitch14.active = true;

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
                yield break;
            }
            volumeProfile = volume.profile;
            volumeProfile.TryGet<LimitlessGlitch14>(out glitch14);
            glitch14.active = false;

            yield break;
        }


    }

}
