using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TW.Utility.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class Projectile : ACachedMonoBehaviour
{
    [field: SerializeField] public Vector3 CurrentPosition { get; private set; }
    [field: SerializeField] public Enemy TargetEnemy { get; private set; }
    [field: SerializeField] public int AttackDamage { get; private set; }
    [field: SerializeField] private float MoveSpeed { get; set; }
    [field: SerializeField] public float CurrentTime { get; private set; }
    private Vector3 LastPosition { get; set; }

    public void Init(Enemy targetEnemy, int attackDamage)
    {
        CurrentPosition = Transform.position;
        TargetEnemy = targetEnemy;
        AttackDamage = attackDamage;
        CurrentTime = 0;
        StartMoveToTarget().Forget();
    }

    private async UniTask StartMoveToTarget()
    {
        float distance = Vector3.Distance(CurrentPosition, TargetEnemy.transform.position);
        float totalTime = distance / MoveSpeed;
        
        await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                           .WithCancellation(this.GetCancellationTokenOnDestroy()))
        {
            if (TargetEnemy == null)
            {
                SelfDespawn();
                break;
            }

            LastPosition = Transform.position;
            
            
            CurrentTime += Time.deltaTime * MoveSpeed;

            
            Transform.position = PointOnTime(CurrentPosition, TargetEnemy.transform.position, CurrentTime, totalTime, 0.25f);
            Vector3 direction = Transform.position - LastPosition;
            Transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            
            if (CurrentTime >= totalTime)
            {
                TargetEnemy.TakeDamage(AttackDamage);
                SelfDespawn();
                break;
            }
        }
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
        float y = alpha < 0.5f ? 
            Mathf.Lerp(start.y, height, EaseOutQuad(alpha * 2)) : 
            Mathf.Lerp(height, end.y, EaseInQuad((alpha - 0.5f) * 2));
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