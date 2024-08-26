using System;
using System.Collections;
using System.Collections.Generic;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.UI;

public class ModalHeroInteract : ACachedMonoBehaviour
{
    [field: SerializeField] public FieldSlot FieldSlot {get; private set;}
    [field: SerializeField] public Hero OwnHero {get; private set;}
    [field: SerializeField] public Transform InteractGroup {get; private set;}
    [field: SerializeField] public Button ButtonMerge {get; private set;}
    [field: SerializeField] public Button ButtonSell {get; private set;}
    [field: SerializeField] public Button ButtonClose {get; private set;}

    private void Awake()
    {
        ButtonMerge.onClick.AddListener(OnButtonMergeClick);
        ButtonSell.onClick.AddListener(OnButtonSellClick);
        ButtonClose.onClick.AddListener(OnButtonCloseClick);
    }
    public void SetFieldSlot(FieldSlot fieldSlot)
    {
        if (FieldSlot != null)
        {
            FieldSlot.Hero.HideAttackRange();
        }
        FieldSlot = fieldSlot;
        if (OwnHero != null)
        {
            OwnHero.HideAttackRange();
        }
        OwnHero = FieldSlot.Hero;
        OwnHero.ShowAttackRange();
        InteractGroup.position = OwnHero.transform.position;
    }
    private void OnButtonMergeClick()
    {
        Debug.Log("Merge");
        FieldManager.Instance.TryFusionHeroInFieldSlot(FieldSlot);
        OnClose();
    }
    private void OnButtonSellClick()
    {
        Debug.Log("Sell");
        FieldManager.Instance.TrySellHeroInFieldSlot(FieldSlot);
        OnClose();
    }
    private void OnButtonCloseClick()
    {
        OnClose();
    }
    private void OnClose()
    {
        OwnHero.HideAttackRange();
        OwnHero = null;
        gameObject.SetActive(false);
    }

}
