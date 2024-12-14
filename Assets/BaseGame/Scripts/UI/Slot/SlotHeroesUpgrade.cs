using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotHeroesUpgrade : SlotBase<HeroConfigData>
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
    public GameObject objUpgrade;

    public HeroSave heroSave;
    private void Start()
    {
        AnimOpen();
    }
    public override void InitData(HeroConfigData data)
    {
        base.InitData(data);
        animator.runtimeAnimatorController = data.ImageAnimatorController;
        imgBG.sprite = SpriteGlobalConfig.Instance.GetFrameSprite(data.HeroRarity);
        imgSubBG.sprite = imgBG.sprite;
        txtName.text = data.Name;

        objUnLock.SetActive(true);
        objPurchase.SetActive(false);
        objRequire.SetActive(false);

        heroSave = HeroManager.Instance.GetHeroSaveData(data.Name);
        txtLevel.text = $"Lv. {heroSave.level.Value}";
        pieceProgress.ChangeProgress(heroSave.piece.Value/10f);
        pieceProgress.ChangeTextProgress($"{heroSave.piece.Value}/10");
        objUpgrade.SetActive(heroSave.piece.Value / 10f == 1f);
    }

    public override void AnimOpen()
    {
        if (mySequence != null) mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence = UIAnimation.AnimSlotPopUp(trsContent);
    }

    public override void CleanAnimation()
    {
        base.CleanAnimation();
        pieceProgress.ClearAnimation();
    }
}
