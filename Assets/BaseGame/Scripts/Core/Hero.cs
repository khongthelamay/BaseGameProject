using System;
using System.Collections;
using System.Collections.Generic;
using TW.Utility.CustomComponent;
using UnityEngine;

public class Hero : ACachedMonoBehaviour
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
        
    }
    private void InitSkill()
    {
        foreach (HeroSkillData heroSkillData in HeroSkillDataList)
        {
            heroSkillData.InitSkill(this);
        }
    }
}
