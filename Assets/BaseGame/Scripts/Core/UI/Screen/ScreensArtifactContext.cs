using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class ScreensArtifactContext 
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

        [field: SerializeField] public List<ReactiveValue<ArtifactInfor>> ArtifactInfos { get; set; } = new();

        public UniTask Initialize(Memory<object> args)
        {
            ArtifactInfos = ArtifactManager.Instance.ArtifactInfos;
            Debug.Log(ArtifactInfos == null);
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}  
        [field: SerializeField] public ArtifactMainContent artifactMainContent {get; private set;}  

        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void InitData(List<ArtifactDataConfig> listData) {
            artifactMainContent.InitData(listData); 
        }

        public void ReloadData(int artifactID)
        {
            artifactMainContent.ReloadData(artifactID);
        }

        public void SetActionCallbackArtifactSlot(UnityAction<SlotBase<ArtifactDataConfig>> actionCallback) { artifactMainContent.SetActionSlotCallBack(actionCallback); }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        


        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            View.SetActionCallbackArtifactSlot(ActionCallBackArtifactSlot);
            for (int i = 0; i < Model.ArtifactInfos.Count; i++)
            {
                Model.ArtifactInfos[i].ReactiveProperty
                    .CombineLatest(Model.ArtifactInfos[i].Value.Level.ReactiveProperty, (artifactInfo, artifactLevel) => (artifactInfo, artifactLevel))
                    .CombineLatest(Model.ArtifactInfos[i].Value.PiecesAmount.ReactiveProperty, (artifact, artifactAmount) => (artifact.artifactInfo, artifact.artifactLevel, artifactAmount))
                    .Subscribe(ChangeData)
                    .AddTo(View.MainView);
            }
            View.InitData(ArtifactGlobalConfig.Instance.artifactDataConfigs);
        }

        private void ChangeData((ArtifactInfor artifactinfor, int level, int pieceAmount) value)
        {
            Debug.Log(value);
            View.ReloadData(value.artifactinfor.Id);
        }

        void ActionCallBackArtifactSlot(SlotBase<ArtifactDataConfig> slotArtifact) {
            ArtifactManager.Instance.currentArtifactOnChoose.Value = slotArtifact.slotData;
        }
    }
}