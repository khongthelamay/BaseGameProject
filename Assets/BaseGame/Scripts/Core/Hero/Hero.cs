using System;
using System.Collections.Generic;
using Core.SimplePool;
using Manager;
using Sirenix.OdinInspector;
using TW.ACacheEverything;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using TW.Utility.Extension;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core
{
    public partial class Hero : ACachedMonoBehaviour, IAbilityTargetAble, IPoolAble<Hero> 
    {
        private static Vector3[] VectorFacing { get; set; } = new Vector3[2]
        {
            new Vector3(1, 1, 1),
            new Vector3(-1, 1, 1),
        };
        
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
        private BattleManager BattleManagerCache { get; set; }
        protected BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
        
        [field: Title(nameof(Hero))]
        [field: SerializeField] public HeroConfigData HeroConfigData { get; set; }
        [field: SerializeField] public HeroAttackRange HeroAttackRange { get; set; }
        [field: SerializeField] public SpriteRenderer SpriteGraphic { get; set; }
        [field: SerializeField] public SpriteRenderer SpriteRarity { get; set; }
        [field: SerializeField] public MoveProjectile MoveProjectile { get; private set; }
        [field: SerializeField] public GameObject VisibleGroup {get; private set;}
        [field: SerializeField] public Transform GraphicGroupTransform {get; private set;}

        [field: SerializeField] public FieldSlot FieldSlot { get; private set; }
        [field: SerializeField] public HeroAnim HeroAnim {get; private set;}
        [field: SerializeField] public SortingGroup SortingGroup {get; private set;}

        
        private StateMachine StateMachine { get; set; }
        [field: SerializeField] private int BaseAttackDamage { get; set; }
        [field: SerializeField] private float BaseAttackSpeed { get; set; }
        [field: SerializeField] private float BaseAttackRange { get; set; }
        
        public BigNumber AttackDamage => BaseAttackDamage * (1 + BattleManager.GetGlobalBuff(GlobalBuff.Type.AttackDamage).Value/100);
        public float AttackSpeed => BaseAttackSpeed * (1 + BattleManager.GetGlobalBuff(GlobalBuff.Type.AttackSpeed).Value/100);
        public float AttackRange => BaseAttackRange;
        [ShowInInspector]
        protected List<ActiveAbility> ActiveAbilities { get; set; } = new();
        [ShowInInspector]
        protected List<PassiveAbility> PassiveAbilities { get; set; } = new();
        
        private Vector3 MoveFromPosition { get; set; }
        private Vector3 MoveToPosition { get; set; }
        private Action OnMoveComplete { get; set; }
        
        [field: SerializeField] private bool IsCancelAbility {get; set;}

        public void OnDestroy()
        {
            StateMachine.Stop();
        }
        public virtual Hero OnSpawn()
        {
            InitStateMachine();
            InitStat();
            InitAbility();
            
            
            return BattleManager.Instance.RegisterHero(this);
        }

        public virtual void OnDespawn()
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
            BaseAttackDamage = HeroConfigData.BaseAttackDamage;
            BaseAttackSpeed = HeroConfigData.BaseAttackSpeed;
            BaseAttackRange = HeroConfigData.BaseAttackRange;
        }

        protected virtual void InitAbility()
        {
            ActiveAbilities.Clear();
            PassiveAbilities.Clear();
            foreach (Ability heroAbility in HeroConfigData.HeroAbilities)
            {
                if (heroAbility is ActiveAbility)
                {
                    ActiveAbilities.Add(heroAbility.Clone(this) as ActiveAbility);
                }
                else
                {
                    PassiveAbilities.Add(heroAbility.Clone(this) as PassiveAbility);
                }
            }
        }
        public bool IsCurrentState(IState state)
        {
            return StateMachine.IsCurrentState(state);
        }
        public bool IsInBattleState()
        {
            return IsCurrentState(HeroBattleDecisionState) || IsCurrentState(HeroAttackState);
        }

        [ACacheMethod]
        private bool IsCancelAbilityMethod()
        {
            return IsCancelAbility;
        }
        public void ChangeToSleepState()
        {
            StateMachine.RequestTransition(HeroSleepState);
        }
        public void MoveToFieldSlot(FieldSlot fieldSlot)
        {
            SetCancelAbility(true);
            MoveFromPosition = Transform.position;
            MoveToPosition = fieldSlot.Transform.position;
            OnMoveComplete = MoveComplete;
            FieldSlot = fieldSlot;
            Transform.SetParent(FieldSlot.Transform);
            
            StateMachine.RequestTransition(HeroMoveState);
            return;
            void MoveComplete()
            {
                SetCancelAbility(false);
                Transform.localPosition = Vector3.zero;
                UpdateLayer();
                FieldSlot.FieldSlotChangedCallback?.Invoke();
                StateMachine.RequestTransition(HeroBattleDecisionState);
            }
        }
        public void MoveToPositionAndSelfDespawn(Vector3 toPosition)
        {
            SetCancelAbility(true);
            MoveFromPosition = Transform.position;
            MoveToPosition = toPosition;
            OnMoveComplete = MoveComplete;
            
            Transform.localPosition = Vector3.zero;
            
            StateMachine.RequestTransition(HeroMoveState);
            return;
            void MoveComplete()
            {
                SetCancelAbility(false);
                this.Despawn();
            }
        }
        public void SetVisible(bool isVisible)
        {
            VisibleGroup.SetActive(isVisible);
        }
        public void SetCancelAbility(bool isCancelAbility)
        {
            IsCancelAbility = isCancelAbility;
        }
        public void SetFacingPosition(Vector3 targetPosition)
        {
            GraphicGroupTransform.localScale = targetPosition.x < Transform.position.x ? VectorFacing[0] : VectorFacing[1];
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
        private Hero UpdateLayer()
        {
            SortingGroup.sortingOrder = -(int) (Transform.position.y * 100 + Transform.position.x);
            return this;
        }
        public bool TryReduceCooldown(float rate)
        {
            int countCooldownAbility = 0;
            foreach (ActiveAbility activeAbility in ActiveAbilities)
            {
                if (activeAbility is ActiveCooldownAbility activeCooldownAbility)
                {
                    activeCooldownAbility.ReduceCooldown(rate);
                    countCooldownAbility++;
                }
            }
            return countCooldownAbility > 0;
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
            GraphicGroupTransform = transform.FindChildOrCreate("HeroGraphic").transform;
            
            SpriteGraphic.sprite = HeroConfigData.HeroSprite;
            SpriteRarity.color = BaseHeroGenerateGlobalConfig.Instance.RarityColorArray[(int)HeroConfigData.HeroRarity];
            HeroAnim.Animator.runtimeAnimatorController = HeroConfigData.AnimatorController;
            SortingGroup = GetComponent<SortingGroup>();
        }
    }
#endif
}