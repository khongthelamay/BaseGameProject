﻿using Core;
using UnityEngine;
using Zenject;

namespace Manager
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<BattleManager>().FromInstance(BattleManager.Instance).AsSingle();
            
            Container.BindFactory<Object, Enemy, Enemy.Factory>().FromFactory<PrefabFactory<Enemy>>();
            
        }
    }
}
