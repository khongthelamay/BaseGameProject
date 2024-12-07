using System;
using Core;
using Cysharp.Threading.Tasks;
using Manager;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Activities;
using UnityEngine.UI;

[Serializable]
public class ActivityHeroInfoContext 
{
    public static class Events
    {
        public static Action<FieldSlot> ShowFieldSlotInteract { get; set; }
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public FieldSlot FieldSlot { get; set; }
        [field: SerializeField] public Hero OwnHero { get; set; }


        public UniTask Initialize(Memory<object> args)
        {   
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;} 
        [field: SerializeField] public GameObject Root {get; private set;}

        [field: SerializeField] public Transform InteractGroup { get; private set; }
        [field: SerializeField] public Button ButtonMerge { get; private set; }
        [field: SerializeField] public Button ButtonSell { get; private set; }
        public UniTask Initialize(Memory<object> args)
        {
            Root.SetActive(false);
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
    {
        private CameraManager CameraManagerCache { get; set; }
        private CameraManager CameraManager => CameraManagerCache ??= CameraManager.Instance;
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        
        
        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            
            View.ButtonMerge.SetOnClickDestination(OnButtonMergeClick);
            View.ButtonSell.SetOnClickDestination(OnButtonSellClick);
            Events.ShowFieldSlotInteract = ShowFieldSlotInteract;
        }     
        private void ShowFieldSlotInteract(FieldSlot fieldSlot)
        {
            if (Model.FieldSlot != null && Model.FieldSlot.TryGetHero(out Hero oldHero))
            {
                oldHero.HideAttackRange();
            }

            Model.FieldSlot = fieldSlot;

            if (Model.FieldSlot == null || !Model.FieldSlot.TryGetHero(out Hero newHero))
            {
                Model.OwnHero = null;
                View.Root.SetActive(false);
                return;
            }
            
            View.Root.SetActive(true);
            Model.OwnHero = Model.FieldSlot.Hero;
            Model.OwnHero.ShowAttackRange();
            View.ButtonMerge.interactable = Model.FieldSlot.CanFusionHero();
            
            View.InteractGroup.position = CameraManager.WorldToScreenPoint(Model.OwnHero.Transform.position);
        }
        private async UniTask OnButtonMergeClick()
        {
            View.Root.SetActive(false);
            await Model.FieldSlot.TryFusionHero();
        }

        private void OnButtonSellClick(Unit _)
        {
            View.Root.SetActive(false);
            Model.FieldSlot.TrySellHero();
        }
        
        private void OnHide()
        {
            Model.OwnHero?.HideAttackRange();
            Model.OwnHero = null;
            View.Root.SetActive(false);
        }
        
    }
}