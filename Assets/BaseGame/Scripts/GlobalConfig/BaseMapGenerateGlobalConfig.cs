using System.Collections.Generic;
using System.Linq;
using Core;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseMapGenerateGlobalConfig", menuName = "GlobalConfigs/BaseMapGenerateGlobalConfig")]
public class BaseMapGenerateGlobalConfig : ScriptableObject
{
    [field: SerializeField] public string MapStyle {get; private set;}
    [field: SerializeField] public GameObject BaseMapObject {get; private set;}
    [field: SerializeField] public List<Sprite> TileSprites {get; private set;}
    [field: SerializeField] public List<Sprite> GroundSprites {get; private set;}

    [field: SerializeField] public List<GameObject> TilePrefabList {get; private set;}
    [field: SerializeField] public List<GameObject> GroundPrefabList {get; private set;}
    
#if UNITY_EDITOR
    [Button]
    private void GenerateAllPrefabMap()
    {
        EditorUtility.SetDirty(this);
        
        GenerateTile();
        GenerateGround();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        HeroPoolGlobalConfig.Instance.GetAllHeroPrefab();
    }

    private void GenerateTile()
    {
        TilePrefabList = AssetDatabase.FindAssets("t:Prefab MapTile", new string[] {$"Assets/BaseGame/Prefabs/Map/{MapStyle}"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
            .ToList();
        
        TileSprites.ForEach(sprite =>
        {
            if (TilePrefabList.Any(p => p.name == $"MapTile_{sprite.name}")) return;
            GameObject newSprite = Instantiate(BaseMapObject);
            newSprite.GetComponent<SpriteRenderer>().sprite = sprite;
            newSprite.name = sprite.name;
            
            PrefabUtility.SaveAsPrefabAsset(newSprite, $"Assets/BaseGame/Prefabs/Map/{MapStyle}/MapTile_{sprite.name}.prefab");
            AssetDatabase.SaveAssets();
            
            DestroyImmediate(newSprite.gameObject);
        });

        TilePrefabList = AssetDatabase.FindAssets("t:Prefab MapTile", new string[] {$"Assets/BaseGame/Prefabs/Map/{MapStyle}"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
            .ToList();
        
    }

    private void GenerateGround()
    {
        GroundPrefabList = AssetDatabase.FindAssets("t:Prefab MapGround", new string[] {$"Assets/BaseGame/Prefabs/Map/{MapStyle}"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
            .ToList();
        
        GroundSprites.ForEach(sprite =>
        {
            if (GroundPrefabList.Any(p => p.name == $"MapGround_{sprite.name}")) return;
            GameObject newSprite = Instantiate(BaseMapObject);
            newSprite.GetComponent<SpriteRenderer>().sprite = sprite;
            newSprite.name = sprite.name;
            
            PrefabUtility.SaveAsPrefabAsset(newSprite, $"Assets/BaseGame/Prefabs/Map/{MapStyle}/MapGround_{sprite.name}.prefab");
            AssetDatabase.SaveAssets();
            
            DestroyImmediate(newSprite.gameObject);
        });

        GroundPrefabList = AssetDatabase.FindAssets("t:Prefab MapGround", new string[] {$"Assets/BaseGame/Prefabs/Map/{MapStyle}"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
            .ToList();
    }
#endif
}
