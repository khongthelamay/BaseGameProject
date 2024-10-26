using System;
using System.Collections;
using System.Collections.Generic;
using Core.SimplePool;
using Manager;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Core
{
    public partial class Hero : ACachedMonoBehaviour, IAbilityTargetAble
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
        public class Factory : PlaceholderFactory<Object, Hero>
        {
            public static Factory CreateInstance()
            {
                return new Factory();
            }
        }
        
        [Inject] private TargetingManager TargetingManager { get; set; }
        [field: SerializeField] public HeroStatData HeroStatData { get; set; }
        [field: SerializeField] public HeroAttackRange HeroAttackRange { get; set; }
        [field: SerializeField] public SpriteRenderer SpriteGraphic { get; set; }
        [field: SerializeField] public SpriteRenderer SpriteRarity { get; set; }
        [field: SerializeField] public MoveProjectile MoveProjectile { get; private set; }
        [field: SerializeField] public GameObject VisibleGroup {get; private set;}
        [field: SerializeField] public FieldSlot FieldSlot { get; private set; }
        [field: SerializeField] public HeroAnim HeroAnim {get; private set;}
        private StateMachine StateMachine { get; set; }
        public float AttackRange => HeroStatData.BaseAttackRange;
        private Vector3 MoveFromPosition { get; set; }
        private Vector3 MoveToPosition { get; set; }
        private Action OnMoveComplete { get; set; }
        private void Awake()
        {
            InitStateMachine();
            HeroAttackRange.InitAttackRange(HeroStatData.BaseAttackRange);
        }
        public void OnDestroy()
        {
            StateMachine.Stop();
        }
        private void InitStateMachine()
        {
            StateMachine = new StateMachine();
            StateMachine.RegisterState(HeroSleepState);
            StateMachine.Run();
        }

        public bool IsCurrentState(UniTaskState<Hero> state)
        {
            // return StateMachine.IsCurrentState(state);
            return false;
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
            StateMachine.RequestTransition(HeroIdleState);
        }

        public void ShowAttackRange()
        {
            HeroAttackRange.ShowAttackRange();
        }

        public void HideAttackRange()
        {
            HeroAttackRange.HideAttackRange();
        }
        public List<T> GetAbilityTarget<T>(Ability.Target target) where T : IAbilityTargetAble
        {
            return TargetingManager.GetAbilityTarget<T>(this, target);
        }
        
    }

#if UNITY_EDITOR
    public partial class Hero
    {
        [Button]
        public void InitHero()
        {
            SpriteGraphic.sprite = HeroStatData.HeroSprite;
            SpriteRarity.color = BaseHeroGenerateGlobalConfig.Instance.RarityColorArray[(int)HeroStatData.HeroRarity];
            HeroAnim.Animator.runtimeAnimatorController = HeroStatData.AnimatorController;
        }
    }
#endif
}