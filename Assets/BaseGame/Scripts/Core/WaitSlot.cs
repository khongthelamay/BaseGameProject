using Core;
using Core.SimplePool;
using Manager;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaitSlot : ACachedMonoBehaviour, IPointerClickHandler
{
    private BattleManager BattleManagerCache { get; set; }
    private BattleManager BattleManager => BattleManagerCache ??= BattleManagerCache = BattleManager.Instance;
    [field: SerializeField] public Hero OwnerHero {get; private set;}
    
    [Button]
    public void RandomOwnerHero()
    {
        if (OwnerHero != null)
        {
            OwnerHero.Despawn();
        }
        HeroConfigData heroConfigData = HeroPoolGlobalConfig.Instance.GetRandomHeroPrefab(1);
        OwnerHero = heroConfigData.HeroPrefab.Spawn()
            .WaitSlotInit(this);
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
