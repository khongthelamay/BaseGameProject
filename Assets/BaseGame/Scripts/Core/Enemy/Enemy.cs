using LitMotion;
using Manager;
using R3;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomComponent;
using UnityEngine;
using Zenject;

namespace Core
{
    public class Enemy : ACachedMonoBehaviour, IAbilityTargetAble
    {
        public class Factory : PlaceholderFactory<Object ,Enemy>
        {
            public static Factory CreateInstance()
            {
                return new Factory();
            }
        }
        [Inject] private BattleManager BattleManager { get; set; }
        [field: SerializeField] public int CurrentHealthPoint { get; private set; }
        [field: SerializeField] private float MovementSpeed { get; set; }
        [field: SerializeField] public Transform[] MovePoint { get; private set; }
        [field: SerializeField] public int CurrentPoint { get; private set; }
        [field: SerializeField] public ReactiveValue<float> PlaybackSpeed { get; private set; }

        private MotionHandle _movementMotionHandle;

        public Enemy SetupMovePoint(Transform[] movePoint)
        {
            MovePoint = movePoint;
            CurrentPoint = 0;
            Transform.position = MovePoint[CurrentPoint].position;
            PlaybackSpeed.Value = 1;


            return this;
        }

        public void StartMoveToNextPoint()
        {
            Vector3 currentPosition = MovePoint[CurrentPoint].position;
            CurrentPoint = (CurrentPoint + 1) % MovePoint.Length;
            Vector3 targetPosition = MovePoint[CurrentPoint].position;
            float distance = Vector3.Distance(currentPosition, targetPosition);
            float duration = distance / MovementSpeed;

            _movementMotionHandle = LMotion.Create(currentPosition, targetPosition, duration)
                .WithEase(Ease.Linear)
                .WithOnComplete(StartMoveToNextPoint)
                .Bind(OnUpdate);

            _movementMotionHandle.PlaybackSpeed = PlaybackSpeed.Value;
            PlaybackSpeed.ReactiveProperty.Subscribe(OnPlaybackSpeedChanged).AddTo(this);
            return;
            void OnUpdate(Vector3 position)
            {
                Transform.position = position;
            }
        }



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

        public void TakeDamage(int attackDamage)
        {
            attackDamage = (int)(attackDamage * Random.Range(0.2f, 1.8f));
            FactoryManager.Instance.CreateDamageNumber(Transform.position, attackDamage);
            CurrentHealthPoint -= attackDamage;
            if (CurrentHealthPoint <= 0)
            {
                SelfDespawn();
            }
        }
    }
}