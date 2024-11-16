using System;
using LitMotion;
using LitMotion.Extensions;
using Manager;
using R3;
using TW.ACacheEverything;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;
using Zenject;
using Ease = LitMotion.Ease;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Core
{
    public partial class Enemy : ACachedMonoBehaviour, IAbilityTargetAble
    {
        public class Factory : PlaceholderFactory<Object, Enemy>
        {
            public static Factory CreateInstance()
            {
                return new Factory();
            }
        }

        [Inject] private BattleManager BattleManager { get; set; }
        [field: SerializeField] public BigNumber CurrentHealthPoint { get; private set; }
        [field: SerializeField] public BigNumber FutureHealthPoint { get; private set; }
        [field: SerializeField] private float MovementSpeed { get; set; }
        [field: SerializeField] public Transform[] MovePoint { get; private set; }
        [field: SerializeField] public int CurrentPoint { get; private set; }
        [field: SerializeField] public ReactiveValue<float> PlaybackSpeed { get; private set; }
        [field: SerializeField] public float Deep { get; private set; }
        // [field: SerializeField] public int Damage {get; private set;}
        // [field: SerializeField] public float DPS {get; private set;}


        private MotionHandle _movementMotionHandle;
        public bool WillBeDead => FutureHealthPoint <= 0;
        public bool IsDead => CurrentHealthPoint <= 0;

        // private void Update()
        // {
        //     DelayTime += Time.deltaTime;
        //     if (DelayTime >= 10)
        //     {
        //         DPS = Damage/10f;
        //         Debug.Log("Damage per sec: " + DPS);
        //         DelayTime = 0;
        //         Damage = 0;
        //     }
        // }

        public Enemy SetupMovePoint(Transform[] movePoint)
        {
            Deep = Random.Range(0f, 1f);
            MovePoint = movePoint;
            CurrentPoint = 0;
            Transform.position = MovePoint[CurrentPoint].position;
            PlaybackSpeed.Value = 1;


            return this;
        }
        [ACacheMethod]
        public void StartMoveToNextPoint()
        {
            Vector3 currentPosition = MovePoint[CurrentPoint].position + Vector3.forward * Deep;
            CurrentPoint = (CurrentPoint + 1) % MovePoint.Length;
            Vector3 targetPosition = MovePoint[CurrentPoint].position + Vector3.forward * Deep;
            float distance = Vector3.Distance(currentPosition, targetPosition);
            float duration = distance / MovementSpeed;
            Transform.localScale = currentPosition.x < targetPosition.x ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            _movementMotionHandle = LMotion.Create(currentPosition, targetPosition, duration)
                .WithEase(Ease.Linear)
                .WithOnComplete(StartMoveToNextPointCache)
                .BindToPosition(Transform);
            _movementMotionHandle.PlaybackSpeed = PlaybackSpeed.Value;
            PlaybackSpeed.ReactiveProperty.Subscribe(OnPlaybackSpeedChangedCache).AddTo(this);
        
        }


        [ACacheMethod]
        private void OnPlaybackSpeedChanged(float speed)
        {
            if (!_movementMotionHandle.IsActive()) return;
            _movementMotionHandle.PlaybackSpeed = speed;
        }

        private void SelfDespawn()
        {
            _movementMotionHandle.TryCancel();
            BattleManager.RemoveEnemy(this);
            Destroy(gameObject);
        }

        public void WillTakeDamage(BigNumber attackDamage)
        {
            if (WillBeDead) return;
            FutureHealthPoint -= attackDamage;
        }
        
        public void TakeDamage(BigNumber attackDamage, DamageType damageType)
        {
            if (IsDead) return;
            FactoryManager.Instance.CreateDamageNumber(Transform.position, attackDamage);
            CurrentHealthPoint -= attackDamage;
            if (CurrentHealthPoint <= 0)
            {
                SelfDespawn();
            }
        }
    }
    
    public static class TransformExtension
    {
        private static Action<Vector3, Transform> SetPositionCache { get; set; }
        public static Action<Vector3, Transform> SetPosition => SetPositionCache ??= SetPositionCache = SetPositionFunc;

        private static void SetPositionFunc(Vector3 position, Transform transform)
        {
            transform.position = position;
        }

    }
}