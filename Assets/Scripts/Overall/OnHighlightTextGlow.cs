using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnHighlightTextGlow : MonoBehaviour
{

    public IEnumerator GlowOn(){
        gameObject.GetComponentInChildren<TextMeshProUGUI>().font = (TMP_FontAsset)GameManager.LoadResource("Fonts/KRGlow");
        yield break;
    }

    public IEnumerator GlowOff(){
        gameObject.GetComponentInChildren<TextMeshProUGUI>().font = (TMP_FontAsset)GameManager.LoadResource("Fonts/KR");
        yield break;
    }
}
