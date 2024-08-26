using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TW.Utility.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class Projectile : ACachedMonoBehaviour
{
    [field: SerializeField] public Vector3 CurrentPosition {get; private set;}
    [field: SerializeField] public Enemy TargetEnemy {get; private set;}
    [field: SerializeField] public int AttackDamage {get; private set;}
    [field: SerializeField] private float MoveSpeed {get; set;} 
    [field: SerializeField] public float CurrentTime {get; private set;}
    [field: SerializeField] public AnimationCurve XCurve {get; private set;}
    [field: SerializeField] public AnimationCurve YCurve {get; private set;}

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
        await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(this.GetCancellationTokenOnDestroy()))
        {
            if (TargetEnemy == null)
            {
                SelfDespawn();
                break;
            }
            CurrentTime += Time.deltaTime;
            float distance = Vector3.Distance(CurrentPosition, TargetEnemy.transform.position);
            float fullTime = distance / MoveSpeed;
            float alpha = Mathf.Clamp(CurrentTime/fullTime, 0, 1);
            float xPos = LerpWithoutClamp(CurrentPosition.x, TargetEnemy.transform.position.x, XCurve.Evaluate(alpha));
            float yPos = LerpWithoutClamp(CurrentPosition.y, TargetEnemy.transform.position.y, YCurve.Evaluate(alpha));
            
            Transform.position = new Vector3(xPos, yPos, 0);
            if (CurrentTime >= fullTime)
            {
                TargetEnemy.TakeDamage(AttackDamage);
                SelfDespawn();
                break;
            }
        }
    }
    float LerpWithoutClamp(float a, float b, float t)
    {
        return a + (b-a)*t;
    }
    private void SelfDespawn()
    {
        Destroy(gameObject);
    }
}