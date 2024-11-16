using System;
using R3;
using TW.ACacheEverything;
using UnityEngine;

namespace Core
{
    public partial class FuryPowerAbility : PassiveAbility
    {
        [field: SerializeField] private float AttackDamageScalePerFury { get; set; } = 0.25f;
        [field: SerializeField] private float CriticalDamageScalePerFury { get; set; } = 0.25f;
        [field: SerializeField] private float AttackDamageGain { get; set; }
        [field: SerializeField] private float CriticalDamageGain { get; set; }
        private IDisposable Disposable { get; set; }
        private Tiger OwnerTiger { get; set; }
        public FuryPowerAbility(Hero owner, int levelUnlock) : base(owner, levelUnlock)
        {
            OwnerTiger = (Tiger) owner;
        }

        public override void OnEnterBattleField()
        {
            AttackDamageGain = AttackDamageScalePerFury * OwnerTiger.FuryPoint.Value;
            CriticalDamageGain = CriticalDamageScalePerFury * OwnerTiger.FuryPoint.Value;
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.AttackDamage, AttackDamageGain);
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.CriticalDamage, CriticalDamageGain);
            Disposable = OwnerTiger.FuryPoint.ReactiveProperty.Pairwise().Subscribe(OnFuryPointChangeCache).AddTo(Owner);
        }

        public override void OnExitBattleField()
        {
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.AttackDamage, -AttackDamageGain);
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.CriticalDamage, -CriticalDamageGain);
            AttackDamageGain = 0;
            CriticalDamageGain = 0;
            Disposable.Dispose();
            Disposable = null;
        }
        [ACacheMethod]
        private void OnFuryPointChange((int last, int cur) pair)
        {
            AttackDamageGain = AttackDamageScalePerFury * pair.cur ;
            CriticalDamageGain = CriticalDamageScalePerFury * pair.cur;
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.AttackDamage, (pair.cur - pair.last) * AttackDamageScalePerFury);
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.CriticalDamage, (pair.cur - pair.last) * CriticalDamageScalePerFury);
        }
    }
}