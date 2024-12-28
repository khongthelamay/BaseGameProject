using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AbilityContent : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtAbilityName;
    [SerializeField] TextMeshProUGUI txtAbilityDescription;

    private Sequence mySequence;
    private Ability currentAbility;
    
    UnityAction<bool> actionCallback;

    public void SetActionCallBack(UnityAction<bool> action)
    {
        this.actionCallback = action;
    }

    void AnimShow()
    {
        if (mySequence != null)
            mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOScale(1f, .15f).From(.5f).SetEase(Ease.OutBack));
        mySequence.OnComplete(() =>
        {
            actionCallback(true);
        });
    }

    public void InitData(Ability ability)
    {
        if (ability == currentAbility)
        {
            currentAbility = null;
            gameObject.SetActive(false);
            return;
        }

        string originalText = ability.Description;
        string des = ChangeNumberColor(originalText);
        
        txtAbilityName.text = ability.Name;
        txtAbilityDescription.text = des;
        currentAbility = ability;
        AnimShow();
    }
    
    private string ChangeNumberColor(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, @"(\S*\d+\S*\s*times|\S*\d+\S*)", match => $"<color=#34f16b>{match.Value}</color>");
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            DetectClickedObject();
        }
        #endif

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                DetectClickedObject();
            }
        }
    }
    
    private void DetectClickedObject()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        
#if UNITY_EDITOR
        pointerEventData.position = Input.mousePosition;
#endif
        
        if (Application.platform == RuntimePlatform.Android)
        {
            pointerEventData.position = Input.GetTouch(0).position;
        }

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == this.gameObject)
                return;
        }

        currentAbility = null;
        actionCallback(false);
        gameObject.SetActive(false);
    }

    public void ClearAnimation()
    {
        if (mySequence != null)
            mySequence.Kill();
    }
}
