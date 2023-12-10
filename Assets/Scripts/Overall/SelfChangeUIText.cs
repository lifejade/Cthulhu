using Dialogue;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelfChangeUIText : MonoBehaviour
{
    TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        DialogueUnits temp = JsonConvert.DeserializeObject<DialogueUnits>(GameManager.LoadResource<TextAsset>("Dialogues/" + "DialogueCp1json").text);
        

        text.text = temp.uList.First.Next.Next.Value.sentence;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
