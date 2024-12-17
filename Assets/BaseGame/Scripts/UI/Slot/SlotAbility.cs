using Core;
using UnityEngine;

public class SlotAbility : SlotBase<Ability>
{
    [Header("==== Ability Slot ====")]
    [SerializeField] GameObject objLock;
    public override void InitData(Ability data)
    {
        if (data is NormalAttackAbility)
        {
            gameObject.SetActive(false);
            return;
        }

        base.InitData(data);
        imgIcon.sprite = data.Icon;
        objLock.SetActive(!HeroManager.Instance.CurrentHeroAbilityIsUnlock(data));
    }
}
