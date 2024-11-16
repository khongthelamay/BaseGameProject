using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonNotice : MonoBehaviour
{
    [Header("====== Button Notice ======")]
    [SerializeField] Button btnWithNotice;
    public GameObject objNotice;
    Sequence sequence;
    UnityAction actionCallBack;


    private void Awake()
    {
        btnWithNotice.onClick.AddListener(OnClick);
    }

    public void SetButtonOnClick(UnityAction actionCall) { actionCallBack = actionCall; }

    public void OnClick()
    {
        if (sequence != null) sequence.Kill();
        sequence = UIAnimation.BasicButton(transform);
        if (actionCallBack != null)
            actionCallBack();
    }
}
