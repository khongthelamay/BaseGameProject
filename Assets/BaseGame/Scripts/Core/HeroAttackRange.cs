﻿using LitMotion;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using TW.Utility.Extension;
using UnityEngine;

public class HeroAttackRange : ACachedMonoBehaviour
{
    [field: SerializeField] private float AttackRange {get; set;}
    private MotionHandle UpdateAttackMotion {get; set;}
    private float OriginalScale {get; set;} = 0;
    // public void InitAttackRange(float attackRange)
    // {
    //     BaseAttackRange = attackRange;
    //     OriginalScale = 0;
    //     Transform.localScale = Vector3.zero;
    // }

    private void OnDestroy()
    {
        UpdateAttackMotion.TryCancel();
    }

    [Button]
    public void ShowAttackRange(float range)
    {
        UpdateAttackMotion.TryCancel();
        UpdateAttackMotion = LMotion.Create(OriginalScale, range * 2, 0.2f)
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
    public void ResetAttackRange()
    {
        UpdateAttackMotion.TryCancel();
        OriginalScale = 0;
        Transform.localScale = Vector3.one * OriginalScale;
    }
    private void UpdateAttackRange(float value)
    {
        OriginalScale = value;
        Transform.localScale = Vector3.one * value;
    }

}