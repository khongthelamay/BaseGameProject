using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class AxisDetection<T> 
{
    [field: SerializeField] public bool UsingAxis { get; private set; }

    [field: SerializeField, ShowIf("UsingAxis"), InfoBox("-1: Vertical, 1: Horizontal")]
    public int Axis { get; set; }

    [field: SerializeField, ShowIf("UsingAxis")]
    public T Horizontal { get; set; }

    [field: SerializeField, ShowIf("UsingAxis")]
    public T Vertical { get; set; }

    [field: SerializeField, HideIf("UsingAxis")]
    public T NoneAxis { get; set; }
    public T Current => UsingAxis ? Axis == 1 ? Horizontal : Vertical : NoneAxis;
        
}