using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Core
{
    public class HeroMoveState : IState
    {
        public interface IHandler
        {
            UniTask OnEnter(HeroMoveState state, CancellationToken ct);
            UniTask OnUpdate(HeroMoveState state, CancellationToken ct);
            UniTask OnExit(HeroMoveState state, CancellationToken ct);
        }

        private IHandler Owner { get; set; }

        public HeroMoveState(IHandler owner)
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

    public partial class Hero : HeroMoveState.IHandler
    {
        private HeroMoveState HeroMoveStateCache { get; set; }
        public HeroMoveState HeroMoveState => HeroMoveStateCache ??= new HeroMoveState(this);

        public UniTask OnEnter(HeroMoveState state, CancellationToken ct)
        {
            SetVisible(false);
            MoveProjectile.Spawn(Transform.position, Quaternion.identity)
                .Init(MoveFromPosition, MoveToPosition, OnMoveComplete);
            return UniTask.CompletedTask;
        }

        public UniTask OnUpdate(HeroMoveState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(HeroMoveState state, CancellationToken ct)
        {
            SetVisible(true);
            return UniTask.CompletedTask;
        }
    }
}