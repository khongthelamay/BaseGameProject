﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "RallyingFuryAbility", menuName = "Ability/Boar/RallyingFuryAbility")]
    public class RallyingFuryAbility : ActiveAbility
    {
        public override void OnEnterBattleField()
        {
        }

        public override void OnExitBattleField()
        {
        }

        public override bool CanUseAbility()
        {
            return false;
        }

        public override UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    }
}