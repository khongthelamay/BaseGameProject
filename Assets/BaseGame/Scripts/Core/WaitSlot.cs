using System;
using Core;
using Manager;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class WaitSlot : ACachedMonoBehaviour, IPointerClickHandler
{
    [Inject] private BattleManager BattleManager { get; set; }
    [Inject] private Hero.Factory HeroFactory { get; set; }
    [field: SerializeField] public Hero OwnerHero {get; private set;}
    
    [Button]
    public void RandomOwnerHero()
    {
        if (OwnerHero != null)
        {
            Destroy(OwnerHero.gameObject);
        }
        Hero heroPrefab = HeroPoolGlobalConfig.Instance.GetRandomHeroPrefab(1);
        OwnerHero = HeroFactory.Create(heroPrefab);
        OwnerHero.WaitSlotInit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OwnerHero == null) return;
        if (BattleManager.TryAddNewHero(OwnerHero))
        {
            OwnerHero = null;
        }
    }
}
