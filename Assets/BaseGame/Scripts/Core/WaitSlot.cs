using System;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEngine;

public class WaitSlot : ACachedMonoBehaviour, IInteractable
{
    [field: SerializeField] public Hero OwnerHero {get; private set;}
    
    [Button]
    public void RandomOwnerHero()
    {
        if (OwnerHero != null)
        {
            Destroy(OwnerHero.gameObject);
        }
        Hero heroPrefab = HeroPoolGlobalConfig.Instance.GetRandomHeroPrefab(1);
        OwnerHero = Instantiate(heroPrefab);
        OwnerHero.WaitSlotInit(this);
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
        if (OwnerHero == null) return;
        if (FieldManager.Instance.TryAddHeroToFieldSlot(OwnerHero))
        {
            OwnerHero = null;
        }

    }

    #endregion
}
