using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPack : SlotBase<ShopPackData>
{
    public ShopPackData shopPackData;
    public TextMeshProUGUI txtPrice;
    public TextMeshProUGUI txtPackName;
    public List<ShopDealReward> shopDealRewards = new();
    int rewardIndex = 0;

    public override void InitData(ShopPackData shopPackData)
    {
        slotData = shopPackData;

        if (slotData == null)
            return;
        if (!slotData.isShow)
        {
            gameObject.SetActive(false);
            return;
        }

        if (txtPackName != null)
            txtPackName.text = slotData.PackageName;

        if (txtPrice != null)
            txtPrice.text = slotData.PaymentAmount.value.Value.ToString();
        rewardIndex = 0;
        for (int i = 0; i < slotData.rewards.Count; i++)
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
