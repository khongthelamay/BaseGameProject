using System;
using Core.GameStatusEffect;
using Core.SimplePool;
using LitMotion;
using LitMotion.Extensions;
using Manager;
using R3;
using Sirenix.OdinInspector;
using TW.ACacheEverything;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEngine;
using Ease = LitMotion.Ease;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Core
{
    public partial class Enemy : ACachedMonoBehaviour, IPoolAble<Enemy>, IAbilityTargetAble, IStatusEffectAble, ISlowAble, IStunAble
    {
        private static int EnemyIdCounter { get; set; } = 0;
        private BattleManager BattleManagerCache { get; set; }
        private BattleManager BattleManager => BattleManagerCache ??= BattleManagerCache = BattleManager.Instance;
        [field: SerializeField] public Animator Animator {get; private set;}
        [field: SerializeField] public SpriteRenderer Graphic {get; private set;}
        [field: SerializeField] public int Id {get; private set;}
        [field: SerializeField] public StatusEffectStack StatusEffectStack {get; private set;}
        [field: SerializeField] public BigNumber CurrentHealthPoint { get; private set; }
        [field: SerializeField] public BigNumber FutureHealthPoint { get; private set; }
        [field: SerializeField] private float MovementSpeed { get; set; }
        [field: SerializeField] public Transform[] MovePoint { get; private set; }
        [field: SerializeField] public int CurrentPoint { get; private set; }
        [field: SerializeField] public ReactiveValue<float> PlaybackSpeed { get; private set; }
        [field: SerializeField] public float Deep { get; private set; }
        [field: SerializeField] public int PhysicalArmor { get; private set; }
        [field: SerializeField] public int MagicalArmor { get; private set; }
        [field: SerializeField, ReadOnly] public SerializableReactiveProperty<float> SlowAmount { get; set; }
        [field: SerializeField, ReadOnly] public SerializableReactiveProperty<bool> IsStun { get; set; }
        public int MoveAxis { get; private set; }
        private MotionHandle movementMotionHandle;
        public bool WillBeDead => FutureHealthPoint <= 0;
        public bool IsDead => CurrentHealthPoint <= 0;

        private void Awake()
        {
            SlowAmount = new SerializableReactiveProperty<float>(0);
            SlowAmount.Subscribe(OnSlowAmountChangedCache).AddTo(this);
            IsStun = new SerializableReactiveProperty<bool>(false);
            IsStun.Subscribe(OnStunChangedCache).AddTo(this);
        }

        private void OnDestroy()
        {
            StatusEffectStack.Stop();
        }

        public Enemy InitStats(BigNumber healthPoint, float movementSpeed)
        {
            CurrentHealthPoint = healthPoint;
            FutureHealthPoint = healthPoint;
            MovementSpeed = movementSpeed;
            SlowAmount.Value = 0;
            
            return this;
        }
        public Enemy InitStatusEffectStack()
        {
            StatusEffectStack = new StatusEffectStack(this);
            StatusEffectStack.Run();
            return this;
        }
        public Enemy SetupMovePoint(Transform[] movePoint)
        {
            Deep = Random.Range(0f, 1f);
            MovePoint = movePoint;
            CurrentPoint = 0;
            Transform.position = MovePoint[0].position;
            PlaybackSpeed.Value = 1;

            return this;
        }
        [ACacheMethod]
        public void StartMoveToNextPoint()
        {
            Vector3 currentPosition = MovePoint[CurrentPoint].position + Vector3.forward * Deep;
            CurrentPoint = (CurrentPoint + 1) % MovePoint.Length;
            Vector3 targetPosition = MovePoint[CurrentPoint].position + Vector3.forward * Deep;
            Vector3 moveDirection = (targetPosition - currentPosition).normalized;
            MoveAxis = Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y) ? 1 : -1;
            float distance = Vector3.Distance(currentPosition, targetPosition);
            float duration = distance / MovementSpeed;
            Transform.localScale = currentPosition.x < targetPosition.x ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            movementMotionHandle = LMotion.Create(currentPosition, targetPosition, duration)
                .WithEase(Ease.Linear)
                .WithOnComplete(StartMoveToNextPointCache)
                .BindToPosition(Transform);
            movementMotionHandle.PlaybackSpeed = PlaybackSpeed.Value;
            PlaybackSpeed.ReactiveProperty.Subscribe(OnPlaybackSpeedChangedCache).AddTo(this);
        }
        
        [ACacheMethod]
        private void OnPlaybackSpeedChanged(float speed)
        {
            if (!movementMotionHandle.IsActive()) return;
            movementMotionHandle.PlaybackSpeed = speed;
            Animator.speed = speed;
        }
        public bool WillTakeDamage(int id, BigNumber attackDamage)
        {
            if (id != Id) return false;
            if (WillBeDead) return false;
            FutureHealthPoint -= attackDamage;
            return true;
        }
        
        public bool TakeDamage(int id, BigNumber attackDamage, DamageType damageType, bool isCritical)
        {
            if (id != Id) return false;
            if (IsDead) return false;
            BigNumber finalDamage = attackDamage * GetArmorMultiplier(damageType);
            FactoryManager.Instance.CreateDamageNumber(Transform.position, finalDamage, damageType, isCritical);
            CurrentHealthPoint -= finalDamage;
            if (CurrentHealthPoint <= 0)
            {
                this.Despawn();
            }
            return true;
        }
        private float GetArmorMultiplier(DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Physical:
                    float physicalArmorReduction = BattleManager.GetGlobalBuff(GlobalBuff.Type.PhysicalArmorReduction).Value;
                    return DamageMultiplier.GetDamageMultiplier(PhysicalArmor - (int)physicalArmorReduction);
                case DamageType.Magical:
                    float magicalArmorReduction = BattleManager.GetGlobalBuff(GlobalBuff.Type.MagicArmorReduction).Value;
                    return DamageMultiplier.GetDamageMultiplier(MagicalArmor - (int)magicalArmorReduction);
                default:
                    return 1;
            }
        }
        [ACacheMethod]
        public void OnSlowAmountChanged(float slowAmount)
        {
            PlaybackSpeed.Value = IsStun.Value ? 0 : MovementSpeedMultiplier.GetMovementSpeedMultiplier(slowAmount);
        }
        [ACacheMethod]
        public void OnStunChanged(bool isStun)
        {
            PlaybackSpeed.Value = isStun ? 0 : MovementSpeedMultiplier.GetMovementSpeedMultiplier(SlowAmount.Value);
        }
        public Enemy OnSpawn()
        {
            Id = EnemyIdCounter++;
            if (EnemyIdCounter >= 10000)
            {   
                EnemyIdCounter = 0;
            }
            BattleManager.RegisterEnemy(this);
            InitStatusEffectStack();
            return this;
        }

        public void OnDespawn()
        {
            movementMotionHandle.TryCancel();
            StatusEffectStack.Stop();
            BattleManager.UnregisterEnemy(this);
        }


        public void AddStatusEffect(StatusEffect statusEffect)
        {
            StatusEffectStack.Add(statusEffect);
        }

        public void RemoveStatusEffect(StatusEffect statusEffect)
        {
            StatusEffectStack.Remove(statusEffect);
        }
        public void ChangeColor(Color color)
        {
            Graphic.color = color;
        }
    }
    
    public static class TransformExtension
    {
        private static Action<Vector3, Transform> SetPositionCache { get; set; }
        public static Action<Vector3, Transform> SetPosition => SetPositionCache ??= SetPositionCache = SetPositionFunc;

        private static void SetPositionFunc(Vector3 position, Transform transform)
        {
            transform.position = position;
        }

    }
}