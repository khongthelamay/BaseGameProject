using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using UGUI.Core.Modals;
using UnityEngine.UI;
using TW.UGUI.Core.Modals;

[Serializable]
public class ModalQuestContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }

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
        [field: SerializeField] public Transform mainContent { get; private set; }

        [field: SerializeField] public Button btnClose { get; private set; }

        [field: SerializeField] public Button btnDailyQuest { get; private set; }
        [field: SerializeField] public Button btnAchivement { get; private set; }

        [field: SerializeField] public GameObject objDailyQuest { get; private set; }
        [field: SerializeField] public GameObject objAchivement { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void OnOpen() { UIAnimation.ModalOpen(MainView, mainContent); }
        public void OnClose() {
            UIAnimation.BasicButton(btnClose.transform);
            UIAnimation.ModalClose(MainView, mainContent, () => {
                ModalContainer.Find(ContainerKey.Modals).Pop(true);
            });
        }

        public void OnDailyQuestShow() {
            objDailyQuest.SetActive(true);
            objAchivement.SetActive(false);
        }

        public void OnAchivementShow()
        {
            objDailyQuest.SetActive(false);
            objAchivement.SetActive(true);
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            View.OnOpen();
            View.btnClose.onClick.AddListener(View.OnClose);
            View.btnDailyQuest.onClick.AddListener(View.OnDailyQuestShow);
            View.btnAchivement.onClick.AddListener(View.OnAchivementShow);
        }      
    }
}