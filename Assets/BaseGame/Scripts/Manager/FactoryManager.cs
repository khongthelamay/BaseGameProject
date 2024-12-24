using System;
using System.Threading;
using Core;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using DamageNumbersPro;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Manager
{
    public class FactoryManager : Singleton<FactoryManager>
    {
        [field: SerializeField] public bool IsHideTextDamage {get; private set;}
        [field: Title("Damage Number")]
        [field: SerializeField] private DamageNumber DamageNumberMesh {get; set;}
        [field: SerializeField] private Color[] TextColor {get; set;}
        [field: SerializeField] private int[] TextSize {get; set;}
        
        [field: Title("SpawnEffect")]
        [field: SerializeField] private VisualEffect SpawnEffect {get; set;}
        
        [field: Title("UpgradeEffect")]
        [field: SerializeField] private VisualEffect UpgradeCommonRareEffect {get; set;}
        [field: SerializeField] private VisualEffect UpgradeEpicEffect {get; set;}
        [field: SerializeField] private VisualEffect UpgradeLegendaryMythicEffect {get; set;}
        
        [field: Title("FusionEffect")]
        [field: SerializeField] private VisualEffect FusionEffect {get; set;}
        public void CreateDamageNumber(Vector3 position, BigNumber damage, DamageType damageType, bool isCritical = false)
        {
            if (IsHideTextDamage) return;
            Color textColor = TextColor[(int)damageType * 2 + (isCritical ? 1 : 0)];
            string damageText = $"{damage.ToStringUI()}{(isCritical ? "!" : "")}";
            int textSize = TextSize[isCritical ? 1 : 0];
            DamageNumber damageNumber = DamageNumberMesh.Spawn(position, damageText);
            damageNumber.SetColor(textColor);
            damageNumber.GetTextMesh().fontSize = textSize;
        }
        
        public void CreateSpawnEffect(Vector3 position)
        {
            SpawnEffect.Spawn(position, Quaternion.identity);
        }
        
        public async UniTask CreateFusionEffect(Vector3 position, CancellationToken ct)
        {
            FusionEffect.Spawn(position, Quaternion.identity);
            await DelaySample(3, ct);
        }
        public void CreateUpgradeEffect(Vector3 position, Hero.Rarity rarity)
        {
            VisualEffect upgradeEffect = rarity switch
            {
                Hero.Rarity.Common => UpgradeCommonRareEffect,
                Hero.Rarity.Rare => UpgradeCommonRareEffect,
                Hero.Rarity.Epic => UpgradeEpicEffect,
                Hero.Rarity.Legendary => UpgradeLegendaryMythicEffect,
                Hero.Rarity.Mythic => UpgradeLegendaryMythicEffect,
                _ => null
            };
            upgradeEffect.Spawn(position, Quaternion.identity);
        }
        
        private UniTask DelaySample(int sample, CancellationToken ct)
        {
            int timeDelay = 1000 * sample / 30;
            return UniTask.Delay(timeDelay, cancellationToken: ct);
        }
    }
}