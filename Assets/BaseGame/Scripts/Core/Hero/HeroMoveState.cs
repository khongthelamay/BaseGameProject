using System.Threading;
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
            Owner.OnEnter(this, ct);
            return UniTask.CompletedTask;
        }

        public UniTask OnUpdate(CancellationToken ct)
        {
            Owner.OnUpdate(this, ct);
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(CancellationToken ct)
        {
            Owner.OnExit(this, ct);
            return UniTask.CompletedTask;
        }
    }

    public partial class Hero : HeroMoveState.IHandler
    {
        private HeroMoveState HeroMoveStateCache { get; set; }
        public HeroMoveState HeroMoveState => HeroMoveStateCache ??= new HeroMoveState(this);

        public UniTask OnEnter(HeroMoveState state, CancellationToken ct)
        {
            // MoveProjectile.Spawn(Transform.position, Quaternion.identity)
            //     .Init(MoveFromPosition, MoveToPosition, OnMoveComplete);
            Instantiate(MoveProjectile, Transform.position, Quaternion.identity)
                .Init(MoveFromPosition, MoveToPosition, OnMoveComplete);
            return UniTask.CompletedTask;
        }

        public UniTask OnUpdate(HeroMoveState state, CancellationToken ct)
        { 
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(HeroMoveState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    }
}