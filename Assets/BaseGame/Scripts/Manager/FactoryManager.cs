using Core;
using Core.SimplePool;
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
        [field: Title("Damage Number")]
        [field: SerializeField] private DamageNumber DamageNumberMesh {get; set;}
        [field: SerializeField] private Color[] TextColor {get; set;}
        [field: SerializeField] private int[] TextSize {get; set;}
        
        [field: Title("SpawnEffect")]
        [field: SerializeField] private VisualEffect SpawnEffect {get; set;}
        
        [field: Title("UpgradeEffect")]
        [field: SerializeField] private VisualEffect UpgradeEffect {get; set;}
        public void CreateDamageNumber(Vector3 position, BigNumber damage, DamageType damageType, bool isCritical = false)
        {
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

        public void CreateUpgradeEffect(Vector3 position)
        {
            UpgradeEffect.Spawn(position, Quaternion.identity);
        }
    }
}