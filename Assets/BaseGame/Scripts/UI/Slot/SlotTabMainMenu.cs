using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;

public enum TabType { 
    TabShop,
    TabHeroes,
    TabBattle,
    TabArtiFact,
    TabCommingsoon
}

public class SlotTabMainMenu : MonoBehaviour
{
    public TabType tabType;

    public Image imgIcon;

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
    }

    public void InitData(TabType tabType, UnityAction<SlotTabMainMenu> actionCallBack)
    {
        this.tabType = tabType;
        txtTabName.text = tabType.ToString();
        this.actionCallBack = actionCallBack;
        txtTabName.gameObject.SetActive(false);
    }

    [Button]
    void OnSelect() {
        if (actionCallBack != null)
            actionCallBack(this);
    }

    public void AnimOnSelect() {
        if (sequence != null) sequence.Kill();

        sequence = DOTween.Sequence();
        txtTabName.gameObject.SetActive(true);
        sequence.Append(imgIcon.transform.DOScale(Vector3.one * 1.5f, .15f));
        sequence.Join(imgIcon.transform.DOLocalMove(positionDefault + offset, .15f).SetEase(Ease.OutBack));
        sequence.Join(
           DOVirtual.Float(widthDefeault, widthDefeault * 1.5f, .15f, (value) =>
           {
               layoutElement.preferredWidth = value;
           })
        );
        sequence.Append(txtTabName.transform.DOPunchScale(Vector3.one * 0.15f, .15f));
        sequence.Append(imgIcon.transform.DOScale(Vector3.one * 1.3f, .15f));
        sequence.Play();
    }

    [Button]
    public void OnDeSelect() {
        if (sequence != null) sequence.Kill();
        txtTabName.gameObject.SetActive(false);
        sequence = DOTween.Sequence();
        sequence.Append(imgIcon.transform.DOScale(Vector3.one, .1f)); 
        sequence.Join(imgIcon.transform.DOLocalMove(positionDefault, .15f).SetEase(Ease.InBack));
        if (layoutElement.preferredWidth != widthDefeault)
        {
            layoutElement.preferredWidth = widthDefeault;
            //sequence.Append(
            //            DOVirtual.Float(widthDefeault * 1.5f, widthDefeault, .15f, (value) =>
            //            {
            //                layoutElement.preferredWidth = value;
            //            })
            //        );
        }
        
        sequence.Play();
    }
}
