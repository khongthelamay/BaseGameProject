using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonNotice : MonoBehaviour
{
    [Header("====== Button Notice ======")]
    [SerializeField] Button btnWithNotice;
    public GameObject objNotice;
    bool needAnim = false;
    Sequence sequence;
    UnityAction actionCallBack;


    private void Awake()
    {
        btnWithNotice.onClick.AddListener(OnClick);
    }

    public void SetButtonOnClick(UnityAction actionCall, bool needAnim = true) { 
        actionCallBack = actionCall;
        this.needAnim = needAnim;
    }

    public void OnClick()
    {
        if (needAnim)
        {
            if (sequence != null) sequence.Kill();
            sequence = UIAnimation.BasicButton(transform);
        }
      
        if (actionCallBack != null)
            actionCallBack();
    }

    public void SetInteract(bool interactable)
    {
        btnWithNotice.interactable = interactable;
    }

    public void ChangeShowNotice(bool active)
    {
        objNotice.SetActive(active);
    }
}
