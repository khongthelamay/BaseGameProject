using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Core
{
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
        [field: SerializeField] public SpriteRenderer SpriteShadow { get; set; }
        [field: SerializeField] public SpriteRenderer SpriteRarity { get; set; }
        [field: SerializeField] public Projectile Projectile { get; private set; }

        [field: SerializeField] public List<HeroSkillData> HeroSkillDataList { get; set; }
        [field: SerializeField] public FieldSlot FieldSlot { get; private set; }
        private UniTaskStateMachine<Hero> UniTaskStateMachine { get; set; }

        private void Awake()
        {
            InitStateMachine();
            HeroAttackRange.InitAttackRange(HeroStatData.BaseAttackRange);
        }

        public void OnDestroy()
        {
            UniTaskStateMachine?.Stop();
        }
        private void InitStateMachine()
        {
            UniTaskStateMachine = new UniTaskStateMachine<Hero>(this);
            UniTaskStateMachine.RegisterState(HeroSleepState.Instance);
            UniTaskStateMachine.Run();
        }

        public void FieldInit()
        {
            InitSkill();
            StartAttack();
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

        private void StartAttack()
        {
            UniTaskStateMachine.RequestTransition(HeroAttackState.Instance);
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
            SpriteShadow.sprite = spriteIcon;
            SpriteRarity.sprite = spriteRarity;
        }
    }
#endif
}