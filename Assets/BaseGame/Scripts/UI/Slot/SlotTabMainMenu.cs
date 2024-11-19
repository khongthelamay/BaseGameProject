using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public enum TabType { 
    TabShop,
    TabHeroes,
    TabBattle,
    TabArtiFact,
    TabCommingsoon
}

public class SlotTabMainMenu : MonoBehaviour
{
    [Header("---- Slot Tab Main Menu ----")]
    public int levelUnlock;

    public List<Sprite> sprBG = new();
    public List<Sprite> sprIcon = new();

    public TabType tabType;

    public Image imgIcon;
    public Image imgBG;

    public Button btnChoose;

    public TextMeshProUGUI txtTabName;

    public GameObject objNotice;
    public GameObject objLock;

    public LayoutElement layoutElement;

    public UnityAction<SlotTabMainMenu> actionCallBack;

    public Sequence sequence;

    public Vector3 offset;

    float widthDefeault;

    [ReadOnly][SerializeField]Vector3 positionDefault;

    private void Awake()
    {
        btnChoose.onClick.AddListener(OnSelect);
        widthDefeault = layoutElement.preferredWidth;
        positionDefault = imgIcon.transform.localPosition;

        imgIcon.sprite = sprIcon[0];
        imgBG.sprite = sprBG[0];
       
    }

    public void InitData(TabType tabType, UnityAction<SlotTabMainMenu> actionCallBack)
    {
        this.tabType = tabType;
        txtTabName.text = GetNameTab();
        this.actionCallBack = actionCallBack;
        txtTabName.gameObject.SetActive(false);
    }

    string GetNameTab() {
        switch (tabType)
        {
            case TabType.TabShop:
                return "<style=M>Shop</style>";
            case TabType.TabHeroes:
                return "<style=M>Heroes";
            case TabType.TabBattle:
                return "<style=M>Battle";
            case TabType.TabArtiFact:
                return "<style=M>Artifact";
            case TabType.TabCommingsoon:
                return "<style=M>CommingSoon";
            default:
                return "";
        }
    }

    public void UnlockLevel(int level)
    {
        objLock.SetActive(level < levelUnlock);
    }

    [Button]
    void OnSelect() {
        if (actionCallBack != null)
            actionCallBack(this);
    }

    public void SelectMode()
    {
        imgBG.sprite = sprBG[1];

        if (sprIcon.Count == 2)
            imgIcon.sprite = sprIcon[1];
        AnimOnSelect();
    }

    void AnimOnSelect() {
        if (sequence != null) sequence.Kill();

        sequence = DOTween.Sequence();
        txtTabName.gameObject.SetActive(true);
        sequence.Append(imgIcon.transform.DOScale(Vector3.one * 1.8f, .15f));
        sequence.Join(imgIcon.transform.DOLocalMove(positionDefault + offset, .15f).SetEase(Ease.OutBack));
        sequence.Join(
           DOVirtual.Float(widthDefeault, widthDefeault * 1.5f, .15f, (value) =>
           {
               layoutElement.preferredWidth = value;
           })
        );
        sequence.Append(txtTabName.transform.DOPunchScale(Vector3.one * 0.15f, .15f));
        sequence.Append(imgIcon.transform.DOScale(Vector3.one * 1.5f, .15f));
        sequence.Play();
    }

    [Button]
    public void OnDeSelect() {
        imgIcon.sprite = sprIcon[0];
        imgBG.sprite = sprBG[0];

        if (sequence != null) sequence.Kill();
        txtTabName.gameObject.SetActive(false);
        sequence = DOTween.Sequence();
        sequence.Append(imgIcon.transform.DOScale(Vector3.one, .1f)); 
        sequence.Join(imgIcon.transform.DOLocalMove(positionDefault, .15f).SetEase(Ease.InBack));
        if (layoutElement.preferredWidth != widthDefeault)
        {
            layoutElement.preferredWidth = widthDefeault;
        }
        
        sequence.Play();
    }
}
