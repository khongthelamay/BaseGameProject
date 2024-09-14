using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TW.Utility.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class Projectile : ACachedMonoBehaviour
{
    [field: SerializeField] public SpriteRenderer Graphic { get; private set; }
    [field: SerializeField] public SpriteRenderer Shadow { get; private set; }

    [field: SerializeField] public Transform ShadowTransform { get; private set; }
    [field: SerializeField] public Vector3 StartPosition { get; private set; }
    [field: SerializeField] public Vector3 CurrentPosition { get; private set; }
    [field: SerializeField] public Enemy TargetEnemy { get; private set; }
    [field: SerializeField] public int AttackDamage { get; private set; }
    [field: SerializeField] private float MaxMoveSpeed { get; set; }
    [field: SerializeField] public float TrajectoryMaxHeight { get; private set; }
    [field: SerializeField] public AnimationCurve Trajectory { get; private set; }
    [field: SerializeField] public AnimationCurve Axis { get; private set; }
    [field: SerializeField] public AnimationCurve Velocity { get; private set; }
    [field: SerializeField] public AnimationCurve Scale {get; private set;}
    private Vector3 End { get; set; }
    private float CurrentMoveSpeed { get; set; }
    
    public Projectile Init(Enemy targetEnemy, int attackDamage)
    {
        Transform.localScale = Vector3.zero;
        StartPosition = Transform.position;
        CurrentPosition = StartPosition;
        TargetEnemy = targetEnemy;
        AttackDamage = attackDamage;
        CurrentMoveSpeed = MaxMoveSpeed;
        End = TargetEnemy.Transform.position;
        StartMoveToTarget().Forget();

        return this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(StartPosition, End);
        Debug.Log(Vector3.Distance(StartPosition, End));
    }

    private async UniTask StartMoveToTarget()
    {
        await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                           .WithCancellation(this.GetCancellationTokenOnDestroy()))
        {
            if (TargetEnemy == null)
            {
                SelfDespawn();
                break;
            }

            Vector3 currentPosition = Transform.position;
            Vector3 targetPosition = TargetEnemy.Transform.position;
            Vector3 distance = targetPosition - StartPosition;
            Vector3 distanceNormalized = distance.normalized;
            bool isComplete = true;
            if (Mathf.Abs(distanceNormalized.x) < Mathf.Abs(distanceNormalized.y))
            {
                UpdatePositionWithXCurve(currentPosition, distance, targetPosition, out isComplete);
            }
            else
            {
                UpdatePositionWithYCurve(currentPosition, distance, targetPosition, out isComplete);
            }


            if (!isComplete) continue;
            TargetEnemy.TakeDamage(AttackDamage);
            SelfDespawn();
            break;
        }
    }

    private void UpdatePositionWithYCurve(Vector3 currentPosition, Vector3 distance, Vector3 targetPosition, out bool isComplete)
    {
        Vector3 nextPosition = currentPosition;
        nextPosition.x = currentPosition.x + CurrentMoveSpeed * Time.deltaTime * Mathf.Sign(distance.x);
        float nextPositionXNormalized = Mathf.Abs((nextPosition.x - StartPosition.x) / distance.x);
        float nextPositionYAxis = Axis.Evaluate(nextPositionXNormalized) * distance.y;
        float trajectoryValue = Trajectory.Evaluate(nextPositionXNormalized) * TrajectoryMaxHeight * Mathf.Abs(distance.x);

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

    private void UpdatePositionWithXCurve(Vector3 currentPosition, Vector3 distance, Vector3 targetPosition, out bool isComplete)
    {
        Vector3 nextPosition = currentPosition;
        nextPosition.y = currentPosition.y + CurrentMoveSpeed * Time.deltaTime * Mathf.Sign(distance.y);
        float nextPositionYNormalized = Mathf.Abs((nextPosition.y - StartPosition.y) / distance.y);
        float nextPositionXAxis = Axis.Evaluate(nextPositionYNormalized) * distance.x;
        float trajectorySign = distance.y * distance.x > 0 ? -1 : 1;
        float trajectoryValue = Trajectory.Evaluate(nextPositionYNormalized) * TrajectoryMaxHeight * Mathf.Abs(distance.y) * trajectorySign;
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
        Destroy(gameObject);
    }

    private Vector2 PointOnTime(Vector2 start, Vector2 end, float time, float totalTime, float jumpHeight)
    {
        float alpha = Mathf.Clamp(time / totalTime, 0, 1);

        float x = Mathf.Lerp(start.x, end.x, EaseInOutSine(alpha));
        float height = Mathf.Max(start.y, end.y) + jumpHeight;
        float y = alpha < 0.5f
            ? Mathf.Lerp(start.y, height, EaseOutQuad(alpha * 2))
            : Mathf.Lerp(height, end.y, EaseInQuad((alpha - 0.5f) * 2));
        return new Vector2(x, y);
    }

    private float EaseOutQuad(float x)
    {
        return 1f - (1f - x) * (1f - x);
    }

    private float EaseInQuad(float x)
    {
        return x * x;
    }

    private float EaseLinear(float x)
    {
        return x;
    }

    private float EaseInOutSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
    }
}