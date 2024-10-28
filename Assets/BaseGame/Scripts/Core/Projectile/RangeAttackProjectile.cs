using BaseGame.Scripts.Enum;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

namespace Core
{
    public class RangeAttackProjectile : Projectile
    {
        private delegate void UpdatePosition(Vector3 currentPosition, Vector3 distance, Vector3 targetPosition, out bool isComplete);
        
        [field: SerializeField] public Transform ShadowTransform { get; private set; }
        [field: SerializeField] public Vector3 CurrentPosition { get; private set; }
        [field: SerializeField] private float MaxMoveSpeed { get; set; }
        [field: SerializeField] public float TrajectoryMaxHeight { get; private set; }
        [field: SerializeField] public AnimationCurve Trajectory { get; private set; }
        [field: SerializeField] public AnimationCurve Axis { get; private set; }
        [field: SerializeField] public AnimationCurve Velocity { get; private set; }
        [field: SerializeField] public AnimationCurve Scale { get; private set; }
        [field: SerializeField] public TrailRenderer TrailRenderer {get; private set;}

        private int AttackDamage { get; set; }
        private Vector3 StartPosition { get; set; }
        private Vector3 EndPosition { get; set; }
        private float CurrentMoveSpeed { get; set; }
        private UpdatePosition UpdatePositionCallback { get; set; }
        public override Projectile OnSpawn()
        {
            TrailRenderer.Clear();
            return this;
        }

        public override void OnDespawn()
        {
            TrailRenderer.Clear();
        }

        public override Projectile Setup(Hero ownerHero, Enemy targetEnemy, DamageType damageType)
        {
            base.Setup(ownerHero, targetEnemy, damageType);
            
            Transform.localScale = Vector3.zero;
            StartPosition = Transform.position;
            CurrentPosition = StartPosition;
            AttackDamage = OwnerHero.HeroConfigData.BaseAttackDamage;
            CurrentMoveSpeed = MaxMoveSpeed;
            EndPosition = TargetEnemy.Transform.position;
            StartMoveToTarget().Forget();
            return this;
        }

        private async UniTask StartMoveToTarget()
        {
            UpdatePositionCallback = SetupUpdatePositionCallback();
            await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                               .WithCancellation(this.GetCancellationTokenOnDestroy()))
            {
                TryUpdateEndPosition();
                if (TryMoveToTarget()) continue;
                
                OnCompleteCallback?.Invoke();
                this.Despawn();
                break;
            }
        }

        private UpdatePosition SetupUpdatePositionCallback()
        {
            Vector3 targetPosition = TargetEnemy.Transform.position;
            Vector3 distance = targetPosition - StartPosition;
            Vector3 distanceNormalized = distance.normalized;
            return Mathf.Abs(distanceNormalized.x) < Mathf.Abs(distanceNormalized.y)
                ? UpdatePositionWithXCurve
                : UpdatePositionWithYCurve;
        }

        private void TryUpdateEndPosition()
        {
            if (TargetEnemy != null)
            {
                EndPosition = TargetEnemy.Transform.position;
            }
        }

        private bool TryMoveToTarget()
        {
            Vector3 currentPosition = Transform.position;
            Vector3 targetPosition = EndPosition;
            Vector3 distance = targetPosition - StartPosition;
            UpdatePositionCallback(currentPosition, distance, targetPosition, out bool isComplete);
            return !isComplete;
        }

        private void UpdatePositionWithYCurve(Vector3 currentPosition, Vector3 distance, Vector3 targetPosition,
            out bool isComplete)
        {
            Vector3 nextPosition = currentPosition;
            nextPosition.x = currentPosition.x + CurrentMoveSpeed * Time.deltaTime * Mathf.Sign(distance.x);
            float nextPositionXNormalized = Mathf.Abs((nextPosition.x - StartPosition.x) / distance.x);
            float nextPositionYAxis = Axis.Evaluate(nextPositionXNormalized) * distance.y;
            float trajectoryValue = Trajectory.Evaluate(nextPositionXNormalized) * TrajectoryMaxHeight *
                                    Mathf.Abs(distance.x);

            nextPosition.y = StartPosition.y + trajectoryValue + nextPositionYAxis;

            Vector3 direction = nextPosition - currentPosition;
            Transform.position = nextPosition;
            Transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            Transform.localScale = Vector3.one * Scale.Evaluate(nextPositionXNormalized);

            ShadowTransform.position = Vector3.Lerp(StartPosition, targetPosition, nextPositionXNormalized);
            ShadowTransform.rotation = Quaternion.FromToRotation(Vector3.up, distance);

            CurrentMoveSpeed = Velocity.Evaluate(nextPositionXNormalized) * MaxMoveSpeed;
            isComplete = Mathf.Abs(nextPosition.x - StartPosition.x) > Mathf.Abs(targetPosition.x - StartPosition.x);
        }

        private void UpdatePositionWithXCurve(Vector3 currentPosition, Vector3 distance, Vector3 targetPosition,
            out bool isComplete)
        {
            Vector3 nextPosition = currentPosition;
            nextPosition.y = currentPosition.y + CurrentMoveSpeed * Time.deltaTime * Mathf.Sign(distance.y);
            float nextPositionYNormalized = Mathf.Abs((nextPosition.y - StartPosition.y) / distance.y);
            float nextPositionXAxis = Axis.Evaluate(nextPositionYNormalized) * distance.x;
            float trajectorySign = distance.y * distance.x > 0 ? -1 : 1;
            float trajectoryValue = Trajectory.Evaluate(nextPositionYNormalized) * TrajectoryMaxHeight *
                                    Mathf.Abs(distance.y) * trajectorySign;
            nextPosition.x = StartPosition.x + trajectoryValue + nextPositionXAxis;

            Vector3 direction = nextPosition - currentPosition;
            Transform.position = nextPosition;
            Transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            Transform.localScale = Vector3.one * Scale.Evaluate(nextPositionYNormalized);

            ShadowTransform.position = Vector3.Lerp(StartPosition, targetPosition, nextPositionYNormalized);
            ShadowTransform.rotation = Quaternion.FromToRotation(Vector3.up, distance);

            CurrentMoveSpeed = Velocity.Evaluate(nextPositionYNormalized) * MaxMoveSpeed;
            isComplete = Mathf.Abs(nextPosition.y - StartPosition.y) > Mathf.Abs(targetPosition.y - StartPosition.y);
        }
        
    }
}