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
    public enum Trait
    {
        Trait0 = 0,
        Trait1 = 1,
        Trait2 = 2,
        Trait3 = 3,
    }
    public enum Race
    {
        Race0 = 0,
        Race1 = 1,
        Race2 = 2,
        Race3 = 3,
    }
    [field: SerializeField] public SpriteRenderer SpriteShape {get;  set;}
    [field: SerializeField] public SpriteRenderer SpriteNumber {get; set;}
    [field: SerializeField] public SpriteRenderer SpriteColor {get; set;}
    [field: SerializeField] public HeroStatData HeroStatData {get; set;}
    [field: SerializeField] public List<HeroSkillData> HeroSkillDataList {get; set;}
    [field: SerializeField] public FieldSlot FieldSlot {get; private set;}

    public void FieldInit()
    {
        InitSkill();
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
        Debug.Log(1);
        FieldSlot = fieldSlot;
        Transform.SetParent(FieldSlot.Transform);
        Transform.localPosition = Vector3.zero;
    }
    public void WaitSlotInit(WaitSlot waitSlot)
    {
        Transform.SetParent(waitSlot.Transform);
        Transform.localPosition = Vector3.zero;
    }
}

#if UNITY_EDITOR
public partial class Hero
{
    [Button]
    private void InitHero()
    {
        InitHero(BaseHeroGenerateGlobalConfig.Instance.NumberArray[(int)HeroStatData.HeroRarity], 
            BaseHeroGenerateGlobalConfig.Instance.ShapeArray[(int)HeroStatData.HeroTrait], 
            BaseHeroGenerateGlobalConfig.Instance.ColorArray[(int)HeroStatData.HeroRace]);
    }
    private void InitHero(Sprite spriteNumber, Sprite spriteShape, Color spriteColor)
    {
        SpriteNumber.sprite = spriteNumber;
        SpriteShape.sprite = spriteShape;
        SpriteColor.color = spriteColor;
    }

}
#endif