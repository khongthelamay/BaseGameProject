using Core;
using R3;
using R3.Triggers;
using TW.Utility.DesignPattern;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public FieldSlot FieldSlotSelect { get; private set; }

    [field: SerializeField] public FieldSlot StartDragFieldSlot { get; private set; }
    [field: SerializeField] public FieldSlot EndDragFieldSlot { get; private set; }
    [field: SerializeField] public Transform StartSelect { get; private set; }
    [field: SerializeField] public Transform EndSelect { get; private set; }
    [field: SerializeField] public Transform ArrowDirection { get; private set; }
    [field: SerializeField] public LineRenderer LineRenderer { get; private set; }


#if UNITY_EDITOR
    private void Start()
    {
        this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.Space)).Subscribe(_ => { Debug.Break(); });
    }
#endif
    public void ShowFieldSlotInteract(FieldSlot fieldSlot)
    {
        FieldSlotSelect = fieldSlot;
        ActivityHeroInfoContext.Events.ShowFieldSlotInteract?.Invoke(fieldSlot);
    }

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
            ArrowDirection.gameObject.SetActive(false);
            return;
        }

        ArrowDirection.gameObject.SetActive(StartDragFieldSlot != EndDragFieldSlot);
        Vector3 arrowDirection = EndDragFieldSlot.Transform.position - StartDragFieldSlot.Transform.position;
        if (arrowDirection != Vector3.zero)
        {
            ArrowDirection.rotation = Quaternion.LookRotation(arrowDirection, Vector3.forward) * Quaternion.Euler(90, 90, 90);
        }
        LineRenderer.positionCount = 2;
        LineRenderer.SetPosition(0, StartDragFieldSlot.Transform.position);
        LineRenderer.SetPosition(1, EndDragFieldSlot.Transform.position);
    }

    public void TrySwapHeroInFieldSlot()
    {
        if (StartDragFieldSlot == null || EndDragFieldSlot == null) return;
        if (StartDragFieldSlot == EndDragFieldSlot) return;
        if (FieldSlotSelect != null && (StartDragFieldSlot == FieldSlotSelect || EndDragFieldSlot == FieldSlotSelect))
        {
            ShowFieldSlotInteract(null);
        }

        StartDragFieldSlot.TryRemoveHero(out Hero startHero);
        EndDragFieldSlot.TryRemoveHero(out Hero endHero);

        if (startHero != null && startHero.IsInBattleState())
        {
            EndDragFieldSlot.TryAddHero(startHero);
        }

        if (endHero != null && startHero.IsInBattleState())
        {
            StartDragFieldSlot.TryAddHero(endHero);
        }
    }
}