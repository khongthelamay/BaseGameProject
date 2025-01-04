using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UI;

public class SlotAbility : SlotBase<Ability>
{
    [Header("==== Ability Slot ====")] 
    [SerializeField] Image imgBg;
    [SerializeField] GameObject objLock;

    [SerializeField] List<Sprite> sprBg = new();
    public override void InitData(Ability data)
    {
        if (!data)
            return;
        if (data is NormalAttackAbility)
        {
            gameObject.SetActive(false);
            return;
        }

        base.InitData(data);
        imgIcon.sprite = data.Icon;
        bool isUnlock = HeroManager.Instance.CurrentHeroAbilityIsUnlock(data);
        bool isUltimate = false;
        imgBg.sprite = isUltimate? sprBg[0] : sprBg[1];
        objLock.SetActive(!isUnlock);
    }
}
