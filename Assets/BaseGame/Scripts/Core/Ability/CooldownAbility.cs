using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public abstract class CooldownAbility : ActiveAbility
    {
        [field: SerializeField] public float Cooldown { get; set; }
        [ShowInInspector, ReadOnly] public bool IsOnCooldown { get; set; }
        [ShowInInspector, ReadOnly] public float CooldownTimer { get; set; }
        private CancellationTokenSource CooldownCancellationTokenSource { get; set; }

        protected override void SetupGroup()
        {
            base.SetupGroup();
            AbilityGroup |= Group.Cooldown;
        }

        public override void OnEnterBattleField()
        {
            StartCooldownHandle();
        }

        public override void OnExitBattleField()
        {
            StopCooldownHandle();
        }

        public override Ability ResetAbility()
        {
            IsOnCooldown = true;
            CooldownTimer = Cooldown;
            return base.ResetAbility();
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