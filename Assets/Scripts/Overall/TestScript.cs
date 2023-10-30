using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dialogue;

public class TestScript : MonoBehaviour
{

    public JArray root;
    public string json;

    public void Start()
    {
        json = Resources.Load<TextAsset>("Dialogues/" + "test").text;

        DialogueUnits a = JsonConvert.DeserializeObject<DialogueUnits>(json);

        Debug.Log(a.ToString());
        
        // Recursively parse the JSON data and save it as variables.
        // ParseJson(json, out root);

    }

    private void ParseJson(string json, out JArray parsedJson)
    {
        // Convert the JSON string to a JSON object.
        parsedJson = JsonConvert.DeserializeObject<JArray>(json);

        // Recursively parse the JSON object and save it as variables.
        foreach (object jOb in parsedJson)
        {
            if(jOb is JObject){
                JObject tempJOb = null;
                if (tempJOb.ContainsKey("choice"))
                {
                   Debug.Log((jOb as JObject)["choice"]["choice1"][0]);
                }
            }
            /*
            object value = kvp.Value;

            // If the value is a dictionary, recursively parse it.
            if (value is Dictionary<string, object>)
            {
                ParseJson(value.ToString(), out value);
            }

            // Otherwise, simply save the value as a variable.
            else
            {
                parsedJson[kvp.Key] = value;
            }*/
        }

    }
}