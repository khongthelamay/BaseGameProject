using System;
using System.Linq;
using Core;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Manager;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEngine;

#if UNITY_EDITOR
#endif

public class Map : ACachedMonoBehaviour
{
    private BattleManager BattleManagerCache { get; set; }
    private BattleManager BattleManager => BattleManagerCache ??= BattleManagerCache = BattleManager.Instance;
    
    [field: SerializeField] public MapDrawer MapDrawer {get; private set;}
    [field: SerializeField] private MapData MapData {get; set;}
    [field: SerializeField] private Transform[] MovePoints {get; set;}
    [field: SerializeField] public FieldSlot[] FieldSlotArray {get; private set;}
    private DateTime WaveStartTime { get; set; }
    public async UniTask StartMap()
    {
        await UniTask.Delay(5000, cancellationToken: this.GetCancellationTokenOnDestroy());
        foreach (WaveData waveData in MapData.WaveDataList)
        {
            WaveStartTime = DateTime.Now;
            int spawnInterval = (int)(waveData.SpawnInterval * 1000);
            int waveInterval = (int)(waveData.WaveInterval * 1000);
            Enemy enemyPrefab = PrefabGlobalConfig.Instance.GetEnemyPrefab(waveData.EnemyId);
            for (int i = 0; i < waveData.EnemyCount; i++)
            {
                Enemy enemy = enemyPrefab
                    .Spawn(MovePoints[0].position, Quaternion.identity)
                    .SetupMovePoint(MovePoints)
                    .InitStats(100000, 1.5f);
                
                enemy.StartMoveToNextPoint();
                
                await UniTask.Delay(spawnInterval, cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            await UniTask.Delay(waveInterval, cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }
    
#if UNITY_EDITOR
    [Button]
    public void InitFieldSlot(int row, int column)
    {
        FieldSlotArray = Transform.GetComponentsInChildren<FieldSlot>().OrderBy(g => g.gameObject.name).ToArray();
    }
#endif
}