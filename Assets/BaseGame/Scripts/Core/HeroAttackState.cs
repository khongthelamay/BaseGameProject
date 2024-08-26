using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

public class HeroAttackState : SingletonState<Hero, HeroAttackState>
{
        
}

public partial class Hero : HeroAttackState.IHandler
{
    [field: SerializeField] private float CurrentAttackTimer { get; set; }
    [field: SerializeField] private float CurrentAttackTimeCounter { get; set; }
    public UniTask OnStateEnter(HeroAttackState state, CancellationToken token)
    {

        return UniTask.CompletedTask;
    }

    public UniTask OnStateExecute(HeroAttackState state, CancellationToken token)
    {
        CurrentAttackTimer = 1f/HeroStatData.BaseAttackSpeed;
        if (CurrentAttackTimeCounter > CurrentAttackTimer)
        {
            if (TryAttackEnemy())
            {
                CurrentAttackTimeCounter -= CurrentAttackTimer;
            }
        }
        else
        {
            CurrentAttackTimeCounter += Time.deltaTime;
        }
        return UniTask.CompletedTask;
    }

    public UniTask OnStateExit(HeroAttackState state, CancellationToken token)
    {
        return UniTask.CompletedTask;
    }
    public bool TryAttackEnemy()
    {
        if (!TryFindEnemy(out Enemy closeEnemy)) return false;
        Projectile projectile = Instantiate(BaseProjectile, Transform.position, Quaternion.identity);
        projectile.Init(closeEnemy, HeroStatData.BaseAttackDamage);
        return true;
    }
    public bool TryFindEnemy(out Enemy closeEnemy)
    {
        foreach (Enemy enemy in FieldManager.Instance.EnemyList)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < HeroStatData.BaseAttackRange)
            {
                closeEnemy = enemy;
                return true;
            }
        }
        closeEnemy = null;
        return false;
    }
}