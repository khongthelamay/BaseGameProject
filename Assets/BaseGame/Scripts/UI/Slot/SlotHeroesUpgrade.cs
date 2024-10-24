using Core;
using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotHeroesUpgrade : SlotBase<Hero>
{
    [Header("========= Slot Heroes Upgrade =========")]
    public Animator animator;
    public Image imgBG;
    public Image imgSubBG;
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
    public override void InitData(Hero data)
    {
        base.InitData(data);
        animator.runtimeAnimatorController = data.HeroStatData.ImageAnimatorController;
        imgBG.sprite = SpriteGlobalConfig.Instance.GetFrameSprite(data.HeroStatData.HeroRarity);
        imgSubBG.sprite = imgBG.sprite;
        txtName.text = data.HeroStatData.Name;

        objUnLock.SetActive(true);
        objPurchase.SetActive(false);
        objRequire.SetActive(false);
        pieceProgress.ChangeProgress(0);

        heroSave = HeroManager.Instance.GetHeroSaveData(data.HeroStatData.Name);
        txtLevel.text = $"Lv. {heroSave.level.Value}";
        pieceProgress.ChangeProgress(0);
        pieceProgress.ChangeTextProgress("0/0");
    }

    public override void AnimOpen()
    {
        if (mySequence != null) mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence = UIAnimation.AnimSlotPopUp(trsContent);
    }
}
