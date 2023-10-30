using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialogueWriter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(1);
        Write();
    }

    void Write()
    {
        // Folder, where a file is created.
        // Make sure to change this folder to your own folder
        string folder = @"D:/Text/";
        // Filename
        string txtFile = folder + "dialogueCp1txt.txt";
        string jsonFile = folder + "dialogueCp1json.json";
        // Fullpath. You can direct hardcode it if you like.
        StreamReader reader = new StreamReader(txtFile);
        
        StreamWriter writer = new StreamWriter(jsonFile);
        writer.AutoFlush = true;

        writer.WriteLine("{\"data\": [");
        string line;
        int id = 0;
        line = reader.ReadLine();
        while (null != line)
        {
            writer.WriteLine("{");
            writer.WriteLine("\"id\" : " + id + ",");
            id++;
            writer.WriteLine("\"action\" : \"\",");
            writer.WriteLine("\"param\" : \"\",");
            writer.WriteLine("\"sentence\" : \"" + line + "\"");

            if (null != (line = reader.ReadLine()))
                writer.WriteLine("},");
            else
                writer.WriteLine("}");
        }
        writer.WriteLine("]}");
        // using (StreamWriter writer = new StreamWriter(jsonFile))
        // {
        //     writer.WriteLine("Monica Rathbun");
        // }
        // Read a file
        // string readText = File.ReadAllText(fullPath);
    }
}
