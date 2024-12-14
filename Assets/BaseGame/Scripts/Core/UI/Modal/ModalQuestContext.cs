using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using UnityEngine.UI;
using TW.UGUI.Core.Modals;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;

[Serializable]
public class ModalQuestContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Action ChangeTextDaily { get; internal set; }
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }
        [field: SerializeField] public List<ReactiveValue<QuestSave>> questSaves { get; set; } = new();
        [field: SerializeField] public List<ReactiveValue<AchievementSave>> achievementSaves { get; set; } = new();
        [field: SerializeField] public List<ReactiveValue<StreakSave>> streakDailySaves { get; set; } = new();
        [field: SerializeField] public List<ReactiveValue<StreakSave>> streakWeeklySaves { get; set; } = new();
        [field: SerializeField] public ReactiveValue<float> timeDailyRemaining { get; set; } = new();
        [field: SerializeField] public ReactiveValue<double> timeWeeklyRemaining { get; set; } = new();
        [field: SerializeField] public ReactiveValue<float> currentDailyStreak { get; set; } = new();
        [field: SerializeField] public ReactiveValue<float> currentWeeklyStreak { get; set; } = new();


        public UniTask Initialize(Memory<object> args)
        {
            questSaves = QuestManager.Instance.questSaves;
            achievementSaves = AchievementManager.Instance.achievements;
            timeDailyRemaining = QuestManager.Instance.timeDailyRemaining;
            timeWeeklyRemaining = QuestManager.Instance.timeWeeklyRemaining;
            currentDailyStreak = QuestManager.Instance.currentDailyStreak;
            currentWeeklyStreak = QuestManager.Instance.currentWeeklyStreak;

            streakDailySaves = QuestManager.Instance.streakDailySaves;
            streakWeeklySaves = QuestManager.Instance.streakWeeklySaves;

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
        [field: SerializeField] public MainContentAchievement mainContentAchievement {get; private set;}
        [field: SerializeField] public MainContentStreak mainContentDailyStreak {get; private set;}
        [field: SerializeField] public MainContentStreak mainContentWeeklyStreak {get; private set;}

        [field: SerializeField] public Transform mainContent { get; private set; }

        [field: SerializeField] public Button btnClose { get; private set; }

        [field: SerializeField] public Button btnDailyQuest { get; private set; }
        [field: SerializeField] public Button btnAchievement { get; private set; }

        [field: SerializeField] public TextMeshProUGUI txtTimeDailyRemaining { get; private set; }
        [field: SerializeField] public TextMeshProUGUI txtTimeWeeklyRemaining { get; private set; }

        [field: SerializeField] public GameObject objDailyQuest { get; private set; }
        [field: SerializeField] public GameObject objAchievement { get; private set; }
        [field: SerializeField] public GameObject objRewardChest { get; private set; }

        [field: SerializeField] public Image imgTabAchievement { get; private set; }
        [field: SerializeField] public Image imgTabRewardChest { get; private set; }
        [field: SerializeField] public Transform trsTabSpite { get; private set; }

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

        public void LoadAchievementData(List<AchievementDataConfig> achievementDataConfigs) {
            mainContentAchievement.DeActiveSlotOut(0);
            mainContentAchievement.InitData(achievementDataConfigs);
            mainContentAchievement.SortSlot();
            mainContentAchievement.AnimOpen();
        }

        public void LoadStreakData(List<StreakDataConfig> streakDataConfigs) {
            mainContentDailyStreak.DeActiveSlotOut(0);
            mainContentDailyStreak.InitData(streakDataConfigs);
            mainContentDailyStreak.SetPositionStreak();
            mainContentDailyStreak.AnimOpen();
        }

        void LoadWeeklyStreakData(List<StreakDataConfig> weeklyStreaks)
        {
            mainContentWeeklyStreak.DeActiveSlotOut(0);
            mainContentWeeklyStreak.InitData(weeklyStreaks);
            mainContentWeeklyStreak.SetPositionStreak();
            mainContentWeeklyStreak.AnimOpen();
        }

        public void OnOpen() { UIAnimation.ModalOpen(MainView, mainContent, () => {
            trsTabSpite.position = btnAchievement.transform.position;
        }); }

        public void OnClose() {
            mainContentQuest.CleanAnimation();
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
            objRewardChest.SetActive(true);
            objAchievement.SetActive(false);
            trsTabSpite.position = btnAchievement.transform.position;
            LoadQuestData(QuestGlobalConfig.Instance.questDataConfigs);
            LoadStreakData(QuestGlobalConfig.Instance.dailyStreaks);
            LoadWeeklyStreakData(QuestGlobalConfig.Instance.weeklyStreaks);
        }

        public void OnAchievementShow()
        {
            if (objAchievement == currentGameObjShow)
                return;
            currentGameObjShow = objAchievement;
            objDailyQuest.SetActive(false);
            objRewardChest.SetActive(false);
            objAchievement.SetActive(true);
            trsTabSpite.position = btnDailyQuest.transform.position;
            LoadAchievementData(AchievementManager.Instance.GetAchievements());
        }

        public void SetActionCallBackOnQuestSlot(UnityAction<SlotBase<QuestDataConfig>> actionCallBack) {
            mainContentQuest.SetActionSlotCallBack(actionCallBack);
        }

        public void SetActionCallBackOnAchievementSlot(UnityAction<SlotBase<AchievementDataConfig>> actionCallBack)
        {
            mainContentAchievement.SetActionSlotCallBack(actionCallBack);
        }

        public void ChangeQuestData(ReactiveValue<int> id)
        {
            mainContentQuest.ReloadData(id.Value);
        }

        public void ChangeTextTimeRemaining(float time)
        {
            txtTimeDailyRemaining.text = TimeUtil.TimeToString(time);
        }

        public void ChangeTextTimeWeeklyRemaining(double time)
        {
            txtTimeWeeklyRemaining.text = TimeUtil.TimeToString(time, TimeFommat.Keyword);
        }

        public void ChangeDailyProgress(float value)
        {
            mainContentDailyStreak.ChangeCurrentProgress(value / QuestGlobalConfig.Instance.GetMaxValueDailyStreak());
        }

        public void ChangeWeeklyProgress(float value)
        {
            mainContentWeeklyStreak.ChangeCurrentProgress(value / QuestGlobalConfig.Instance.GetMaxValueWeeklyStreak());
        }

        public void ChangeAchievementData(ReactiveValue<int> achievementType)
        {
            mainContentAchievement.ReloadData(achievementType.Value);
        }

        public void ChangeDailyStreakData((StreakSave streakDaily, bool canClaim, bool claimed) value)
        {
            mainContentDailyStreak.ChangeStreakData(value);
        }

        public void ChangeWeeklyStreakData((StreakSave streakDaily, bool canClaim, bool claimed) value)
        {
            mainContentWeeklyStreak.ChangeStreakData(value);
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

            QuestManager.Instance.ShowModelQuest();

            View.mainContentDailyStreak.SetActionSlotCallBack(SlotDailyStreakCallBack);
            View.mainContentWeeklyStreak.SetActionSlotCallBack(SlotWeeklyStreakCallBack);

            View.OnOpen();
            View.SetActionCallBackOnQuestSlot(ActionQuestSlotCallBack);
            View.SetActionCallBackOnAchievementSlot(ActionAchievementSlotCallBack);
            View.btnClose.onClick.AddListener(View.OnClose);
            View.btnDailyQuest.onClick.AddListener(View.OnDailyQuestShow);
            View.btnAchievement.onClick.AddListener(View.OnAchievementShow);

            for (int i = 0; i < Model.questSaves.Count; i++)
            {
                Model.questSaves[i].ReactiveProperty
                    .CombineLatest(Model.questSaves[i].Value.progress.ReactiveProperty, (questSave, progress) => (questSave, progress))
                    .Subscribe(ChangeQuestData)
                    .AddTo(View.MainView);
            }

            for (int i = 0; i < Model.achievementSaves.Count; i++)
            {
                Model.achievementSaves[i].ReactiveProperty
                    .CombineLatest(Model.achievementSaves[i].Value.currentProgress.ReactiveProperty, (achievement, currentProgress) => (achievement, currentProgress))
                    .Subscribe(ChangeAchievementData)
                    .AddTo(View.MainView);
            }

            for (int i = 0; i < Model.streakDailySaves.Count; i++)
            {
                Model.streakDailySaves[i].ReactiveProperty
                    .CombineLatest(Model.streakDailySaves[i].Value.canClaim.ReactiveProperty, (streakDaily, canClaim) => (streakDaily, canClaim))
                    .CombineLatest(Model.streakDailySaves[i].Value.claimed.ReactiveProperty, (canClaim, claimed) => (canClaim.streakDaily, canClaim.canClaim , claimed))
                    .Subscribe(ChangeDailyStreakData)
                    .AddTo(View.MainView);
            }

            for (int i = 0; i < Model.streakWeeklySaves.Count; i++)
            {
                Model.streakWeeklySaves[i].ReactiveProperty
                    .CombineLatest(Model.streakWeeklySaves[i].Value.canClaim.ReactiveProperty, (streakDaily, canClaim) => (streakDaily, canClaim))
                    .CombineLatest(Model.streakWeeklySaves[i].Value.claimed.ReactiveProperty, (canClaim, claimed) => (canClaim.streakDaily, canClaim.canClaim, claimed))
                    .Subscribe(ChangeWeeklyStreakData)
                    .AddTo(View.MainView);
            }

            //Events.ChangeTextDaily = View.ChangeTextTimeRemaining;
            Model.timeDailyRemaining.ReactiveProperty.Subscribe(View.ChangeTextTimeRemaining).AddTo(View.MainView);
            Model.timeWeeklyRemaining.ReactiveProperty.Subscribe(View.ChangeTextTimeWeeklyRemaining).AddTo(View.MainView);

            Model.currentDailyStreak.ReactiveProperty.Subscribe(View.ChangeDailyProgress).AddTo(View.MainView);
            Model.currentWeeklyStreak.ReactiveProperty.Subscribe(View.ChangeWeeklyProgress).AddTo(View.MainView);

            View.OnDailyQuestShow();
        }

        private void SlotDailyStreakCallBack(SlotBase<StreakDataConfig> slotBase)
        {
            QuestManager.Instance.ClaimDailyStreak(slotBase.slotData.streak);
        }

        private void SlotWeeklyStreakCallBack(SlotBase<StreakDataConfig> slotBase)
        {
            QuestManager.Instance.ClaimWeeklyStreak(slotBase.slotData.streak);
        }

        void ChangeDailyStreakData((StreakSave streakDaily, bool canClaim, bool claimed) value)
        {
            View.ChangeDailyStreakData(value);
        }

        void ChangeWeeklyStreakData((StreakSave streakDaily, bool canClaim, bool claimed) value)
        {
            View.ChangeWeeklyStreakData(value);
        }

        void ChangeAchievementData((AchievementSave achievement, float currentProgress) value)
        {
            View.ChangeAchievementData(value.achievement.achievementType);
        }

        void ActionQuestSlotCallBack(SlotBase<QuestDataConfig> slotBase) {
            slotBase.AnimDone();
        }
        
        void ActionAchievementSlotCallBack(SlotBase<AchievementDataConfig> slotBase) {
            AchievementManager.Instance.ClaimAchievement(slotBase.slotData.achievementType);
            slotBase.AnimDone();
        }

        void ChangeQuestData((QuestSave questSave, int progress) value) {
            View.ChangeQuestData(value.questSave.id);
        }

        UniTask IModalLifecycleEvent.Cleanup(Memory<object> args)
        {
            View.mainContentAchievement.CleanAnimation();
            View.mainContentQuest.CleanAnimation();
            View.mainContentDailyStreak.CleanAnimation();
            View.mainContentWeeklyStreak.CleanAnimation();
            QuestManager.Instance.showModalQuest = false;
            Events.ChangeTextDaily = null;
            QuestManager.Instance.CheckShowNoticeQuest();
            return UniTask.CompletedTask;
        }
    }
}