using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimOnSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Transform contentAnim;
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
    }
}
