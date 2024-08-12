using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TW.Utility.CustomComponent;
using UnityEngine;

public class FieldSlot : ACachedMonoBehaviour
{
    [field: SerializeField] public Vector2 DefaultSize {get; private set;}

    [field: SerializeField] public int RowId {get; private set;}
    [field: SerializeField] public int ColumnId {get; private set;}
    
    public FieldSlot SetupCoordinate(int rowId, int columnId)
    {
        RowId = rowId;
        ColumnId = columnId;

        return this;
    }
    public FieldSlot SetupTransform(Transform parent)
    {
        transform.SetParent(parent);
        return this;
    }
    public FieldSlot SetupPosition(int maxRow, int maxColumn)
    {
        float x = (ColumnId - maxColumn / 2f + 0.5f) * DefaultSize.x;
        float y = (RowId - maxRow / 2f + 0.5f) * DefaultSize.y;
        transform.localPosition = new Vector3(x, y, 0);
        Tween a = Transform.DOMove(Vector3.zero, 1);
        a.timeScale = 0.5f;
        
        return this;
    }
}