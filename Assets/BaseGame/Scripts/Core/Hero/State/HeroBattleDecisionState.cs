using System.Threading;
using Cysharp.Threading.Tasks;
using Manager;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Core
{
    public class HeroBattleDecisionState : IState
    {
        public interface IHandler
        {
            UniTask OnEnter(HeroBattleDecisionState state, CancellationToken ct);
            UniTask OnUpdate(HeroBattleDecisionState state, CancellationToken ct);
            UniTask OnExit(HeroBattleDecisionState state, CancellationToken ct);
        }

        private IHandler Owner { get; set; }

        public HeroBattleDecisionState(IHandler owner)
        {
            Owner = owner;
        }

        public UniTask OnEnter(CancellationToken ct)
        {
            return Owner.OnEnter(this, ct);
        }

        public UniTask OnUpdate(CancellationToken ct)
        {
            return Owner.OnUpdate(this, ct);
        }

        public UniTask OnExit(CancellationToken ct)
        {
            return Owner.OnExit(this, ct);
        }
    }

    public partial class Hero : HeroBattleDecisionState.IHandler
    {
        private HeroBattleDecisionState HeroBattleDecisionStateCache { get; set; }

        public HeroBattleDecisionState HeroBattleDecisionState =>
            HeroBattleDecisionStateCache ??= new HeroBattleDecisionState(this);

        public UniTask OnEnter(HeroBattleDecisionState state, CancellationToken ct)
        {
            foreach (var passiveAbility in PassiveAbilities)
            {
                passiveAbility.OnEnterBattleField();
            }

            return UniTask.CompletedTask;
        }

        public async UniTask OnUpdate(HeroBattleDecisionState state, CancellationToken ct)
        {
            foreach (var activeAbility in ActiveAbilities)
            {
                if (!activeAbility.CanUseAbility()) continue;
                await activeAbility.UseAbility(BattleManager.Instance.TickRate, ct);
                break;
            }
        }

        public UniTask OnExit(HeroBattleDecisionState state, CancellationToken ct)
        {
            foreach (var passiveAbility in PassiveAbilities)
            {
                passiveAbility.OnExitBattleField();
            }

            return UniTask.CompletedTask;
        }
    }
}