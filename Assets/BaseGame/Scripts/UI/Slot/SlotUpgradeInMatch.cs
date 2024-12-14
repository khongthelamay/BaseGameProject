using R3;
using System.Collections;
using System.Collections.Generic;
using Manager;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeInMatchType { 
    Rare,
    Epic,
    Legendary,
    Percent,

}
public class SlotUpgradeInMatch : MonoBehaviour
{
    private BattleManager BattleManagerCache { get; set; }
    private BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
    private UpgradeInMatchGlobalConfig UpgradeInMatchGlobalConfigCache { get; set; }
    private UpgradeInMatchGlobalConfig UpgradeInMatchGlobalConfig => UpgradeInMatchGlobalConfigCache ??= UpgradeInMatchGlobalConfig.Instance;
    
    public Image imgIcon;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtPrice;
    public UpgradeInMatchType type;
    public ResourceType rType;

    public Button btnUpgrade;
    public UpgradeInMatchLevel upgradeLevel;
    private UpgradeInMatchData UpgradeInMatchData { get; set; }
    private ReactiveValue<int> UpgradeLevel { get; set; } = new(0);
    private GameResource ResourceConsume { get; set; } = new();

    private void Awake()
    {

        switch (type)
        {
            case UpgradeInMatchType.Rare:
                UpgradeLevel = BattleManager.CommonRareLevel;
                ResourceConsume = BattleManager.CoinResource;
                break;
            case UpgradeInMatchType.Epic:
                UpgradeLevel = BattleManager.EpicLevel;
                ResourceConsume = BattleManager.CoinResource;
                break;
            case UpgradeInMatchType.Legendary:
                UpgradeLevel = BattleManager.LegendaryMythicLevel;
                ResourceConsume = BattleManager.StoneResource;
                break;
            case UpgradeInMatchType.Percent:
                UpgradeLevel = BattleManager.ShopLevel;
                ResourceConsume = BattleManager.CoinResource;
                break;
        }
        UpgradeLevel.ReactiveProperty.Subscribe(OnUpgradeLevelChange).AddTo(this);
        ResourceConsume.ReactiveAmount.ReactiveProperty.Subscribe(OnResourceRequireChange).AddTo(this);
        btnUpgrade.onClick.AddListener(OnUpgrade);
    }
    private void OnUpgradeLevelChange(int level) {
        txtLevel.text = $"Lev.{level.ToString()}";
        UpgradeInMatchData = UpgradeInMatchGlobalConfig.GetUpgradeInMatchData(level, type);
        
        txtPrice.text = UpgradeInMatchData != null ? UpgradeInMatchData.Resource.Amount.ToStringUI() : "Max";
        bool canUpgrade = UpgradeInMatchData != null && ResourceConsume.IsEnough(UpgradeInMatchData.Resource.Amount);
        btnUpgrade.interactable = canUpgrade;
    }
    private void OnResourceRequireChange(BigNumber amount) {
        bool canUpgrade = UpgradeInMatchData != null && ResourceConsume.IsEnough(UpgradeInMatchData.Resource.Amount);
        btnUpgrade.interactable = canUpgrade;
    }
    private void OnUpgrade() {
        ResourceConsume.Consume(UpgradeInMatchData.Resource.Amount);
        switch (type)
        {
            case UpgradeInMatchType.Rare:
                BattleManager.UpgradeCommonRareLevel();
                break;
            case UpgradeInMatchType.Epic:
                BattleManager.UpgradeEpicLevel();
                break;
            case UpgradeInMatchType.Legendary:
                BattleManager.UpgradeLegendaryMythicLevel();
                break;
            case UpgradeInMatchType.Percent:
                BattleManager.UpgradeShopLevel();
                break;
        }

    }
}
