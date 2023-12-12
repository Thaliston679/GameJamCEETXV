using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTranslate : MonoBehaviour
{
    TextMeshProUGUI textUGUI;
    [TextArea]public string[] texto;

    void Start()
    {
        textUGUI = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    public void UpdateText()
    {
        if (textUGUI != null) textUGUI.text = LangM.lang == 0 ? texto[0] : texto[1];
    }
}
