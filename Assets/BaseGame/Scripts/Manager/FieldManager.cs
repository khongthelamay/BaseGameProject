using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Manager;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEngine;


public class FieldManager : Singleton<FieldManager>
{
    [field: SerializeField] public WaitSlot[] WaitSlotArray {get; private set;}
    
    private void Start()
    {
        RandomWaitSlot();
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.C))
            .Subscribe(_ => RandomWaitSlot());
    }

    
    [Button]
    public void RandomWaitSlot()
    {
        foreach (var waitSlot in WaitSlotArray)
        {
            waitSlot.RandomOwnerHero();
        }
    }
    
    
}
