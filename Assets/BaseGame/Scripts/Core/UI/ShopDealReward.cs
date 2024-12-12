using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopDealReward : MonoBehaviour
{
    public Image imgBG;
    public Image imgIcon;
    public Animator heroIcon;
    public TextMeshProUGUI txtReward;
    public Button btnChoose;
    public HeroConfigData heroConfig;
    public Resource resource;
    private void Awake()
    {
        btnChoose.onClick.AddListener(OnChoose);
    }

    void OnChoose() {
        Debug.Log($"Onchoose reward {resource.type}");
    }

    public void InitData(Resource resource)
    {
        imgIcon.gameObject.SetActive(true);
        heroIcon.gameObject.SetActive(false);
        txtReward.text = resource.value.Value.ToString();
        this.resource = resource;
    }

    public void InitData(HeroReward heroReward) 
    {
        imgIcon.gameObject.SetActive(false);
        heroIcon.gameObject.SetActive(true);
        heroConfig = HeroManager.Instance.GetHeroConfigData(heroReward.heroID);
        txtReward.text = heroReward.amount.ToString();
        heroIcon.runtimeAnimatorController = heroConfig.ImageAnimatorController;
    }
}
