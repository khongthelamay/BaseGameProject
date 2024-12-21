using System;
using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Manager;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    public class DamageOverTimeArea : ACachedMonoBehaviour, IPoolAble<DamageOverTimeArea>
    {
        private BattleManager BattleManagerCache { get; set; }
        private BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
        [field: SerializeField] public DamageType DamageType {get; private set;}

        [field: SerializeField] public float Radius {get; private set;}
        private Enemy[] Enemies { get; set; } = new Enemy[30];
        private int[] EnemiesTargetId { get; set; } = new int[30];
        protected int EnemiesCount { get; private set; }

        [field: SerializeField] public float Duration {get; protected set;}
        [field: SerializeField] public float TickRate {get; private set;}
        [field: SerializeField] public BigNumber DamagePerTick {get; private set;}
        [field: SerializeField] public bool IsCritical {get; private set;}
        [field: SerializeField] public AxisDetection<Vector2> Range {get; private set;}


        private CancellationTokenSource CancellationTokenSource {get; set;}

        private void OnDestroy()
        {
            StopDamageOverTime();
        }

        public virtual DamageOverTimeArea WithAxis(int axis)
        {
            Range.Axis = axis;
            return this;
        }
        public virtual DamageOverTimeArea Setup(DamageType damageType, float radius, float duration, float tickRate, BigNumber damagePerTick, bool isCritical)
        {
            DamageType = damageType;
            Radius = radius;
            Duration = duration;
            TickRate = tickRate;
            DamagePerTick = damagePerTick;
            IsCritical = isCritical;
            return this;
        }
        public virtual void StartDamageOverTimeHandle()
        {
            StartDamageOverTime().Forget();
        }
        protected virtual void StopDamageOverTime()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
        }

        protected virtual async UniTask StartDamageOverTime()
        {
            StopDamageOverTime();
            CancellationTokenSource = new CancellationTokenSource();
            
            await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                               .WithCancellation(CancellationTokenSource.Token))
            {
                if (!IsFindAnyEnemyTarget()) continue;
                OnDamageOverTimeTick(Enemies, EnemiesTargetId);
                await UniTask.Delay((int)(TickRate * 1000), cancellationToken: CancellationTokenSource.Token);
                Duration -= TickRate;
                if (Duration <= 0)
                {
                    this.Despawn();
                }
            }
        }
        protected virtual void OnDamageOverTimeTick(Enemy[] enemies, int[] enemiesId)
        {
            for (int i = 0; i < EnemiesCount; i++)
            {
                enemies[i].WillTakeDamage(enemiesId[i], DamagePerTick);
                enemies[i].TakeDamage(enemiesId[i], DamagePerTick, DamageType, IsCritical);
            }
        }
        
        public virtual DamageOverTimeArea OnSpawn()
        {
            return this;
        }

        public virtual void OnDespawn()
        {
            StopDamageOverTime();
        }

        private bool IsFindAnyEnemyTarget()
        {
            Vector2 currentRange = Range.Current;
            EnemiesCount = BattleManager.GetEnemyAroundNonAlloc(Transform.position, currentRange, Enemies);
            if (EnemiesCount == 0) return false;
            EnemiesTargetId = new int[EnemiesCount];
            for (int i = 0; i < EnemiesCount; i++)
            {
                EnemiesTargetId[i] = Enemies[i].Id;
            }
            return true;
        }
        private int GetEnemyInRange(Vector2 range)
        {
            return BattleManager.GetEnemyAroundNonAlloc(Transform.position, range, Enemies);
        }

        public T As<T>() where T : DamageOverTimeArea
        {
            return this as T;
        }

        private void OnDrawGizmos()
        {
            Vector2 currentRange = Range.Current;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Transform.position, currentRange);
        }
    }
}