using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class FieldManager : Singleton<FieldManager>
{
    public static class Events
    {
        public static Action OnFieldSlotChange { get; set; }
        
    }
    [field: SerializeField] public MapData MapData {get; private set;}
    [field: SerializeField] public int Row {get; private set;}
    [field: SerializeField] public int Column {get; private set;}
    [field: SerializeField] public Transform FieldSlotContainer {get; private set;}
    [field: SerializeField] public FieldSlot[] FieldSlotArray {get; private set;}
    [field: SerializeField] public Transform[] MovePoint {get; private set;}
    [field: SerializeField] public WaitSlot[] WaitSlotArray {get; private set;}
    public List<Enemy> EnemyList { get; private set; } = new List<Enemy>();
    private DateTime WaveStartTime { get; set; }
    protected override void Awake()
    {
        base.Awake();
        Events.OnFieldSlotChange += OnFieldSlotChange;
    }

    private void Start()
    {
        StartMap().Forget();
        RandomWaitSlot();

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.C))
            .Subscribe(_ => RandomWaitSlot());
    }
    public bool TryFusionHeroInFieldSlot(FieldSlot fieldSlot)
    {
        if (!TryGetSameHeroInFieldSlot(fieldSlot, out List<FieldSlot> sameHeroFieldSlotList)) return false;
        if (sameHeroFieldSlotList.Count < 2) return false;
        foreach (var slot in sameHeroFieldSlotList)
        {
            slot.RemoveHero();
        }

        if (fieldSlot.TryUpgradeHero())
        {
            OnFieldSlotChange();
        }
        return false;
    }

    private bool TryGetSameHeroInFieldSlot(FieldSlot fieldSlot, out List<FieldSlot> sameHeroFieldSlotList)
    {
        sameHeroFieldSlotList = new List<FieldSlot>();
        if (!fieldSlot.TryGetHero(out Hero hero)) return false;
        foreach (FieldSlot slot in FieldSlotArray)
        {
            if (slot == fieldSlot) continue;
            if (!IsFieldSlotHasSameHeroData(slot, hero))
            {
                continue;
            }
            sameHeroFieldSlotList.Add(slot);
            if (sameHeroFieldSlotList.Count == 2) break;
        }
        return sameHeroFieldSlotList.Count == 2;
    }

    private bool IsFieldSlotHasSameHeroData(FieldSlot fieldSlot, Hero hero)
    {
        return fieldSlot.TryGetHero(out Hero otherHero) && otherHero.HeroStatData == hero.HeroStatData;
    }

    private void OnFieldSlotChange()
    {
        foreach (FieldSlot slot in FieldSlotArray)
        {
            slot.SetUpgradeMark(TryGetSameHeroInFieldSlot(slot, out List<FieldSlot> _));
        }
    }
    
    [Button]
    public void RandomWaitSlot()
    {
        foreach (var waitSlot in WaitSlotArray)
        {
            waitSlot.RandomOwnerHero();
        }
    }

    public bool TryAddHeroToFieldSlot(Hero hero)
    {
        foreach (FieldSlot fieldSlot in FieldSlotArray)
        {
            if (fieldSlot.TryAddHeroFromWaitSlot(hero))
            {
                OnFieldSlotChange();
                return true;
            }
        }

        return false;
    }
    public bool TrySellHeroInFieldSlot(FieldSlot fieldSlot)
    {
        fieldSlot.RemoveHero();
        OnFieldSlotChange();
        return true;
    }

    public async UniTask StartMap()
    {
        await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
        foreach (var waveData in MapData.WaveDataList)
        {
            WaveStartTime = DateTime.Now;
            int spawnInterval = (int)(waveData.SpawnInterval * 1000);
            int waveInterval = (int)(waveData.WaveInterval * 1000);
            Enemy enemyPrefab = PrefabGlobalConfig.Instance.GetEnemyPrefab(waveData.EnemyId);
            for (int i = 0; i < waveData.EnemyCount; i++)
            {
                Enemy enemy = Instantiate(enemyPrefab);
                enemy.transform.position = MovePoint[0].position;
                enemy.SetupMovePoint(MovePoint);
                enemy.StartMoveToNextPoint();
                EnemyList.Add(enemy);
                await UniTask.Delay(spawnInterval, cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            await UniTask.Delay(waveInterval, cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }
    public void RemoveEnemy(Enemy enemy)
    {
        if (!EnemyList.Contains(enemy)) return;
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}

#if UNITY_EDITOR
public partial class FieldManager
{
    [field: Title("Editor Only")]
    [field: SerializeField] public FieldSlot FieldSlotPrefab {get; private set;}

    [Button]
    public void InitFieldSlot()
    {
        foreach (var fieldSlot in FieldSlotArray)
        {
            DestroyImmediate(fieldSlot.gameObject);
        }
        FieldSlotArray = new FieldSlot[Row * Column];
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                FieldSlotArray[i * Column + j] = PrefabUtility.InstantiatePrefab(FieldSlotPrefab) as FieldSlot;
                FieldSlotArray[i * Column + j]
                    .SetupTransform(FieldSlotContainer)
                    .SetupCoordinate(i, j)
                    .SetupPosition(Row, Column);
            }
        }
    }
}

#endif