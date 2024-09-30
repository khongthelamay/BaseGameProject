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
using System.Collections.Generic;
using UnityEngine.Events;
using TW.UGUI.Core.Views;

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
        [field: SerializeField] public List<ReactiveValue<QuestSave>> questSaves { get; set; } = new();

        public UniTask Initialize(Memory<object> args)
        {
            questSaves = QuestManager.Instance.questSaves;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public MainContentQuest mainContentQuest {get; private set;}
        [field: SerializeField] public MainContentAchivement mainContentAchivement {get; private set;}

        [field: SerializeField] public Transform mainContent { get; private set; }

        [field: SerializeField] public Button btnClose { get; private set; }

        [field: SerializeField] public Button btnDailyQuest { get; private set; }
        [field: SerializeField] public Button btnAchievement { get; private set; }

        [field: SerializeField] public GameObject objDailyQuest { get; private set; }
        [field: SerializeField] public GameObject objAchievement { get; private set; }

        GameObject currentGameObjShow;

        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void LoadQuestData(List<QuestDataConfig> questDataConfigs) {
            mainContentQuest.DeActiveSlotOut(0);
            mainContentQuest.InitData(questDataConfigs);
            mainContentQuest.SortSlot();
            mainContentQuest.AnimOpen();
        }

        public void LoadAchievementData() {
            
        }

        public void OnOpen() { UIAnimation.ModalOpen(MainView, mainContent); }

        public void OnClose() {
            mainContentQuest.ClearAnim();
            UIAnimation.BasicButton(btnClose.transform);
            UIAnimation.ModalClose(MainView, mainContent, () => {
                ModalContainer.Find(ContainerKey.Modals).Pop(true);
            });
        }

        public void OnDailyQuestShow() {
            if (objDailyQuest == currentGameObjShow)
                return;
            currentGameObjShow = objDailyQuest;
            objDailyQuest.SetActive(true);
            objAchievement.SetActive(false);
            LoadQuestData(QuestGlobalConfig.Instance.questDataConfigs);
        }

        public void OnAchievementShow()
        {
            if (objAchievement == currentGameObjShow)
                return;
            currentGameObjShow = objAchievement;
            objDailyQuest.SetActive(false);
            objAchievement.SetActive(true);
            LoadAchievementData();
        }

        public void SetActionCallBackOnQuestSlot(UnityAction<SlotBase<QuestDataConfig>> actionCallBack) {
            mainContentQuest.SetActionSlotCallBack(actionCallBack);
        }

        public void ChangeData(ReactiveValue<int> id)
        {
            mainContentQuest.ReloadData(id);
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
            View.SetActionCallBackOnQuestSlot(ActionQuestSlotCallBack);
            View.btnClose.onClick.AddListener(View.OnClose);
            View.btnDailyQuest.onClick.AddListener(View.OnDailyQuestShow);
            View.btnAchievement.onClick.AddListener(View.OnAchievementShow);

            for (int i = 0; i < Model.questSaves.Count; i++)
            {
                Model.questSaves[i].ReactiveProperty
                    .CombineLatest(Model.questSaves[i].Value.progress.ReactiveProperty, (questSave, progress) => (questSave, progress))
                    .Subscribe(ChangeData)
                    .AddTo(View.MainView);
            }

            View.OnDailyQuestShow();
        }

        void ActionQuestSlotCallBack(SlotBase<QuestDataConfig> slotBase) {
            (slotBase as QuestSlot).AnimDone();
        }

        void ChangeData((QuestSave questSave, int progress) value) {
            View.ChangeData(value.questSave.id);
        }
    }
}