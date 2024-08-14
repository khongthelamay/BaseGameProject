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
    [field: SerializeField] private HeroStatData HeroStatData {get; set;}
    [field: SerializeField] private List<HeroSkillData> HeroSkillDataList {get; set;}

    private void Awake()
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
}

#if UNITY_EDITOR
public partial class Hero
{
    [field: Title("Editor Only")]
    [field: SerializeField] private SpriteRenderer SpriteShape {get;  set;}
    [field: SerializeField] private SpriteRenderer SpriteNumber {get; set;}
    [field: SerializeField] private SpriteRenderer SpriteColor {get; set;}
    
    [field: SerializeField] public Sprite[] ShapeArray {get; private set;}
    [field: SerializeField] public Sprite[] NumberArray {get; private set;}
    [field: SerializeField] public Color[] ColorArray {get; private set;}
    
    [Button]
    private void InitHero()
    {
        SpriteNumber.sprite = NumberArray[(int)HeroStatData.HeroRarity];
        SpriteShape.sprite = ShapeArray[(int)HeroStatData.HeroTrait];
        SpriteColor.color = ColorArray[(int)HeroStatData.HeroRace];
    }
}
#endif