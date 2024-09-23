﻿using Core;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class FieldSlot : ACachedMonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
{
    [Inject] private Hero.Factory HeroFactory { get; set; }
    [field: SerializeField] public Vector2 DefaultSize {get; private set;}
    [field: SerializeField] public int RowId {get; private set;}
    [field: SerializeField] public int ColumnId {get; private set;}
    [field: SerializeField] private GameObject UpgradeMark {get; set;}
    [field: SerializeField] public Hero Hero {get; private set;}

    public bool TryAddHero(Hero hero)
    {
        if (Hero != null) return false;
        Hero = hero;
        Hero.SetupFieldSlot(this);
        Hero.FieldInit();
        return true;
    }
    
    public bool TryGetHero(out Hero hero)
    {
        hero = Hero;
        return hero != null;
    }
    public bool TryRemoveHero()
    {
        if (!TryGetHero(out Hero hero)) return false;
        hero.SelfDespawn();
        Hero = null;
        return true;
    }
    public FieldSlot SetupCoordinate(int rowId, int columnId)
    {
        RowId = rowId;
        ColumnId = columnId;

        return this;
    }
    public FieldSlot SetupTransform(Transform parent)
    {
        Transform.SetParent(parent);
        return this;
    }
    public FieldSlot SetupPosition(int maxRow, int maxColumn)
    {
        float x = (ColumnId - maxColumn / 2f + 0.5f) * DefaultSize.x;
        float y = (- RowId + maxRow / 2f - 0.5f) * DefaultSize.y;
        transform.localPosition = new Vector3(x, y, 0);
        
        return this;
    }
    public void RemoveHero()
    {
        if (!TryGetHero(out Hero hero)) return;
        hero.SelfDespawn();
        Hero = null;
    }
    public bool TryUpgradeHero()
    {
        if (!TryGetHero(out Hero hero)) return false;
        HeroStatData heroStatData = hero.HeroStatData;
        if (heroStatData.HeroRarity == Hero.Rarity.Mythic) return false;
        hero.SelfDespawn();
        
        Hero heroPrefab = HeroPoolGlobalConfig.Instance.GetRandomHeroUpgradePrefab(heroStatData.HeroRarity + 1);
        Hero = HeroFactory.Create(heroPrefab);
        Hero.SetupFieldSlot(this);

        return true;
    }
    public void SetUpgradeMark(bool isShow)
    {
        UpgradeMark.SetActive(isShow);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Hero != null)
        {
            TempUIManager.Instance.ShowModalHeroInteract(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!TryGetHero(out _)) return;
        InputManager.Instance.SetStartDragFieldSlot(this);
        InputManager.Instance.SetEndDragFieldSlot(this);
    }
    public void OnDrag(PointerEventData eventData)
    {
        
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        InputManager.Instance.SetStartDragFieldSlot(null);
        InputManager.Instance.SetEndDragFieldSlot(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InputManager.Instance.StartDragFieldSlot == null) return;
        InputManager.Instance.SetEndDragFieldSlot(this);
    }

}