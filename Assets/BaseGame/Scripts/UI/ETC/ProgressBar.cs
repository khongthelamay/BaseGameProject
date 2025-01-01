using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class ProgressBar : MonoBehaviour
{
    
    [SerializeField] bool isLight = true;
    [SerializeField] TextMeshProUGUI txtProgress;
    [SerializeField] RectTransform rectFill;
    [SerializeField] RectTransform rectBG;

    [Header("---- Light ----")]
    [SerializeField] Transform trsLight;
    [SerializeField] Transform pointStart;
    [SerializeField] Transform pointEnd;

    Sequence mySequence;
    Sequence loopSequence;
    Vector2 vectorSizeDelta;
    private void Awake()
    {
        vectorSizeDelta = rectBG.sizeDelta;
        vectorSizeDelta.x = 0;
        rectFill.sizeDelta = vectorSizeDelta;
        Vector2 vectorStart = new Vector2(rectBG.transform.localPosition.x - rectBG.rect.width, trsLight.localPosition.y);
        Vector2 vectorEnd = new Vector2(rectBG.transform.localPosition.x + rectBG.rect.width * 2f, trsLight.localPosition.y);
        loopSequence = DOTween.Sequence();
        loopSequence.Append(trsLight.DOLocalMove(vectorEnd, 2f).From(vectorStart).SetDelay(2f).SetEase(Ease.Linear));
        loopSequence.SetLoops(-1);
    }

    public void ChangeTextProgress(string strProgress) {
        txtProgress.text = strProgress;
    }

    [Button]
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

    public void ClearAnimation() {
        if (mySequence != null)
            mySequence.Kill();
        if (loopSequence != null)
            loopSequence.Kill();
    }
}
