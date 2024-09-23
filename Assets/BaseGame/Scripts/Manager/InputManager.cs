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
}