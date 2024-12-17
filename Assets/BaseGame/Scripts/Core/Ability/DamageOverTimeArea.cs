using System;
using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Manager;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    public class DamageOverTimeArea : ACachedMonoBehaviour, IPoolAble<DamageOverTimeArea>
    {
        private BattleManager BattleManagerCache { get; set; }
        public BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
        public Hero Owner { get; private set; }
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public float Radius {get; private set;}

        public Enemy[] Enemies { get; set; } = new Enemy[30];
        public int[] EnemiesTargetId { get; set; } = new int[30];
        public int EnemiesCount { get; set; }

        [field: SerializeField] public float Duration {get; private set;}
        [field: SerializeField] public float TickRate {get; private set;}
        [field: SerializeField] public BigNumber DamagePerTick {get; private set;}
        [field: SerializeField] public bool IsCritical {get; private set;}

        private CancellationTokenSource CancellationTokenSource {get; set;}
        
        public virtual DamageOverTimeArea WithAxis(int axis)
        {
            return this;
        }
        public DamageOverTimeArea Setup(DamageType damageType, float radius, float duration, float tickRate, BigNumber damagePerTick, bool isCritical)
        {
            DamageType = damageType;
            Radius = radius;
            Duration = duration;
            TickRate = tickRate;
            DamagePerTick = damagePerTick;
            IsCritical = isCritical;
            StartDamageOverTime().Forget();
            return this;
        }
        public virtual void StopDamageOverTime()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
        }
        public virtual async UniTask StartDamageOverTime()
        {
            StopDamageOverTime();
            CancellationTokenSource = new CancellationTokenSource();
            
            await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                               .WithCancellation(CancellationTokenSource.Token))
            {
                if (!IsFindAnyEnemyTarget()) continue;
                for (int i = 0; i < EnemiesCount; i++)
                {
                    Enemies[i].WillTakeDamage(EnemiesTargetId[i], DamagePerTick);
                    Enemies[i].TakeDamage(EnemiesTargetId[i], DamagePerTick, DamageType, IsCritical);
                }
                await UniTask.Delay((int)(TickRate * 1000), cancellationToken: CancellationTokenSource.Token);
                Duration -= TickRate;
                if (Duration <= 0)
                {
                    this.Despawn();
                }
            }
        }
        
        
        public DamageOverTimeArea OnSpawn()
        {
            return this;
        }

        public void OnDespawn()
        {
            StopDamageOverTime();
        }

        private bool IsFindAnyEnemyTarget()
        {
            EnemiesCount = BattleManager.GetEnemyAroundNonAlloc(Transform.position, Radius, Enemies);
            if (EnemiesCount == 0) return false;
            EnemiesTargetId = new int[EnemiesCount];
            for (int i = 0; i < EnemiesCount; i++)
            {
                EnemiesTargetId[i] = Enemies[i].Id;
            }
            return true;
        }
    }
}