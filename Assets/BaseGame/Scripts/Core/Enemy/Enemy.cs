using BaseGame.Scripts.Enum;
using LitMotion;
using Manager;
using R3;
using TW.ACacheEverything;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomComponent;
using UnityEngine;
using Zenject;

namespace Core
{
    public partial class Enemy : ACachedMonoBehaviour, IAbilityTargetAble
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
        [field: SerializeField] public int MarkLoseHealthPoint { get; private set; }
        [field: SerializeField] private float MovementSpeed { get; set; }
        [field: SerializeField] public Transform[] MovePoint { get; private set; }
        [field: SerializeField] public int CurrentPoint { get; private set; }
        [field: SerializeField] public ReactiveValue<float> PlaybackSpeed { get; private set; }
        [field: SerializeField] public float Deep {get; private set;}
        private MotionHandle m_MovementMotionHandle;
        public bool WillBeDead => MarkLoseHealthPoint <= 0;
        public Enemy SetupMovePoint(Transform[] movePoint)
        {
            Deep = Random.Range(0f,1f);
            MovePoint = movePoint;
            CurrentPoint = 0;
            Transform.position = MovePoint[CurrentPoint].position;
            PlaybackSpeed.Value = 1;


            return this;
        }
        [ACacheMethod]
        public void StartMoveToNextPoint()
        {
            Vector3 currentPosition = MovePoint[CurrentPoint].position;
            CurrentPoint = (CurrentPoint + 1) % MovePoint.Length;
            Vector3 targetPosition = MovePoint[CurrentPoint].position;
            float distance = Vector3.Distance(currentPosition, targetPosition);
            float duration = distance / MovementSpeed;

            m_MovementMotionHandle = LMotion.Create(currentPosition, targetPosition, duration)
                .WithEase(Ease.Linear)
                .WithOnComplete(StartMoveToNextPointCache)
                .Bind(OnMoveUpdateCache);

            m_MovementMotionHandle.PlaybackSpeed = PlaybackSpeed.Value;
            PlaybackSpeed.ReactiveProperty.Subscribe(OnPlaybackSpeedChangedCache).AddTo(this);
            return;

        }
        [ACacheMethod]
        private void OnMoveUpdate(UnityEngine.Vector3 position)
        {
            Vector3 currentPosition = Transform.position;
            Transform.position = position + Vector3.forward * Deep;
            if (currentPosition.x < position.x)
            {
                Transform.localScale = new Vector3(1, 1, 1);
            }
            else if (currentPosition.x > position.x)
            {
                Transform.localScale = new Vector3(-1, 1, 1);
            }
        }


        [ACacheMethod]
        private void OnPlaybackSpeedChanged(float speed)
        {
            if (!m_MovementMotionHandle.IsActive()) return;
            m_MovementMotionHandle.PlaybackSpeed = speed;
        }

        private void SelfDespawn()
        {
            m_MovementMotionHandle.TryCancel();
            BattleManager.RemoveEnemy(this);
            Destroy(gameObject);
        }
        public void MarkLoseHealth(int attackDamage)
        {
            MarkLoseHealthPoint -= attackDamage;
        }
        public void TakeDamage(int attackDamage, DamageType damageType)
        {
            // attackDamage = (int)(attackDamage * Random.Range(0.2f, 1.8f));
            FactoryManager.Instance.CreateDamageNumber(Transform.position, attackDamage);
            CurrentHealthPoint -= attackDamage;
            if (CurrentHealthPoint <= 0)
            {
                SelfDespawn();
            }
        }
    }
}