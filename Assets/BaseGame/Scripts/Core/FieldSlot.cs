using System;
using Core;
using Manager;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[SelectionBase]
public class FieldSlot : ACachedMonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
{
    public static Action FieldSlotChangedCallback { get; private set; }
    [Inject] private BattleManager BattleManager { get; set; }
    [field: SerializeField] public int RowId {get; private set;}
    [field: SerializeField] public int ColumnId {get; private set;}
    [field: SerializeField] private GameObject UpgradeMark {get; set;}
    [field: SerializeField] public Hero Hero {get; private set;}

    private void Awake()
    {
        FieldSlotChangedCallback += OnFieldSlotChanged;
    }

    private void OnDestroy()
    {
        FieldSlotChangedCallback -= OnFieldSlotChanged;
    }

    public bool TryAddHero(Hero hero)
    {
        if (Hero != null) return false;
        Hero = hero;
        Hero.MoveToFieldSlot(this);
        return true;
    }
    
    public bool TryGetHero(out Hero hero)
    {
        hero = Hero;
        return hero != null;
    }
    public bool TryRemoveHero(out Hero hero)
    {
        if (!TryGetHero(out hero)) return false;
        Hero = null;
        return true;
    }
    public void SetUpgradeMark(bool isShow)
    {
        UpgradeMark.SetActive(isShow);
    }
    private void OnFieldSlotChanged()
    {
        if (TryGetHero(out Hero hero) && hero.HeroConfigData.HeroRarity.IsMaxRarity())
        {
            SetUpgradeMark(false);
            return;
        }
        SetUpgradeMark(BattleManager.HasSame2HeroInOtherFieldSlot(this));
    }

    #region Interactable Functions

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!TryGetHero(out Hero hero)) return;
        if (!hero.IsInBattleState()) return;
        TempUIManager.Instance.ShowModalHeroInteract(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!TryGetHero(out Hero hero)) return;
        if (!hero.IsInBattleState()) return;
        InputManager.Instance.SetStartDragFieldSlot(this);
        InputManager.Instance.SetEndDragFieldSlot(this);
    }
    public void OnDrag(PointerEventData eventData)
    {
        
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        InputManager.Instance.TrySwapHeroInFieldSlot();
        InputManager.Instance.SetStartDragFieldSlot(null);
        InputManager.Instance.SetEndDragFieldSlot(null);
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InputManager.Instance.StartDragFieldSlot == null) return;
        InputManager.Instance.SetEndDragFieldSlot(this);
    }

    private void OnDrawGizmos()
    {
        if (!TryGetComponent(out BoxCollider2D boxCollider2D)) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)boxCollider2D.offset, boxCollider2D.size);
    }

    #endregion
}