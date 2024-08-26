using DG.Tweening;
using TW.Utility.CustomComponent;
using UnityEngine;

public class FieldSlot : ACachedMonoBehaviour , IInteractable
{
    [field: SerializeField] public Vector2 DefaultSize {get; private set;}

    [field: SerializeField] public int RowId {get; private set;}
    [field: SerializeField] public int ColumnId {get; private set;}
    [field: SerializeField] private GameObject UpgradeMark {get; set;}
    [field: SerializeField] public Hero Hero {get; private set;}

    public bool TryAddHeroFromWaitSlot(Hero hero)
    {
        if (Hero != null) return false;
        Hero = hero;
        Hero.SetupFieldSlot(this);
        return true;
    }
    public bool TryGetHero(out Hero hero)
    {
        hero = Hero;
        return hero != null;
    }

    public FieldSlot SetupCoordinate(int rowId, int columnId)
    {
        RowId = rowId;
        ColumnId = columnId;

        return this;
    }
    public FieldSlot SetupTransform(Transform parent)
    {
        transform.SetParent(parent);
        return this;
    }
    public FieldSlot SetupPosition(int maxRow, int maxColumn)
    {
        float x = (ColumnId - maxColumn / 2f + 0.5f) * DefaultSize.x;
        float y = (RowId - maxRow / 2f + 0.5f) * DefaultSize.y;
        transform.localPosition = new Vector3(x, y, 0);
        Tween a = Transform.DOMove(Vector3.zero, 1);
        a.timeScale = 0.5f;
        
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
        Hero = Instantiate(heroPrefab);
        Hero.SetupFieldSlot(this);

        return true;
    }
    public void SetUpgradeMark(bool isShow)
    {
        UpgradeMark.SetActive(isShow);
    }

    #region Interact Functions

    public void OnMouseDownCallback()
    {
        
    }

    public void OnMouseCallback()
    {

    }

    public void OnMouseUpCallback()
    {

    }

    public void OnMouseClickCallback()
    {
        if (Hero != null)
        {
            TempUIManager.Instance.ShowModalHeroInteract(this);
        }
    }

    #endregion


}