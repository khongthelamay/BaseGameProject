using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopPack : MonoBehaviour
{
    public PackID packID;
    public ShopPackData shopPackData;
    public TextMeshProUGUI txtPrice;
    public TextMeshProUGUI txtPacName;
    public List<ShopDealReward> shopDealRewards = new();

    int rewardIndex = 0;

    private void Awake()
    {
        InitData();
    }

    public void InitData() {
        shopPackData = ShopGlobalConfig.Instance.GetShopPackData(packID);
        if (shopPackData == null)
            return;
        if (txtPrice != null)
            txtPrice.text = shopPackData.PaymentAmount.value.Value.ToString();
        rewardIndex = 0;
        for (int i = 0; i < shopPackData.rewards.Count; i++)
        {
            if (rewardIndex < shopDealRewards.Count)
            {
                shopDealRewards[i].InitData(shopPackData.rewards[i]);
                shopDealRewards[i].gameObject.SetActive(true);
                rewardIndex++;
            }
            else break;
        }

        for (int i = 0; i < shopPackData.heroRewards.Count; i++)
        {
            if (rewardIndex < shopDealRewards.Count)
            {
                shopDealRewards[rewardIndex].InitData(shopPackData.heroRewards[i]);
                shopDealRewards[rewardIndex].gameObject.SetActive(true);
                rewardIndex++;
            }
            else break;
        }

        for (int i = rewardIndex; i < shopDealRewards.Count; i++)
        {
            shopDealRewards[i].gameObject.SetActive(false);
        }
    }
}
