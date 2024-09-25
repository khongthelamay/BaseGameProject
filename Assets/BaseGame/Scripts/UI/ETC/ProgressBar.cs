using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtProgress;
    [SerializeField] RectTransform rectFill;
    [SerializeField] RectTransform rectBG;
    Vector2 vectorSizeDelta;
    private void Awake()
    {
        vectorSizeDelta = rectBG.sizeDelta;
        vectorSizeDelta.x = 0;
        rectFill.sizeDelta = vectorSizeDelta;
    }

    public void ChangeTextProgress(string strProgress) {
        txtProgress.text = strProgress;
    }

    public void ChangeProgress(float value) {
        if (value > 1)
            value = 1;
        vectorSizeDelta = new Vector2(rectBG.rect.width * value, rectBG.rect.height);
        rectFill.sizeDelta = vectorSizeDelta;
    }
}
