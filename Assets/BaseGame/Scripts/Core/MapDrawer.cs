using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using System.Linq;
#endif


[System.Serializable]
public class MapDrawer : MonoBehaviour
{

    [field: SerializeField] public Vector2 DefaultSize { get; private set; }
    [field: SerializeField] public MapPositionDraw MapPositionDraw {get; private set;}


    [field: SerializeField] public List<MapDrawRule> TileList { get; private set; }
    [field: SerializeField] public List<MapDrawRule> GroundList { get; private set; }
    
    
}
[System.Serializable]
[MapPositionDrawEditor]
public class MapPositionDraw
{
    [field: SerializeField] public int Row { get; set; }
    [field: SerializeField] public int Column { get; set; }
    [field: SerializeField] public List<int> MapPosition {get; set;}
}
[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public sealed class MapPositionDrawEditor : System.Attribute
{
    
}
#if UNITY_EDITOR
public sealed class MapPositionDrawEditorAttributeDrawer : OdinAttributeDrawer<MapPositionDrawEditor, MapPositionDraw>
{
    private enum Tile
    {
        HasTile = 0,
        HasGround = 1,
    }

    private Color[] TileColors { get; } = new Color[2]
    {
        new Color(0.3f, 1.0f, 0.3f), // 1
        new Color(1.0f, 1.0f, 0.0f), // 2
    };

    private int TileSize { get; set; } 
    private int Row { get; set; } 
    private int Col { get; set; } 

    private bool IsDrawing { get; set; } 

    private Tile[,] Tiles { get; set; } 
    private Color OldColor { get; set; } 
    private List<int> MapPosition { get; set; } 
    private bool IsMouseDown { get; set; }
    private Vector2Int LastTilePosition { get; set; }
    private int MouseIndex { get; set; }
    protected override void Initialize()
    {
        TileSize = 40;


        Tiles = new Tile[Row, Col];

        IsDrawing = false;

        OldColor = GUI.backgroundColor;
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        Rect rect;
        if (!IsDrawing)
        {
            ValueEntry.SmartValue.Row = SirenixEditorFields.IntField("Row", ValueEntry.SmartValue.Row);
            ValueEntry.SmartValue.Column = SirenixEditorFields.IntField("Column", ValueEntry.SmartValue.Column);
            
            GUI.backgroundColor = Color.green;
            rect = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect, "Load Map Position"))
            {
                LoadMapPosition();
                ValueEntry.WeakValues.ForceMarkDirty();
            }

            GUI.backgroundColor = OldColor;
        }
        else
        {
            GUI.backgroundColor = Color.blue;
            rect = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect, "Save Map Position"))
            {
                SaveMapPosition();
                ValueEntry.WeakValues.ForceMarkDirty();
            }
            GUI.backgroundColor = Color.red;
            rect = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect, "Clear Map Position"))
            {
                ClearMapPosition();
                ValueEntry.WeakValues.ForceMarkDirty();
            }
            GUI.backgroundColor = OldColor;

        }
        

        OnDrawMap();
    }

    private void LoadMapPosition()
    {
        Row = ValueEntry.SmartValue.Row;
        Col = ValueEntry.SmartValue.Column;
        ValueEntry.SmartValue.MapPosition ??= new List<int>();
        while (ValueEntry.SmartValue.MapPosition.Count < Row * Col)
        {
            ValueEntry.SmartValue.MapPosition.Add(0);
        }
        Tiles = new Tile[ValueEntry.SmartValue.Row, ValueEntry.SmartValue.Column];
        MapPosition = ValueEntry.SmartValue.MapPosition;
        for (int i = 0; i < ValueEntry.SmartValue.Row; i++)
        {
            for (int j = 0; j < ValueEntry.SmartValue.Column; j++)
            {
                Tiles[i, j] = (Tile)MapPosition[i * ValueEntry.SmartValue.Column + j];
            }
        }
        
        IsDrawing = true;
    }

    private void SaveMapPosition()
    {
        ValueEntry.SmartValue.MapPosition = MapPosition;
        IsDrawing = false;
    }
    private void ClearMapPosition()
    {
        ValueEntry.SmartValue.MapPosition = new List<int>();
        LoadMapPosition();
    }

    private void OnDrawMap()
    {
        if (!IsDrawing) return;
    
        Rect rect = EditorGUILayout.GetControlRect(false, TileSize * Row);
        rect = rect.AlignCenter(TileSize * Col);
        rect = rect.AlignMiddle(TileSize * Row);
        if (Event.current.type == EventType.MouseDown)
        {
            MouseIndex = Event.current.button;
            IsMouseDown = true;
            LastTilePosition = new Vector2Int(-1,-1);
        }

        if (Event.current.type == EventType.MouseUp)
        {
            MouseIndex = Event.current.button;
            IsMouseDown = false;
            LastTilePosition = new Vector2Int(-1,-1);;
        }
        
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
            {
                Rect tileRect = rect.SplitGrid(TileSize, TileSize, i * Col + j);
                SirenixEditorGUI.DrawBorders(tileRect.SetWidth(tileRect.width + 1).SetHeight(tileRect.height + 1), 1);
    
                SirenixEditorGUI.DrawSolidRect(new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1),
                    TileColors[(int)Tiles[i, j]]);
    
    
                if (tileRect.Contains(Event.current.mousePosition))
                {
                    SirenixEditorGUI.DrawSolidRect(
                        new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1),
                        new Color(0f, 1f, 1f, 0.3f));

                    if (IsMouseDown && LastTilePosition != new Vector2Int(i, j))
                    {
                        if (Tiles[i, j] == Tile.HasTile && MouseIndex == 0)
                        {
                            Tiles[i, j] = Tile.HasGround;
                            MapPosition[i * Col + j] = (int)Tiles[i, j];
                        }
                        
                        if (Tiles[i, j] == Tile.HasGround && MouseIndex == 1)
                        {
                            Tiles[i, j] = Tile.HasTile;
                            MapPosition[i * Col + j] = (int)Tiles[i, j];
                        }
                        LastTilePosition = new Vector2Int(i, j);
                    }
                }
            }
        }
    
        GUIHelper.RequestRepaint();
    }
}
#endif

