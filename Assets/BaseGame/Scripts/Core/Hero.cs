using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEngine;

public partial class Hero : ACachedMonoBehaviour
{
    public enum Rarity
    {
        Common = 0,
        Rare = 1,
        Epic = 2,
        Legendary = 3,
        Mythic = 4
    }

    public enum Job
    {
        Fighter = 0,
        Ranger = 1,
        Cleric = 2,
        Magician = 3,
        Assassin = 4,
        Monk = 5,
        Summoner = 6,
    }

    [field: SerializeField] public HeroStatData HeroStatData { get; set; }
    [field: SerializeField] public HeroAttackRange HeroAttackRange { get; set; }
    [field: SerializeField] public SpriteRenderer SpriteGraphic { get; set; }
    [field: SerializeField] public SpriteRenderer SpriteRarity { get; set; }
    [field: SerializeField] public List<HeroSkillData> HeroSkillDataList { get; set; }
    [field: SerializeField] public FieldSlot FieldSlot { get; private set; }

    private void Awake()
    {
        HeroAttackRange.InitAttackRange(HeroStatData.BaseAttackRange);
    }

    public void FieldInit()
    {
        InitSkill();
    }
    public void SelfDespawn()
    {
        gameObject.SetActive(false);
    }
    private void InitSkill()
    {
        foreach (HeroSkillData heroSkillData in HeroSkillDataList)
        {
            heroSkillData.InitSkill(this);
        }
    }

    public void SetupFieldSlot(FieldSlot fieldSlot)
    {
        FieldSlot = fieldSlot;
        Transform.SetParent(FieldSlot.Transform);
        Transform.localPosition = Vector3.zero;
    }

    public void WaitSlotInit(WaitSlot waitSlot)
    {
        Transform.SetParent(waitSlot.Transform);
        Transform.localPosition = Vector3.zero;
    }

    public void ShowAttackRange()
    {
        HeroAttackRange.ShowAttackRange();
    }

    public void HideAttackRange()
    {
        HeroAttackRange.HideAttackRange();
    }
}

#if UNITY_EDITOR
public partial class Hero
{
    [Button]
    public void InitHero()
    {
        InitHero(HeroStatData.HeroSprite,
            BaseHeroGenerateGlobalConfig.Instance.RarityArray[(int)HeroStatData.HeroRarity]);
    }

    private void InitHero(Sprite spriteIcon, Sprite spriteRarity)
    {
        SpriteGraphic.sprite = spriteIcon;
        SpriteRarity.sprite = spriteRarity;
    }
}
#endif