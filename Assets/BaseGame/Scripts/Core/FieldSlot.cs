using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Manager;
using TW.ACacheEverything;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.EventSystems;

[SelectionBase]
public partial class FieldSlot : ACachedMonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler
{
    public static Action FieldSlotChangedCallback { get; private set; }
    private BattleManager BattleManagerCache { get; set; }
    private BattleManager BattleManager => BattleManagerCache ??= BattleManagerCache = BattleManager.Instance;
    
    private FactoryManager FactoryManagerCache { get; set; }
    private FactoryManager FactoryManager => FactoryManagerCache ??= FactoryManager.Instance;
    
    private InputManager InputManagerCache { get; set; }
    private InputManager InputManager => InputManagerCache ??= InputManager.Instance;
    
    [field: SerializeField] public int RowId { get; private set; }
    [field: SerializeField] public int ColumnId { get; private set; }
    [field: SerializeField] private GameObject UpgradeMark { get; set; }
    [field: SerializeField] public Hero Hero { get; private set; }

    private FieldSlot[] SameHeroDataFieldSlots { get; set; } = new FieldSlot[2];
    private Hero[] SameHeroData { get; set; } = new Hero[2];

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
    public bool ForceMoveToFieldSlotAfterFusion(Hero hero)
    {
        Hero = hero;
        Hero.ForceMoveToFieldSlotAfterFusion(this);
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
        if (!TryGetHero(out Hero hero)) return;
        SetUpgradeMark(CanFusionHero());
    }

    public async UniTask TryFusionHero()
    {
        if (!CanFusionHero()) return;
        foreach (var slot in SameHeroDataFieldSlots)
        {
            slot.TryRemoveHero(out Hero hero);
            // Hero hero = SameHeroData[i];
            hero.MoveToPositionAndSelfDespawn(Transform.position);
        }

        Hero.ChangeToSleepState();
        Hero.HideAttackRange();
        await UniTask.WaitUntil(AllHeroDespawnCache, cancellationToken: this.GetCancellationTokenOnDestroy());
        await FactoryManager.CreateFusionEffect(Transform.position, this.GetCancellationTokenOnDestroy());
        TryRemoveHero(out Hero ownerHero);
        Hero.Rarity nextRarity = ownerHero.HeroConfigData.HeroRarity + 1;
        ownerHero.Despawn();
        
        HeroConfigData newHeroConfigData = HeroPoolGlobalConfig.Instance.GetRandomHeroConfigDataUpgrade(nextRarity);
        Hero newHero = newHeroConfigData.HeroPrefab.Spawn();
        newHero.SetPosition(Transform.position);
        ForceMoveToFieldSlotAfterFusion(newHero);

        FieldSlotChangedCallback?.Invoke();
    }
    [ACacheMethod]
    private bool AllHeroDespawn()
    {
        foreach (Hero hero in SameHeroData)
        {
            if (!hero.IsStopStateMachine())
            {
                return false;
            }
        }

        return true;
    }

    public bool CanFusionHero()
    {
        if (!TryGetHero(out Hero hero)) return false;
        if (hero.HeroConfigData.HeroRarity.IsMaxRarity()) return false;
        int sameHeroCount = BattleManager.GetFieldSlotHasSameHeroData(this, SameHeroDataFieldSlots, SameHeroData);
        return sameHeroCount >= 2;
    }

    public void TrySellHero()
    {
        if (!TryRemoveHero(out Hero hero)) return;
        hero.ResetAttackRange();
        hero.Despawn();
        FieldSlotChangedCallback?.Invoke();
    }

    #region Interactable Functions

    public void OnPointerClick(PointerEventData eventData)
    {
        InputManager.ShowFieldSlotInteract(this);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!TryGetHero(out Hero hero)) return;
        if (!hero.IsInBattleState()) return;
        InputManager.SetStartDragFieldSlot(this);
        InputManager.SetEndDragFieldSlot(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InputManager.TrySwapHeroInFieldSlot();
        InputManager.SetStartDragFieldSlot(null);
        InputManager.SetEndDragFieldSlot(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InputManager.Instance.StartDragFieldSlot == null) return;
        InputManager.SetEndDragFieldSlot(this);
    }

    private void OnDrawGizmos()
    {
        if (!TryGetComponent(out BoxCollider2D boxCollider2D)) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)boxCollider2D.offset, boxCollider2D.size);
    }

    #endregion
}