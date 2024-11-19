using UnityEngine;
using TMPro;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtProgress;
    [SerializeField] RectTransform rectFill;
    [SerializeField] RectTransform rectBG;
    Sequence mySequence;
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
        vectorSizeDelta = new Vector2(rectBG.rect.size.x * value, rectBG.rect.height);
        
        float sizeCurrent = rectFill.rect.size.x;
        Vector2 fillValue = new Vector2(sizeCurrent, rectBG.rect.height);

        if (mySequence != null) mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(DOVirtual.Float(sizeCurrent, vectorSizeDelta.x, .25f, (value)=>{
            fillValue.x = value;
            rectFill.sizeDelta = fillValue;
        }));
        
    }
}
