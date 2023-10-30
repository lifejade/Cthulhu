using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnHighlightTextGlow : MonoBehaviour
{

    public IEnumerator GlowOn(){
        gameObject.GetComponentInChildren<TextMeshProUGUI>().font = (TMP_FontAsset)GameManager.LoadResource<TMP_FontAsset>("Fonts/KRGlow");
        yield break;
    }

    public IEnumerator GlowOff(){
        gameObject.GetComponentInChildren<TextMeshProUGUI>().font = (TMP_FontAsset)GameManager.LoadResource<TMP_FontAsset>("Fonts/KR");
        yield break;
    }
}