[System.Serializable]
[MapDrawRuleEditor]
public class MapDrawRule
{
    [field: SerializeField] public List<int> BaseRulePosition { get; set; }
    [field: SerializeField] public GameObject MapObject { get; set; }

    public MapDrawRule()
    {
        BaseRulePosition = new List<int>();
        MapObject = null;
    }

    public int[,] GetBaseRulePosition()
    {
        int[,] res = new int[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                res[i, j] = BaseRulePosition[i * 3 + j];
            }
        }

        return res;
    }
}

[System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public sealed class MapDrawRuleEditor : System.Attribute
{
    
}

#if UNITY_EDITOR
public sealed class MapDrawRuleEditorAttributeDrawer : OdinAttributeDrawer<MapDrawRuleEditor, MapDrawRule>
{
    private enum Tile
    {
        HasOrNotHasBlock = 0,
        NeedHasBlock = 1,
        DontHasBlock = 2,
        Middle = 3,
    }

    private Color[] TileColors { get; } = new Color[4]
    {
        new Color(0.3f, 0.3f, 0.3f), // 0
        new Color(0.3f, 1.0f, 0.3f), // 1
        new Color(1.0f, 0.3f, 0.3f), // 2
        new Color(1.0f, 1.0f, 0.0f), // 3
    };

    private int TileSize { get; set; } 
    private int Row { get; set; } 
    private int Col { get; set; } 

    private bool IsDrawing { get; set; } 

    private Tile[,] Tiles { get; set; } 
    private Color OldColor { get; set; } 
    private List<int> BaseRulePosition { get; set; } 

    protected override void Initialize()
    {
        TileSize = 40;
        Row = 3;
        Col = 3;

        Tiles = new Tile[Row, Col];

        IsDrawing = false;

        OldColor = GUI.backgroundColor;
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        Rect rect = EditorGUILayout.GetControlRect();
        ValueEntry.SmartValue.MapObject = SirenixEditorFields.UnityObjectField(rect.AlignLeft(rect.width - 100 - 4),
            ValueEntry.SmartValue.MapObject, typeof(GameObject), true) as GameObject;

        if (!IsDrawing)
        {
            GUI.backgroundColor = Color.green;
            if (GUI.Button(rect.AlignRight(100), "Edit Pos ID"))
            {
                LoadCurrentBaseRule();
                ValueEntry.WeakValues.ForceMarkDirty();
            }

            GUI.backgroundColor = OldColor;
        }
        else
        {
            GUI.backgroundColor = Color.blue;
            if (GUI.Button(rect.AlignRight(100), "Save Pos ID"))
            {
                SaveCurrentBaseRule();
                ValueEntry.WeakValues.ForceMarkDirty();
            }

            GUI.backgroundColor = OldColor;
        }

        OnDrawMap();
    }

    public void LoadCurrentBaseRule()
    {
        if (ValueEntry.SmartValue.BaseRulePosition == null ||
            ValueEntry.SmartValue.BaseRulePosition.Count < 9)
        {
            ValueEntry.SmartValue.BaseRulePosition = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        BaseRulePosition = ValueEntry.SmartValue.BaseRulePosition;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i == 1 && j == 1)
                {
                    Tiles[i, j] = Tile.Middle;
                }
                else
                {
                    Tiles[i, j] = (Tile)BaseRulePosition[i * 3 + j];
                }
            }
        }

        IsDrawing = true;
    }

    public void SaveCurrentBaseRule()
    {
        ValueEntry.SmartValue.BaseRulePosition = BaseRulePosition;
        IsDrawing = false;
    }

    public void OnDrawMap()
    {
        if (!IsDrawing) return;

        Rect rect = EditorGUILayout.GetControlRect(false, TileSize * Row);
        Rect tempRect = rect;
        rect = rect.AlignLeft(rect.width / 2);
        rect = rect.AlignCenter(TileSize * Col);
        rect = rect.AlignMiddle(TileSize * Row);
        SirenixEditorFields.PreviewObjectField(rect, ValueEntry.SmartValue.MapObject, true, false, false, true);

        rect = tempRect;
        rect = rect.AlignRight(rect.width / 2);
        rect = rect.AlignCenter(TileSize * Col);
        rect = rect.AlignMiddle(TileSize * Row);

        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
            {
                Rect tileRect = rect.SplitGrid(TileSize, TileSize, i * Col + j);
                SirenixEditorGUI.DrawBorders(tileRect.SetWidth(tileRect.width + 1).SetHeight(tileRect.height + 1), 1);

                SirenixEditorGUI.DrawSolidRect(
                    new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1),
                    TileColors[(int)Tiles[i, j]]);


                if (tileRect.Contains(Event.current.mousePosition))
                {
                    SirenixEditorGUI.DrawSolidRect(
                        new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1),
                        new Color(0f, 1f, 0f, 0.3f));

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        if (Tiles[i, j] == Tile.DontHasBlock)
                        {
                            Tiles[i, j] = Tile.HasOrNotHasBlock;
                            BaseRulePosition[i * 3 + j] = (int)Tiles[i, j];
                        }
                        else if (Tiles[i, j] == Tile.NeedHasBlock)
                        {
                            Tiles[i, j] = Tile.DontHasBlock;
                            BaseRulePosition[i * 3 + j] = (int)Tiles[i, j];
                        }
                        else if (Tiles[i, j] == Tile.HasOrNotHasBlock)
                        {
                            Tiles[i, j] = Tile.NeedHasBlock;
                            BaseRulePosition[i * 3 + j] = (int)Tiles[i, j];
                        }

                        Event.current.Use();
                    }
                }
            }
        }

        GUIHelper.RequestRepaint();
    }
}
#endif