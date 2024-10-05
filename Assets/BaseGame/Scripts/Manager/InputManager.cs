using System;
using Core;
using R3;
using R3.Triggers;
using TW.Utility.DesignPattern;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public FieldSlot StartDragFieldSlot { get; private set; }
    [field: SerializeField] public FieldSlot EndDragFieldSlot { get; private set; }
    [field: SerializeField] public Transform StartSelect {get; private set;}
    [field: SerializeField] public Transform EndSelect {get; private set;}
    [field: SerializeField] public LineRenderer LineRenderer {get; private set;}

#if UNITY_EDITOR
    private void Start()
    {
        this.UpdateAsObservable().Where(_ =>Input.GetKey(KeyCode.Space)).Subscribe(_ =>
        {
            Debug.Break();
        });
    }
#endif

    public void SetStartDragFieldSlot(FieldSlot fieldSlot)
    {
        StartDragFieldSlot = fieldSlot;
        if (fieldSlot == null)
        {
            StartSelect.gameObject.SetActive(false);
            DrawLine();
            return;
        }
        StartSelect.position = StartDragFieldSlot.Transform.position;
        StartSelect.gameObject.SetActive(true);
    }
    public void SetEndDragFieldSlot(FieldSlot fieldSlot)
    {
        EndDragFieldSlot = fieldSlot;
        if (fieldSlot == null)
        {
            EndSelect.gameObject.SetActive(false);
            DrawLine();
            return;
        }
        EndSelect.position = EndDragFieldSlot.Transform.position;
        EndSelect.gameObject.SetActive(true);
        DrawLine();
    }
    private void DrawLine()
    {
        if (StartDragFieldSlot == null || EndDragFieldSlot == null)
        {
            LineRenderer.positionCount = 0;
            return;
        }
        LineRenderer.positionCount = 2;
        LineRenderer.SetPosition(0, StartDragFieldSlot.Transform.position);
        LineRenderer.SetPosition(1, EndDragFieldSlot.Transform.position);
    }

    public void TrySwapHeroInFieldSlot()
    {
        if (StartDragFieldSlot == null || EndDragFieldSlot == null) return;
        if (StartDragFieldSlot == EndDragFieldSlot) return;

        StartDragFieldSlot.TryRemoveHero(out Hero startHero);
        EndDragFieldSlot.TryRemoveHero(out Hero endHero);

        if (startHero != null && startHero.IsCurrentState(HeroAttackState.Instance))
        {
            EndDragFieldSlot.TryAddHero(startHero);
        }
        if (endHero != null && endHero.IsCurrentState(HeroAttackState.Instance))
        {
            StartDragFieldSlot.TryAddHero(endHero);
        }
    }
}