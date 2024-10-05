using System;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Manager;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using TW.Utility.Extension;
using UnityEngine;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Map : ACachedMonoBehaviour
{
    [Inject] private BattleManager BattleManager { get; set; }
    [Inject] private Enemy.Factory EnemyFactory { get; set; }
    [field: SerializeField] public MapDrawer MapDrawer {get; private set;}
    [field: SerializeField] private MapData MapData {get; set;}
    [field: SerializeField] private Transform[] MovePoints {get; set;}
    [field: SerializeField] public FieldSlot[] FieldSlotArray {get; private set;}
    private DateTime WaveStartTime { get; set; }
    public async UniTask StartMap()
    {
        await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
        foreach (WaveData waveData in MapData.WaveDataList)
        {
            WaveStartTime = DateTime.Now;
            int spawnInterval = (int)(waveData.SpawnInterval * 1000);
            int waveInterval = (int)(waveData.WaveInterval * 1000);
            Enemy enemyPrefab = PrefabGlobalConfig.Instance.GetEnemyPrefab(waveData.EnemyId);
            for (int i = 0; i < waveData.EnemyCount; i++)
            {
                Enemy enemy = EnemyFactory.Create(enemyPrefab);
                enemy.transform.position = MovePoints[0].position;
                enemy.SetupMovePoint(MovePoints);
                enemy.StartMoveToNextPoint();
                BattleManager.AddEnemy(enemy);
                
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