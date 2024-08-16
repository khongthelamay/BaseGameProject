using LitMotion;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEngine;

public class HeroAttackRange : ACachedMonoBehaviour
{
    [field: SerializeField] private float AttackRange {get; set;}
    private MotionHandle UpdateAttackMotion {get; set;}
    private float OriginalScale {get; set;} = 0;
    public void InitAttackRange(float attackRange)
    {
        AttackRange = attackRange;
        OriginalScale = 0;
        Transform.localScale = Vector3.zero;
    }
    [Button]
    public void ShowAttackRange()
    {
        UpdateAttackMotion.TryCancel();
        UpdateAttackMotion = LMotion.Create(OriginalScale, 1 + AttackRange * 2, 0.2f)
            .WithEase(Ease.OutBack)
            .Bind(UpdateAttackRange);
    }
    [Button]
    public void HideAttackRange()
    {
        UpdateAttackMotion.TryCancel();
        UpdateAttackMotion = LMotion.Create(OriginalScale, 0, 0.2f)
            .WithEase(Ease.InBack)
            .Bind(UpdateAttackRange);
    }
    private void UpdateAttackRange(float value)
    {
        OriginalScale = value;
        Transform.localScale = Vector3.one * value;
    }
}