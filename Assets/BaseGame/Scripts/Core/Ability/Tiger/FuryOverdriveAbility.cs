using System;
using R3;
using TW.ACacheEverything;
using UnityEngine;

namespace Core.TigerAbility
{
    [CreateAssetMenu(fileName = "FuryOverdriveAbility", menuName = "Ability/Tiger/FuryOverdriveAbility")]
    public partial class FuryOverdriveAbility : PassiveAbility
    {
        [field: SerializeField] public float AttackDamageScalePerFury { get; private set; } = 0.25f;
        [field: SerializeField] public float CriticalDamageScalePerFury { get; private set; } = 0.25f;
        private float AttackDamageGain { get; set; }
        private float CriticalDamageGain { get; set; }
        private IDisposable Disposable { get; set; }
        private Tiger OwnerTiger { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerTiger = owner as Tiger;
            return base.WithOwnerHero(owner);
        }

        public override void OnEnterBattleField()
        {
            AttackDamageGain = AttackDamageScalePerFury * OwnerTiger.FuryPoint.Value;
            CriticalDamageGain = CriticalDamageScalePerFury * OwnerTiger.FuryPoint.Value;
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.AttackDamage, AttackDamageGain);
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.CriticalDamage, CriticalDamageGain);
            Disposable = OwnerTiger.FuryPoint.ReactiveProperty.Pairwise().Subscribe(OnFuryPointChangeCache)
                .AddTo(Owner);
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
            AttackDamageGain = AttackDamageScalePerFury * pair.cur;
            CriticalDamageGain = CriticalDamageScalePerFury * pair.cur;
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.AttackDamage,
                (pair.cur - pair.last) * AttackDamageScalePerFury);
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.CriticalDamage,
                (pair.cur - pair.last) * CriticalDamageScalePerFury);
        }
    }
}