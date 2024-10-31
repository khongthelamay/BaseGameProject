using System;
using System.Collections.Generic;
using Core.SimplePool;
using Manager;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using TW.Utility.DesignPattern;
using TW.Utility.Extension;
using UnityEngine;

using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core
{
    public partial class Hero : ACachedMonoBehaviour, IAbilityTargetAble, IPoolAble<Hero>
    {
        [GUIColor("@TW.Utility.Extension.AColorExtension.GetColorInPalette(\"Job\", (int)$value)")]
        public enum Job
        {
            Legend = 0,
            
            Human = 1,
            Beast = 2,
            Undead = 3,
            Chess = 4,
            Insect = 5,
            Fish = 6,
        }
        [GUIColor("@TW.Utility.Extension.AColorExtension.GetColorInPalette(\"Class\", (int)$value)")]
        public enum Class
        {
            Melee = 1,
            Range = 2,
        }
        [field: Title(nameof(Hero))]
        [field: SerializeField] public HeroConfigData HeroConfigData { get; set; }
        [field: SerializeField] public HeroAttackRange HeroAttackRange { get; set; }
        [field: SerializeField] public SpriteRenderer SpriteGraphic { get; set; }
        [field: SerializeField] public SpriteRenderer SpriteRarity { get; set; }
        [field: SerializeField] public MoveProjectile MoveProjectile { get; private set; }
        [field: SerializeField] public GameObject VisibleGroup {get; private set;}
        [field: SerializeField] public FieldSlot FieldSlot { get; private set; }
        [field: SerializeField] public HeroAnim HeroAnim {get; private set;}
        
        
        private StateMachine StateMachine { get; set; }
        [field: SerializeField] public int AttackDamage { get; set; }
        [field: SerializeField] public float AttackSpeed { get; set; }
        [field: SerializeField] public float AttackRange { get; set; }
        [ShowInInspector]
        protected List<ActiveAbility> ActiveAbilities { get; set; } = new();
        [ShowInInspector]
        protected List<PassiveAbility> PassiveAbilities { get; set; } = new();
        
        private Vector3 MoveFromPosition { get; set; }
        private Vector3 MoveToPosition { get; set; }
        private Action OnMoveComplete { get; set; }

        public void OnDestroy()
        {
            StateMachine.Stop();
        }
        public Hero OnSpawn()
        {
            InitStateMachine();
            InitStat();
            InitAbility();
            
            
            return BattleManager.Instance.RegisterHero(this);
        }

        public void OnDespawn()
        {
            BattleManager.Instance.UnregisterHero(this);
        }
        private void InitStateMachine()
        {
            StateMachine = new StateMachine();
            StateMachine.RegisterState(HeroSleepState);
            StateMachine.Run();
        }
        private void InitStat()
        {
            AttackDamage = HeroConfigData.BaseAttackDamage;
            AttackSpeed = HeroConfigData.BaseAttackSpeed;
            AttackRange = HeroConfigData.BaseAttackRange;
        }

        protected virtual void InitAbility()
        {

        }
        public bool IsCurrentState(IState state)
        {
            return StateMachine.IsCurrentState(state);
        }
        public void ChangeToSleepState()
        {
            StateMachine.RequestTransition(HeroSleepState);
        }
        public void MoveToFieldSlot(FieldSlot fieldSlot)
        {
            SetVisible(false);
            MoveFromPosition = Transform.position;
            MoveToPosition = fieldSlot.Transform.position;
            OnMoveComplete = MoveComplete;
            
            FieldSlot = fieldSlot;
            Transform.SetParent(FieldSlot.Transform);
            Transform.localPosition = Vector3.zero;
            
            StateMachine.RequestTransition(HeroMoveState);
            return;
            void MoveComplete()
            {
                SetVisible(true);
                FieldSlot.FieldSlotChangedCallback?.Invoke();
                StateMachine.RequestTransition(HeroBattleDecisionState);
            }
        }
        public void MoveToPositionAndSelfDespawn(Vector3 toPosition)
        {
            SetVisible(false);
            MoveFromPosition = Transform.position;
            MoveToPosition = toPosition;
            OnMoveComplete = MoveComplete;
            
            Transform.localPosition = Vector3.zero;
            
            StateMachine.RequestTransition(HeroMoveState);
            return;
            void MoveComplete()
            {
                SelfDespawn();
            }
        }
        public void SetVisible(bool isVisible)
        {
            VisibleGroup.SetActive(isVisible);
        }

        public void SelfDespawn()
        {
            Destroy(gameObject);
        }
        

        public Hero WaitSlotInit(WaitSlot waitSlot)
        {
            Transform.SetParent(waitSlot.Transform);
            Transform.localPosition = Vector3.zero;
            StateMachine.RequestTransition(HeroIdleState);
            return this;
        }

        public void ShowAttackRange()
        {
            HeroAttackRange.ShowAttackRange(HeroConfigData.BaseAttackRange);
        }

        public void HideAttackRange()
        {
            HeroAttackRange.HideAttackRange();
        }
    }

#if UNITY_EDITOR
    public partial class Hero
    {
        public void EditorInit(HeroConfigData heroConfigData)
        {
            HeroConfigData = heroConfigData;
            HeroAttackRange = GetComponentInChildren<HeroAttackRange>();
            
            SpriteGraphic = transform.FindChildOrCreate("SpriteGraphic").GetComponentInChildren<SpriteRenderer>();
            SpriteRarity = transform.FindChildOrCreate("SpriteRarity").GetComponentInChildren<SpriteRenderer>();
            HeroAnim = GetComponentInChildren<HeroAnim>();
            MoveProjectile = AssetDatabase.LoadAssetAtPath<MoveProjectile>(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets("t:Prefab MoveProjectile")[0]));
            VisibleGroup = transform.FindChildOrCreate("VisibleGroup").gameObject;
            
            SpriteGraphic.sprite = HeroConfigData.HeroSprite;
            SpriteRarity.color = BaseHeroGenerateGlobalConfig.Instance.RarityColorArray[(int)HeroConfigData.HeroRarity];
            HeroAnim.Animator.runtimeAnimatorController = HeroConfigData.AnimatorController;
        }
    }
#endif
}