using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeInMatchType { 
    Rare,
    Epic,
    Legendary,
    Persent

}
public class SlotUpgradeInMatch : MonoBehaviour
{
    public Image imgIcon;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtPrice;
    public UpgradeInMatchType type;
    public ResourceType rType;

    public Button btnUpgrade;
    public int currentLevelUpgrade;
    public UpgradeInMatchData uInMatch;
    ReactiveValue<Resource> resource;

    private void Awake()
    {
        btnUpgrade.onClick.AddListener(OnUpgrade);
        currentLevelUpgrade = 0;
    }

    private void Start()
    {
        resource = ResourceInMatchManager.Instance.GetResource(rType);
        resource.Value.value.ReactiveProperty.Subscribe(ChangeResource).AddTo(this);
        InitData();
    }

    void ChangeResource(BigNumber amount) {
        btnUpgrade.interactable = uInMatch != null ? ResourceInMatchManager.Instance.IsEnough(rType, uInMatch.resource.value.Value) : false;
    }

    void InitData() {
        uInMatch = ResourceInMatchManager.Instance.GetUpgradeInMatchConfig(currentLevelUpgrade, type);
        txtPrice.text = uInMatch != null ? uInMatch.resource.value.Value.ToString() : "Max";
        btnUpgrade.interactable = uInMatch != null ? ResourceInMatchManager.Instance.IsEnough(rType, uInMatch.resource.value.Value) : false;
        txtLevel.text = $"Lev.{currentLevelUpgrade}";
    }

    void OnUpgrade() {
        Debug.Log($"On upgrade {type}");
        switch (type)
        {
            case UpgradeInMatchType.Rare:
                break;
            case UpgradeInMatchType.Epic:
                break;
            case UpgradeInMatchType.Legendary:
                break;
            case UpgradeInMatchType.Persent:
                break;
            default:
                break;
        }
        ResourceInMatchManager.Instance.ConsumeResource(rType, uInMatch.resource.value.Value);
        currentLevelUpgrade++;
        InitData();
    }
}
