﻿using System;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TW.Utility.CustomComponent;
using UnityEngine;

namespace Core
{
    public class MoveProjectile : ACachedMonoBehaviour, IPoolAble<MoveProjectile>
    {
        private delegate void UpdatePosition(Vector3 currentPosition, Vector3 distance, Vector3 targetPosition,
            out bool isComplete);

        [field: SerializeField] public SpriteRenderer Graphic { get; private set; }
        [field: SerializeField] public SpriteRenderer Shadow { get; private set; }

        [field: SerializeField] public Transform ShadowTransform { get; private set; }
        [field: SerializeField] public Vector3 CurrentPosition { get; private set; }
        [field: SerializeField] private float MaxMoveSpeed { get; set; }
        [field: SerializeField] public float TrajectoryMaxHeight { get; private set; }
        [field: SerializeField] public AnimationCurve Trajectory { get; private set; }
        [field: SerializeField] public AnimationCurve Axis { get; private set; }
        [field: SerializeField] public AnimationCurve Velocity { get; private set; }
        [field: SerializeField] public AnimationCurve Scale { get; private set; }
        private Vector3 StartPosition { get; set; }
        private Vector3 EndPosition { get; set; }
        private float CurrentMoveSpeed { get; set; }
        private UpdatePosition UpdatePositionCallback { get; set; }
        private Action OnMoveComplete { get; set; }
        
        public MoveProjectile OnSpawn()
        {
            return this;
        }

        public void OnDespawn()
        {

        }

        public MoveProjectile Init(Vector3 startPosition, Vector3 endPosition, Action onMoveComplete)
        {
            Transform.position = startPosition;
            Transform.localScale = Vector3.zero;
            OnMoveComplete = onMoveComplete;
            
            CurrentPosition = StartPosition;
            StartPosition = startPosition;
            CurrentMoveSpeed = MaxMoveSpeed;
            EndPosition = endPosition;
            StartMoveToTarget().Forget();

            return this;
        }

        private async UniTask StartMoveToTarget()
        {
            UpdatePositionCallback = SetupUpdatePositionCallback();
            await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                               .WithCancellation(this.GetCancellationTokenOnDestroy()))
            {
                if (TryMoveToTarget()) continue;
                SelfDespawn();
                break;
            }
        }

        private UpdatePosition SetupUpdatePositionCallback()
        {
            Vector3 distance = EndPosition - StartPosition;
            if (distance.sqrMagnitude < 0.01f) return UpdatePositionInCurrentPosition;
            Vector3 distanceNormalized = distance.normalized;
            return Mathf.Abs(distanceNormalized.x) < Mathf.Abs(distanceNormalized.y)
                ? UpdatePositionWithXCurve
                : UpdatePositionWithYCurve;
        }
        


        private bool TryMoveToTarget()
        {
            Vector3 currentPosition = Transform.position;
            Vector3 targetPosition = EndPosition;
            Vector3 distance = targetPosition - StartPosition;
            UpdatePositionCallback(currentPosition, distance, targetPosition, out bool isComplete);
            return !isComplete;
        }

        private void UpdatePositionInCurrentPosition(Vector3 currentPosition, Vector3 distance, Vector3 targetPosition,
            out bool isComplete)
        {
            Transform.position = targetPosition;
            isComplete = true;
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

        private void SelfDespawn()
        {
            OnMoveComplete?.Invoke();
            Destroy(gameObject);
        }


    }
}