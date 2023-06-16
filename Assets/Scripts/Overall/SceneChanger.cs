using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;
    private GameObject canvas;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            {
                canvas = GameObject.Find("Canvas");

                //Re reference the object when the scene has changed.
                SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode)=>
                {
                    canvas = GameObject.Find("Canvas");
                    GameManager.loadedResources = new Dictionary<string, UnityEngine.Object>();
                };
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
    }


    public void ChangeScene(string sceneName, string effectName)
    {

        if (effectName == "FadeOutScene" || effectName == "")
        {
            StartCoroutine(fadeOutScene(sceneName));
        }
        else if(effectName == "BloodFilled")
        {
            StartCoroutine(fadeOutBloodFilled(sceneName));
        }
        else
        {
            StartCoroutine(fadeOutScene(sceneName));
        }
    }

    public void ChangeScene(string sceneName)
    {
            StartCoroutine(fadeOutScene(sceneName));
    }

    public void LoadScene(string effectName = "FadeInScene")
    {
        if (effectName == "FadeInScene")
        {
            StartCoroutine(fadeInScene());
        }
        else if (effectName == "BloodFilled")
        {
            StartCoroutine(fadeInBloodFilled());
        }
        else
        {
            StartCoroutine(fadeInScene());
        }
    }

    IEnumerator fadeOutScene(string sceneName)
    {
        AudioSource audioSource = AudioManager.instance.bgmAudioSource;
        GameObject blinder = Instantiate(GameManager.LoadPrefab("SceneChanger/FadeOutSceneBlinder"), canvas.transform);
        Animator animator = blinder.GetComponent<Animator>();
        animator.SetTrigger("FadeOut");

        yield return null;

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer"));
        float duration = animState.length;
        float deltaTimeDivDur;

        while (audioSource.volume > 0)
        {
            deltaTimeDivDur = Time.deltaTime / duration;
            audioSource.volume -= deltaTimeDivDur;
            yield return new WaitForSeconds(deltaTimeDivDur);
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    IEnumerator fadeInScene()
    {
        AudioSource audioSource = AudioManager.instance.bgmAudioSource;
        GameObject blinder = Instantiate(GameManager.LoadPrefab("SceneChanger/FadeInSceneBlinder"), canvas.transform);
        Animator animator = blinder.GetComponent<Animator>();
        animator.SetTrigger("FadeIn");

        yield return null;

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer"));
        float duration = animState.length;
        float deltaTimeDivDur;

        while (audioSource.volume < 1)
        {
            deltaTimeDivDur = Time.deltaTime / duration;
            audioSource.volume += deltaTimeDivDur;
            yield return new WaitForSeconds(deltaTimeDivDur);
        }

        Destroy(blinder);
    }

    IEnumerator fadeOutBloodFilled(string sceneName)
    {
        AudioSource audioSource = AudioManager.instance.bgmAudioSource;
        GameObject blinder = Instantiate(GameManager.LoadPrefab("SceneChanger/BloodFilledFilter"), canvas.transform);
        Animator animator = blinder.GetComponent<Animator>();
        animator.SetTrigger("FadeOut");

        yield return null;

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer"));
        float duration = animState.length;
        float deltaTimeDivDur;

        while (audioSource.volume > 0)
        {
            deltaTimeDivDur = Time.deltaTime / duration;
            audioSource.volume -= deltaTimeDivDur;
            yield return new WaitForSeconds(deltaTimeDivDur);
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    IEnumerator fadeInBloodFilled()
    {
        AudioSource audioSource = AudioManager.instance.bgmAudioSource;
        GameObject blinder = Instantiate(GameManager.LoadPrefab("SceneChanger/BloodFilledFilter"), canvas.transform);
        Animator animator = blinder.GetComponent<Animator>();
        animator.SetTrigger("FadeIn");

        yield return null;

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer"));
        float duration = animState.length;
        float deltaTimeDivDur;

        while (audioSource.volume < 1)
        {
            deltaTimeDivDur = Time.deltaTime / duration;
            audioSource.volume += deltaTimeDivDur;
            yield return new WaitForSeconds(deltaTimeDivDur);
        }

        Destroy(blinder);
    }
}
