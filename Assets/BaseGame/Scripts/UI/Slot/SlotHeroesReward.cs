using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotHeroesReward : SlotBase<RecruitReward>
{
    public Animator animator;
    public Image imgBG;
    public Image imgSubBG;
    public ProgressBar pieceProgress;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtRequire;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtCountReward;

    HeroConfigData heroConfig;
    HeroSave heroSave;

    public override void InitData(RecruitReward data)
    {
        base.InitData(data);
        heroConfig = data.heroData;
        heroSave = HeroManager.Instance.GetHeroSaveData(heroConfig.Name);

        animator.runtimeAnimatorController = heroConfig.ImageAnimatorController;
        imgBG.sprite = SpriteGlobalConfig.Instance.GetFrameSprite(heroConfig.HeroRarity);
        imgSubBG.sprite = imgBG.sprite;
        txtName.text = heroConfig.Name;

        pieceProgress.ChangeProgress(0);

        heroSave = HeroManager.Instance.GetHeroSaveData(heroConfig.Name);
        txtLevel.text = $"Lv. {heroSave.level.Value}";
        pieceProgress.ChangeProgress(0);
        pieceProgress.ChangeTextProgress("0/0");

        txtCountReward.text = data.amount.ToString();
    }
}
