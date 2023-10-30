using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    private Dictionary<string, IEnumerator> coroutineDict = new Dictionary<string, IEnumerator>();
    public static Dictionary<string, UnityEngine.Object> loadedResources;

    public float targetScreenWidthRatio = 16f;
    public float targetScreenHeightRatio = 9f;
    [HideInInspector]
    public float targetScreenRatio, screenRatio, cameraWidth, cameraHeight;
    [HideInInspector]
    public bool matchToWidth;
    [HideInInspector]
    public Vector2 screenBottomLeftInWorld, screenTopRightInWorld;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            targetScreenRatio = targetScreenWidthRatio / targetScreenHeightRatio;
            screenRatio = (float)Screen.width / (float)Screen.height;
            matchToWidth = true;
            // GameManager.NearlyEqual(targetScreenRatio, screenRatio) ? true : targetScreenRatio > screenRatio;

            screenBottomLeftInWorld = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
            screenTopRightInWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            cameraWidth = (screenTopRightInWorld.x - screenBottomLeftInWorld.x) * 100;
            cameraHeight = (screenTopRightInWorld.y - screenBottomLeftInWorld.y) * 100;

            coroutineDict = new Dictionary<string, IEnumerator>();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {

    }



    public static T LoadResource<T>(string filename) where T : UnityEngine.Object
    {
        //Load resources that indicated by filename parameter. Filename must not have an extension.
        UnityEngine.Object loadedResource;
        bool isExist = GameManager.loadedResources.TryGetValue(filename, out loadedResource);

        if (!isExist)
        {
            //Resources.Load method's generic type can't be System.Object
            loadedResource = Resources.Load<UnityEngine.Object>(filename);
            GameManager.loadedResources.Add(filename, loadedResource);
            if (loadedResource == null)
            {
                throw new FileNotFoundException(filename + " not found");
            }
        }


        return (T)loadedResource;
    }
    public static GameObject LoadPrefab(string filename)
    {
        GameObject loadedObject = Resources.Load<GameObject>("Prefab/" + filename);
        if (loadedObject == null)
        {
            throw new Exception(filename + " not found");
        }
        return loadedObject;
    }

    public static AudioClip LoadAudio(string filename)
    {
        AudioClip audioClip = Resources.Load<AudioClip>("Audio/" + filename);
        if (audioClip == null)
        {
            throw new Exception(filename + " not found");
        }
        return audioClip;
    }

    public static Sprite LoadImage(string filename)
    {
        Sprite sprite = Resources.Load<Sprite>("Images/" + filename);
        if (sprite == null)
        {
            throw new Exception(filename + " not found");
        }
        return sprite;
    }

    public static Material LoadMaterial(string filename)
    {
        Material material = Resources.Load<Material>("Materials/" + filename);
        if (material == null)
        {
            throw new Exception(filename + " not found");
        }
        return material;
    }

    public void CreateJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream fileStream = new(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public T LoadJsonFile<T>(string loadPath, string fileName)
    {
        // FileStream fileStream = new(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        // byte[] data = new byte[fileStream.Length];
        // fileStream.Read(data, 0, data.Length);
        // fileStream.Close();
        // string jsonData = Encoding.UTF8.GetString(data);
        string jsonData = Resources.Load<TextAsset>("Dialogues/" + fileName).text;
        Debug.Log(jsonData);
        return JsonUtility.FromJson<T>(jsonData);
    }

    public static bool NearlyEqual(float a, float b)
    {
        const double MinNormal = 2.2250738585072014E-308d;
        float absA = Mathf.Abs(a);
        float absB = Mathf.Abs(b);
        float diff = Mathf.Abs(a - b);

        if (a.Equals(b))
        { // shortcut, handles infinities
            return true;
        }
        else if (a == 0 || b == 0 || absA + absB < MinNormal)
        {
            // a or b is zero or both are extremely close to it
            // relative error is less meaningful here
            return diff < (Mathf.Epsilon * MinNormal);
        }
        else
        { // use relative error
            return diff / (absA + absB) < Mathf.Epsilon;
        }
    }

}
