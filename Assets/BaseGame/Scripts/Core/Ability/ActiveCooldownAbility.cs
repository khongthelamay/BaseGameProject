using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public abstract class ActiveCooldownAbility : ActiveAbility
    {
        [field: SerializeField] protected float Cooldown { get; set; }
        [field: SerializeField] protected bool IsOnCooldown { get; set; }
        [field: SerializeField] protected float CooldownTimer { get; set; }
        private CancellationTokenSource CooldownCancellationTokenSource { get; set; }
        protected ActiveCooldownAbility(Hero owner, int levelUnlock, float cooldown) : base(owner, levelUnlock)
        {
            Cooldown = cooldown;
            CooldownTimer = Cooldown;
            IsOnCooldown = true;
        }
        

        public void StartCooldownHandle()
        {
            IsOnCooldown = true;
            CooldownCancellationTokenSource = new CancellationTokenSource();
            ExecuteCooldown(CooldownCancellationTokenSource).Forget();
        }
        private async UniTask ExecuteCooldown(CancellationTokenSource cancellationTokenSource)
        {
            await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                               .WithCancellation(cancellationTokenSource.Token))
            {
                if (CooldownTimer <= 0)
                {
                    IsOnCooldown = false;
                    continue;
                }
                CooldownTimer -= Time.deltaTime;
            }
        }

        public void StopCooldownHandle()
        {
            IsOnCooldown = false;
            CooldownCancellationTokenSource?.Cancel();
            CooldownCancellationTokenSource?.Dispose();
        }
        public void ResetCooldown()
        {
            IsOnCooldown = true;
            CooldownTimer = Cooldown;
        }
        public void ReduceCooldown(float rate)
        {
            CooldownTimer -= Cooldown * rate / 100;
        }
    }
}