using System;
using System.Collections.Generic;
using Core.SimplePool;
using Manager;
using Sirenix.OdinInspector;
using TW.ACacheEverything;
using TW.UGUI.Utility;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using TW.Utility.Extension;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core
{
    [SelectionBase]
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
        private FactoryManager FactoryManagerCache { get; set; }
        protected FactoryManager FactoryManager => FactoryManagerCache ??= FactoryManager.Instance;

        [field: Title(nameof(Hero))]
        [field: SerializeField]
        public HeroConfigData HeroConfigData { get; set; }

        [field: SerializeField] public HeroAttackRange HeroAttackRange { get; set; }
        [field: SerializeField] public SpriteRenderer SpriteGraphic { get; set; }
        [field: SerializeField] public SpriteRenderer SpriteRarity { get; set; }
        [field: SerializeField] public MoveProjectile MoveProjectile { get; private set; }
        [field: SerializeField] public GameObject VisibleGroup { get; private set; }
        [field: SerializeField] public Transform GraphicGroupTransform { get; private set; }

        [field: SerializeField] public FieldSlot FieldSlot { get; private set; }
        [field: SerializeField] public HeroAnim HeroAnim { get; private set; }
        [field: SerializeField] public SortingGroup SortingGroup { get; private set; }


        private StateMachine StateMachine { get; set; }
        [field: SerializeField] private int BaseAttackDamage { get; set; }
        [field: SerializeField] private float BaseAttackSpeed { get; set; }
        [field: SerializeField] private float BaseAttackRange { get; set; }
        [ShowInInspector, ReadOnly] private int BaseCriticalRate { get; set; } = 0;
        [ShowInInspector, ReadOnly] private int BaseCriticalDamage { get; set; } = 150;

        public BigNumber AttackDamage(out bool isCritical)
        {
            BigNumber attackDamage = BaseAttackDamage *
                                     (1 + BattleManager.GetGlobalBuff(GlobalBuff.Type.AttackDamage).Value / 100);
            isCritical = Random.Range(0, 100) < CriticalRate;
            if (isCritical)
            {
                attackDamage *= (CriticalDamage / 100f);
            }

            return attackDamage;
        }

        public float AttackSpeed =>
            BaseAttackSpeed * (1 + BattleManager.GetGlobalBuff(GlobalBuff.Type.AttackSpeed).Value / 100);

        public float AttackRange => BaseAttackRange;
        public float CriticalRate => BaseCriticalRate + BattleManager.GetGlobalBuff(GlobalBuff.Type.CriticalRate).Value;

        public float CriticalDamage =>
            BaseCriticalDamage + BattleManager.GetGlobalBuff(GlobalBuff.Type.CriticalDamage).Value;

        [ShowInInspector, InlineEditor] protected List<Ability> Abilities { get; set; } = new();

        private Vector3 MoveFromPosition { get; set; }
        private Vector3 MoveToPosition { get; set; }
        private Action OnMoveComplete { get; set; }

        [field: SerializeField] private bool IsCancelAbility { get; set; }

        private void Awake()
        {
            InitAbility();
        }

        public void OnDestroy()
        {
            StateMachine.Stop();
        }

        public virtual Hero OnSpawn()
        {
            InitStateMachine();
            InitStat();
            ResetAbility();
            SetVisible(true);
            ResetFacingPosition();
            ResetAttackRange();
            return BattleManager.Instance.RegisterHero(this);
        }

        public virtual void OnDespawn()
        {
            StateMachine.Stop();
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
            foreach (Ability heroAbility in HeroConfigData.HeroAbilities)
            {
                Abilities.Add(heroAbility.Initialize(this));
            }
        }

        private void ResetAbility()
        {
            foreach (Ability ability in Abilities)
            {
                ability.ResetAbility();
            }
        }

        public void SetPosition(Vector3 position)
        {
            Transform.position = position;
        }

        public bool TryGetAbility<T>(out T ability) where T : Ability
        {
            foreach (Ability heroAbility in Abilities)
            {
                if (heroAbility is T tAbility)
                {
                    ability = tAbility;
                    return true;
                }
            }

            ability = null;
            return false;
        }

        public bool IsCurrentState(IState state)
        {
            return StateMachine.IsCurrentState(state);
        }

        public bool IsInBattleState()
        {
            return IsCurrentState(HeroBattleDecisionState) || IsCurrentState(HeroAttackState);
        }

        public bool IsStopStateMachine()
        {
            return !StateMachine.IsRunning;
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
                FactoryManager.CreateSpawnEffect(Transform.position);
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
        public void ForceMoveToFieldSlotAfterFusion(FieldSlot fieldSlot)
        {
            FieldSlot = fieldSlot;
            Transform.SetParent(FieldSlot.Transform);
            Transform.localPosition = Vector3.zero;
            UpdateLayer();
            FieldSlot.FieldSlotChangedCallback?.Invoke();
            StateMachine.RequestTransition(HeroBattleDecisionState);
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
            GraphicGroupTransform.localScale =
                targetPosition.x < Transform.position.x ? VectorFacing[0] : VectorFacing[1];
        }
        public void ResetFacingPosition()
        {
            GraphicGroupTransform.localScale = VectorFacing[0];
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
        public void ResetAttackRange()
        {
            HeroAttackRange.ResetAttackRange();
        }

        private Hero UpdateLayer()
        {
            SortingGroup.sortingOrder = -(int)(Transform.position.y * 100 + Transform.position.x);
            return this;
        }

        public bool TryReduceCooldown(float rate)
        {
            int countCooldownAbility = 0;
            foreach (Ability ability in Abilities)
            {
                if (ability is ActiveCooldownAbility activeCooldownAbility)
                {
                    activeCooldownAbility.ReduceCooldown(rate);
                    countCooldownAbility++;
                }
            }

            return countCooldownAbility > 0;
        }

        public void ChangeCriticalRate(int rate)
        {
            BaseCriticalRate += rate;
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



            HeroAnim = GetComponentInChildren<HeroAnim>();
            MoveProjectile = AssetDatabase.LoadAssetAtPath<MoveProjectile>(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets("t:Prefab MoveProjectile")[0]));
            VisibleGroup = transform.FindChildOrCreate("VisibleGroup").gameObject;
            GraphicGroupTransform = transform.FindChildOrCreate("HeroGraphic").transform;

            SpriteGraphic.sprite = HeroConfigData.HeroSprite;
            
            Transform spriteRarity = GraphicGroupTransform.FindChildOrCreate("SpriteRarity");
            DestroyImmediate(spriteRarity.gameObject);
            if (HeroConfigData.HeroRarity == Rarity.Mythic)
            {
                GameObject aura = AssetDatabase.LoadAssetAtPath<GameObject>(
                    AssetDatabase.GUIDToAssetPath(
                        AssetDatabase.FindAssets("t:Prefab AuraEffect1")[0]));
                GameObject go = PrefabUtility.InstantiatePrefab(aura, GraphicGroupTransform) as GameObject;
                go.name = "SpriteRarity";
            }
            else
            {
                SpriteRarity = GraphicGroupTransform.FindChildOrCreate("SpriteRarity").GetOrAddComponent<SpriteRenderer>();
                SpriteRarity.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                    AssetDatabase.GUIDToAssetPath(
                        AssetDatabase.FindAssets("t:Sprite Shadow Circle")[0]));
                SpriteRarity.color =
                    BaseHeroGenerateGlobalConfig.Instance.RarityColorArray[(int)HeroConfigData.HeroRarity];
                SpriteRarity.transform.eulerAngles = new Vector3(65, 0, 0);
            }

            HeroAnim.Animator.runtimeAnimatorController = HeroConfigData.AnimatorController;
            SortingGroup = GetComponent<SortingGroup>();
        }
    }
#endif
}