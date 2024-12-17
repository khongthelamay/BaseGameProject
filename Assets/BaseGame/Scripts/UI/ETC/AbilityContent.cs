using System.Collections;
using System.Collections.Generic;
using Core;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class AbilityContent : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtAbilityName;
    [SerializeField] TextMeshProUGUI txtAbilityDescription;

    private Sequence mySequence;
    private Ability currentAbility;
    
    void AnimShow()
    {
        if (mySequence != null)
            mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOScale(1f, .15f).From(.5f).SetEase(Ease.OutBack));
    }

    public void InitData(Ability ability)
    {
        if (ability == currentAbility)
        {
            currentAbility = null;
            gameObject.SetActive(false);
            return;
        }
        txtAbilityName.text = ability.Name;
        txtAbilityDescription.text = ability.Description;
        currentAbility = ability;
        AnimShow();
    }
}
