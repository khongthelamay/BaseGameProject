using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
    [field: SerializeField] public float AreaRadius {get; private set;}
    [field: SerializeField] public float Damage {get; private set;}
    [field: SerializeField] public float DamageInterval {get; private set;}
    [field: SerializeField] public float Duration {get; private set;}
    
    public void Setup(float areaRadius, float damage, float damageInterval, float duration)
    {
        AreaRadius = areaRadius;
        Damage = damage;
        DamageInterval = damageInterval;
        Duration = duration;
    }
    public void StartDamage()
    {
        StartCoroutine(DamageCoroutine());
    }
    
    private IEnumerator DamageCoroutine()
    {
        // float time = 0;
        // while (time < Duration)
        // {
        //     Collider[] colliders = Physics.OverlapSphere(transform.position, AreaRadius);
        //     foreach (var collider in colliders)
        //     {
        //         if (collider.TryGetComponent(out Enemy enemy))
        //         {
        //             enemy.TakeDamage(Damage);
        //         }
        //     }
        //     time += DamageInterval;
        //     yield return new WaitForSeconds(DamageInterval);
        // }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AreaRadius);
    }
}
