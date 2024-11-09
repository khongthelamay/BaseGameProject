using System.Threading;
using BaseGame.Scripts.Enum;
using Cysharp.Threading.Tasks;
using LitMotion;
using Manager;
using TW.ACacheEverything;
using UnityEngine;

namespace Core
{
    public abstract partial class ActiveCooldownAbility : ActiveAbility
    {
        [field: SerializeField] protected float Cooldown { get; set; }
        [field: SerializeField] protected bool IsOnCooldown { get; set; }
        [field: SerializeField] protected float CooldownTimer { get; set; }
        private MotionHandle CooldownHandle { get; set; }
        protected ActiveCooldownAbility(Hero owner, int levelUnlock, float cooldown) : base(owner, levelUnlock)
        {
            Cooldown = cooldown;
            CooldownTimer = Cooldown;
            IsOnCooldown = true;
        }
        

        public void StartCooldownHandle()
        {
            IsOnCooldown = true;
            CooldownHandle = LMotion.Create(CooldownTimer, 0, CooldownTimer)
                .WithOnComplete(OnCooldownHandleCompleteCache)
                .Bind(OnCooldownHandleUpdateCache);
        }

        public void StopCooldownHandle()
        {
            CooldownHandle.TryCancel();
        }

        [ACacheMethod]
        private void OnCooldownHandleUpdate(float time)
        {
            CooldownTimer = time;
        }

        [ACacheMethod]
        private void OnCooldownHandleComplete()
        {
            IsOnCooldown = false;
        }
    }
}