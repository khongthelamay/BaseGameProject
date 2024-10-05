using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/MapData")]
public class MapData : ScriptableObject
{
    [field: SerializeField, TableList] public List<WaveData> WaveDataList { get; private set; }

    [Button]
    public void GenerateSample()
    {
#if UNITY_EDITOR
        WaveDataList.Clear();
        EditorUtility.SetDirty(this);
        for (int i = 0; i < 40; i++)
        {
            WaveDataList.Add(new WaveData
            {
                WaveId = i + 1,
                EnemyId = 0,
                EnemyCount = 40,
                SpawnInterval = 0.5f,
                WaveInterval = 5f
            });
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
}

[Serializable]
public class WaveData
{
    [field: SerializeField] public int WaveId { get; set; }
    [field: SerializeField] public int EnemyId { get; set; }
    [field: SerializeField] public int EnemyCount { get; set; }
    [field: SerializeField] public float SpawnInterval { get; set; }
    [field: SerializeField] public float WaveInterval { get; set; }
}