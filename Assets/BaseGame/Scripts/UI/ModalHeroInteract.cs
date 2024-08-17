using System;
using System.Collections;
using System.Collections.Generic;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.UI;

public class ModalHeroInteract : ACachedMonoBehaviour
{
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

    public void SetHero(Hero hero)
    {
        if (OwnHero != null)
        {
            OwnHero.HideAttackRange();
        }
        OwnHero = hero;
        OwnHero.ShowAttackRange();
        InteractGroup.position = OwnHero.transform.position;
    }      
    private void OnButtonMergeClick()
    {
        Debug.Log("Merge");
        OnClose();
    }
    private void OnButtonSellClick()
    {
        Debug.Log("Sell");
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
