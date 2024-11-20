using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AnimOnSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Transform contentAnim;
    public UnityAction actionCallback;
    Sequence sequence;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (sequence != null)
            sequence.Kill();
        sequence = UIAnimation.AnimSlotDown(contentAnim);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (sequence != null)
            sequence.Kill();
        sequence = UIAnimation.AnimSlotUp(contentAnim);
        if (actionCallback != null)
            sequence.OnComplete(() => {
                actionCallback();
            });
    }

    public void CleanAnimation()
    {
        if (sequence != null)
            sequence.Kill();
    }
}
