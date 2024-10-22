using System.Collections.Generic;
using System.Linq;
using Core;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseMapGenerateGlobalConfig", menuName = "GlobalConfigs/BaseMapGenerateGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class BaseMapGenerateGlobalConfig : GlobalConfig<BaseMapGenerateGlobalConfig>
{
    [field: SerializeField] public GameObject BaseMapObject {get; private set;}
    [field: SerializeField] public List<Sprite> TileSprites {get; private set;}
    [field: SerializeField] public List<Sprite> GroundSprites {get; private set;}

    [field: SerializeField] public List<GameObject> TilePrefabList {get; private set;}
    [field: SerializeField] public List<GameObject> GroundPrefabList {get; private set;}
    
#if UNITY_EDITOR

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
        TilePrefabList = AssetDatabase.FindAssets("t:Prefab MapTile", new string[] {"Assets/BaseGame/Prefabs/MapTile"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
            .ToList();
        
        TileSprites.ForEach(sprite =>
        {
            if (TilePrefabList.Any(p => p.name == $"MapTile_{sprite.name}")) return;
            GameObject newSprite = Instantiate(BaseMapObject);
            newSprite.GetComponent<SpriteRenderer>().sprite = sprite;
            newSprite.name = sprite.name;
            
            PrefabUtility.SaveAsPrefabAsset(newSprite, $"Assets/BaseGame/Prefabs/MapTile/MapTile_{sprite.name}.prefab");
            AssetDatabase.SaveAssets();
            
            DestroyImmediate(newSprite.gameObject);
        });

        TilePrefabList = AssetDatabase.FindAssets("t:Prefab MapTile", new string[] {"Assets/BaseGame/Prefabs/MapTile"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
            .ToList();
    }
    private void GenerateGround()
    {
        GroundPrefabList = AssetDatabase.FindAssets("t:Prefab MapGround", new string[] {"Assets/BaseGame/Prefabs/MapGround"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
            .ToList();
        
        GroundSprites.ForEach(sprite =>
        {
            if (GroundPrefabList.Any(p => p.name == $"MapGround_{sprite.name}")) return;
            GameObject newSprite = Instantiate(BaseMapObject);
            newSprite.GetComponent<SpriteRenderer>().sprite = sprite;
            newSprite.name = sprite.name;
            
            PrefabUtility.SaveAsPrefabAsset(newSprite, $"Assets/BaseGame/Prefabs/MapGround/MapGround_{sprite.name}.prefab");
            AssetDatabase.SaveAssets();
            
            DestroyImmediate(newSprite.gameObject);
        });

        GroundPrefabList = AssetDatabase.FindAssets("t:Prefab MapGround", new string[] {"Assets/BaseGame/Prefabs/MapGround"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
            .ToList();
    }
#endif
}
