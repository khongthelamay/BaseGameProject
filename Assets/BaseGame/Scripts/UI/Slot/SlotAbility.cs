using Core;
using UnityEngine;

public class SlotAbility : SlotBase<Ability>
{
    [Header("==== Ability Slot ====")]
    [SerializeField] GameObject objLock;
    public override void InitData(Ability data)
    {
        base.InitData(data);
    }
}
