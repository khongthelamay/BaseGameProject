using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.GameStatusEffect
{
    [System.Serializable]

    public class StatusEffectStack
    {
        private IStatusEffectAble Owner { get; set; }
        private bool IsRunning { get; set; } = false;
        private Queue<StatusEffect> PendingStatusEffect { get; set; } = new();
        private Queue<StatusEffect> PendingRemoveStatusEffect { get; set; } = new();
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private List<StatusEffect> StatusEffectList { get; set; }

#if UNITY_EDITOR
        [ShowInInspector]
        private List<StatusEffect> StatusEffectListEditor => StatusEffectList?.ToList();
#endif
        public StatusEffectStack(IStatusEffectAble owner)
        {
            Owner = owner;
            StatusEffectList = new List<StatusEffect>();
            IsRunning = false;
        }

        public void Run()
        {
            CancellationTokenSource = new CancellationTokenSource();
            IsRunning = true;
            Execute().Forget();
        }
        public void Clear()
        {
            PendingStatusEffect.Clear();
            PendingRemoveStatusEffect.Clear();
            foreach (StatusEffect statusEffect in StatusEffectList)
            {
                statusEffect.OnRemove(Owner);
            }
            StatusEffectList.Clear();
        }
        public void Stop()
        {
            if (!IsRunning) return;
            StatusEffectList.Clear();
            IsRunning = false;
            CancellationTokenSource.Cancel();
            CancellationTokenSource.Dispose();
        }

        private async UniTask Execute()
        {
            await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                               .WithCancellation(CancellationTokenSource.Token))
            {
                AddPendingStatusEffect();
                RemovePendingStatusEffect();

                foreach (StatusEffect statusEffect in StatusEffectList)
                {
                    statusEffect.Execute(Owner);
                }
            }
        }

        private void AddPendingStatusEffect()
        {
            while (PendingStatusEffect.Count > 0)
            {
                StatusEffect statusEffect = PendingStatusEffect.Dequeue();
                statusEffect.OnAdd(Owner);
                if (statusEffect.IsStackable)
                {
                    StatusEffectList.Add(statusEffect);
                }
                else if (TryGetStatusEffect(statusEffect.StatusEffectType, out StatusEffect currentStatusEffect))
                {
                    currentStatusEffect.Overlap(Owner, statusEffect);
                }
                else
                {
                    StatusEffectList.Add(statusEffect);
                }
            }
        }

        private void RemovePendingStatusEffect()
        {
            while (PendingRemoveStatusEffect.Count > 0)
            {
                StatusEffect statusEffect = PendingRemoveStatusEffect.Dequeue();
                statusEffect.OnRemove(Owner);
                StatusEffectList.Remove(statusEffect);
            }
        }

        private bool TryGetStatusEffect(StatusEffect.Type statusEffectType, out StatusEffect statusEffect)
        {
            foreach (StatusEffect status in StatusEffectList)
            {
                if (status.StatusEffectType != statusEffectType) continue;
                statusEffect = status;
                return true;
            }

            statusEffect = null;
            return false;
        }

        public void Add(StatusEffect statusEffect)
        {
            PendingStatusEffect.Enqueue(statusEffect);
        }

        public void Remove(StatusEffect statusEffect)
        {
            PendingRemoveStatusEffect.Enqueue(statusEffect);
        }
    }
}