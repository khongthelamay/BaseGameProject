using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySlot : SlotBase<Ability>
{
    [Header("==== Ability Slot ====")]
    [SerializeField] GameObject objLock;
    public override void InitData(Ability data)
    {
        base.InitData(data);
    }
}
