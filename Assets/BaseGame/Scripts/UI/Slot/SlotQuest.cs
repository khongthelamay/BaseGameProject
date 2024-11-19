using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using System.Collections.Generic;

public class SlotQuest : SlotBase<QuestDataConfig>
{
    [Header("======= QuestSlot =======")]
    [SerializeField] List<Sprite> sprBGSlot = new();
    [SerializeField] List<Sprite> sprBGReward = new();
    [SerializeField] RectMask2D myMask;
    [SerializeField] TextMeshProUGUI txtDes;
    [SerializeField] TextMeshProUGUI txtProgress;
    [SerializeField] TextMeshProUGUI txtReward;
    [SerializeField] Image imgBG;
    [SerializeField] Image imgBGStar;
    [SerializeField] ProgressBar progressBar;
    [SerializeField] LayoutElement myLayout;

    [SerializeField] GameObject objProgressDone;
    [SerializeField] GameObject objNotice;
    [SerializeField] GameObject objTextNotice;
    [SerializeField] GameObject objLight;
    [SerializeField] GameObject objProgress;

    [SerializeField] Transform trsProgressDone;
    [SerializeField] Transform trsIconProgressDone;
    [SerializeField] Transform trsLight;
    [SerializeField] Transform pointLightStart;
    [SerializeField] Transform pointLightEnd;
    [SerializeField] Vector3 vectorRotate;

    ReactiveValue<QuestSave> questSave = new(null);

    float heightDefault;

    public override void Awake()
    {
        base.Awake();
        heightDefault = myLayout.preferredHeight;
    }

    public override void InitData(QuestDataConfig data)
    {
        base.InitData(data);
        animOnSlot.enabled = true;
        questSave = QuestManager.Instance.GetQuestSaveData(slotData.questID);
        
        txtReward.text = data.starReward.ToString();
        txtDes.text = string.Format(data.questDes, data.questRequire);
        txtProgress.text = $"{(BigNumber)questSave.Value.progress.Value} / {(BigNumber)data.questRequire}";

        progressBar.ChangeProgress((float)questSave.Value.progress.Value / (float)data.questRequire);

        btnChoose.interactable = QuestManager.Instance.IsCanClaim(questSave.Value.id.Value);

        imgBG.sprite = btnChoose.interactable ? sprBGSlot[0] : sprBGSlot[1];
        imgBGStar.sprite = btnChoose.interactable ? sprBGReward[0] : sprBGReward[2];

        objNotice.SetActive(btnChoose.interactable);
        objTextNotice.SetActive(btnChoose.interactable);

        if (questSave.Value.claimed.Value) ActionCallBackClaimed();
    }

    public override void AnimOpen() {
        if (mySequence != null) mySequence.Kill();

        mySequence = DOTween.Sequence();

        trsContent.localScale = Vector3.one;

        myMask.enabled = true;

        mySequence.Append(UIAnimation.AnimSlotVerticalOpen(myLayout, heightDefault,()=> { myMask.enabled = false; }));
    }

    public override void AnimDone()
    {
        if (mySequence != null) mySequence.Kill();

        QuestManager.Instance.ClaimQuest(slotData.questID);

        btnChoose.interactable = false;

        objLight.SetActive(true);
        objNotice.SetActive(false);

        myMask.enabled = true;
        animOnSlot.enabled = false;

        mySequence = DOTween.Sequence();
        mySequence.Append(trsLight.DOLocalMove(pointLightEnd.localPosition, .5f).From(pointLightStart.localPosition).OnComplete(() =>
        {
            imgBG.sprite = sprBGSlot[1];
            objProgress.SetActive(false);
            objProgressDone.SetActive(true);
            objTextNotice.SetActive(false);
            objLight.SetActive(false);
        }));

        mySequence.Append(trsProgressDone.DOScale(Vector3.one * 2f, .15f));
        mySequence.Append(trsProgressDone.DOScale(Vector3.one, .15f));
        mySequence.Append(trsIconProgressDone.DORotate(Vector3.zero, .15f).From(vectorRotate));
        mySequence.Append(UIAnimation.AnimSlotVerticalClose(myLayout, heightDefault, () =>
        {
            myMask.enabled = false;
            ActionCallBackClaimed();
        }).SetDelay(.25f));

        mySequence.SetDelay(.45f);
        mySequence.Play();
    }

    void ActionCallBackClaimed() {
        objProgress.SetActive(false);
        objProgressDone.SetActive(true);
        myLayout.preferredHeight = heightDefault;
        transform.SetAsLastSibling();
        imgBGStar.sprite = sprBGReward[1];
    }

    public override void ReloadData()
    {
        InitData(slotData);
    }

    public bool IsClaimed() { return objProgressDone.activeSelf; }
    public bool IsCanClaim() { return btnChoose.interactable; }

    public override void OnChoose()
    {
        if (actionChooseCallBack != null)
            actionChooseCallBack(this);
    }
}
