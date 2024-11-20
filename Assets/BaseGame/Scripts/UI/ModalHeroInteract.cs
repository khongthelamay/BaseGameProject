using Core;
using Cysharp.Threading.Tasks;
using Manager;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.UI;

public class ModalHeroInteract : ACachedMonoBehaviour
{
    [field: SerializeField] public FieldSlot FieldSlot { get; private set; }
    [field: SerializeField] public Hero OwnHero { get; private set; }
    [field: SerializeField] public Transform InteractGroup { get; private set; }
    [field: SerializeField] public Button ButtonMerge { get; private set; }
    [field: SerializeField] public Button ButtonSell { get; private set; }
    [field: SerializeField] public Button ButtonClose { get; private set; }

    private void Awake()
    {
        ButtonMerge.onClick.AddListener(OnButtonMergeClick);
        ButtonSell.onClick.AddListener(OnButtonSellClick);
        ButtonClose.onClick.AddListener(OnButtonCloseClick);
    }

    public void SetFieldSlot(FieldSlot fieldSlot)
    {
        if (FieldSlot != null && FieldSlot.TryGetHero(out Hero hero))
        {
            hero.HideAttackRange();
        }

        FieldSlot = fieldSlot;
        if (OwnHero != null)
        {
            OwnHero.HideAttackRange();
        }

        OwnHero = FieldSlot.Hero;
        OwnHero.ShowAttackRange();
        InteractGroup.position = OwnHero.transform.position;
    }

    private void OnButtonMergeClick()
    {
        if (!BattleManager.Instance.CanFusionHeroInFieldSlot(FieldSlot)) return;
        BattleManager.Instance.FusionHeroInFieldSlot(FieldSlot).Forget();
        OnClose();
    }

    private void OnButtonSellClick()
    {
        if (BattleManager.Instance.TrySellHero(FieldSlot))
        {
            Debug.Log("Sell");
            OnClose();
        }
    }

    private void OnButtonCloseClick()
    {
        OnClose();
    }

    private void OnClose()
    {
        OwnHero?.HideAttackRange();
        OwnHero = null;
        gameObject.SetActive(false);
    }
}