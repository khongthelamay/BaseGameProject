using System;
using DamageNumbersPro;
using R3;
using R3.Triggers;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Manager
{
    public class FactoryManager : Singleton<FactoryManager>
    {
        [field: SerializeField] public DamageNumber DamageNumberMesh {get; private set;}

        private void Start()
        {
            // this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Mouse0))
            //     .Subscribe(_ =>
            //     {
            //         Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //         position.z = 0;
            //         CreateDamageNumber(position, 100);
            //     });
        }

        public void CreateDamageNumber(Vector3 position, int damage)
        {
            DamageNumber damageNumber = DamageNumberMesh.Spawn(position, damage);
        }
    }
}