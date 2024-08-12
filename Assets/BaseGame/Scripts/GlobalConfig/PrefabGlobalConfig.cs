using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "PrefabGlobalConfig", menuName = "GlobalConfigs/PrefabGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class PrefabGlobalConfig : GlobalConfig<PrefabGlobalConfig>
{
    [field: SerializeField] public List<PrefabConfig<Enemy>> EnemyPrefabConfigList {get; private set;}
    
    public Enemy GetEnemyPrefab(int id)
    {
        return EnemyPrefabConfigList.Find(x => x.Id == id).Prefab;
    }

}
[System.Serializable]
public class PrefabConfig<T>
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public T Prefab { get; private set; }
}