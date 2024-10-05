using Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotHeroesUpgrade : SlotBase<HeroStatData>
{
    [Header("========= Slot Heroes Upgrade =========")]
    public Image imgBG;
    public ProgressBar pieceProgress;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtRequire;
    public TextMeshProUGUI txtLevel;
    public GameObject objUnLock;
    public GameObject objPurchase;
    public GameObject objRequire;

    HeroSave heroSave;
    private void Start()
    {
        AnimOpen();
    }
    public override void InitData(HeroStatData data)
    {
        base.InitData(data);
        
        imgIcon.sprite = data.HeroSprite;
        txtName.text = data.Name;

        objUnLock.SetActive(true);
        objPurchase.SetActive(false);
        objRequire.SetActive(false);
        pieceProgress.ChangeProgress(0);

        heroSave = HeroManager.Instance.GetHeroSaveData(data.Name);
        txtLevel.text = $"Lv. {heroSave.level.Value}";
        pieceProgress.ChangeProgress(0);
        pieceProgress.ChangeTextProgress("0/0");
    }

    public override void AnimOpen()
    {
        UIAnimation.AnimSlotPopUp(trsContent);
    }
}
