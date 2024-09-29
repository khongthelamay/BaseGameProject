using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using UGUI.Core.Modals;
using TMPro;
using UnityEngine.UI;
using TW.UGUI.Core.Modals;
using System.Collections.Generic;
using DG.Tweening;

[Serializable]
public class ModalArtifactInforContext 
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
        [field: SerializeField] public ReactiveValue<int> sample { get; set; }
        public ReactiveValue<ArtifactDataConfig> artifactConfig;
        public ReactiveValue<ArtifactInfor> artifactInfor;
        public UniTask Initialize(Memory<object> args)
        {
            artifactConfig = ArtifactManager.Instance.currentArtifactOnChoose;
            artifactInfor = ArtifactManager.Instance.currentArtifactTemp;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}  
        [field: SerializeField] public Transform mainContent {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtName {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtLevel {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtFunDes {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtDes {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtUpgradeRequire {get; private set;}  
        [field: SerializeField] public ProgressBar piecesProgress {get; private set;}  
        [field: SerializeField] public Button btnExit {get; private set;}  
        [field: SerializeField] public Button btnUpgrade {get; private set;}

        UIAnimData animOpenData;
        UIAnimData animCloseData;

        public UniTask Initialize(Memory<object> args)
        {
            animOpenData = UIAnimGlobalConfig.Instance.GetAnimData(UIAnimType.OpenPopup);
            animCloseData = UIAnimGlobalConfig.Instance.GetAnimData(UIAnimType.ClosePopup);
            return UniTask.CompletedTask;
        }

        public void InitData(ArtifactDataConfig artifactDataConfig, ArtifactInfor artifactInfor) {

            txtName.text = artifactDataConfig.strName;
            txtDes.text = artifactDataConfig.strDes;
            txtFunDes.text = artifactDataConfig.strFunDes;
            txtLevel.text = $"Lv. {artifactInfor.Level.Value}";
            

            if (artifactInfor.Level.Value >= artifactDataConfig.piecesRequire.Count)
            {
                piecesProgress.ChangeProgress(1);
                piecesProgress.ChangeTextProgress($"{artifactInfor.PiecesAmount.Value}");
                btnUpgrade.interactable = false;
                txtUpgradeRequire.text = "";
            }
            else
            {
                piecesProgress.ChangeProgress((float)artifactInfor.PiecesAmount.Value / (float)artifactDataConfig.piecesRequire[artifactInfor.Level.Value]);
                piecesProgress.ChangeTextProgress($"{artifactInfor.PiecesAmount.Value}/{artifactDataConfig.piecesRequire[artifactInfor.Level.Value]}");
                btnUpgrade.interactable = artifactInfor.PiecesAmount.Value >= artifactDataConfig.piecesRequire[artifactInfor.Level.Value];
                txtUpgradeRequire.text = artifactDataConfig.priceUpgrade[artifactInfor.Level.Value].ToStringUI();
            }

            
        }

        public void AnimOpen() {
            UIAnimation.ModalOpen(MainView, mainContent);
        }

        public void AnimClose()
        {
            UIAnimation.BasicButton(btnExit.transform);
            UIAnimation.ModalClose(MainView, mainContent, () => {
                ModalContainer.Find(ContainerKey.Modals).Pop(true);
            });
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();       

        [Button]
        public async UniTask Initialize(Memory<object> args)
        {
            Debug.Log("Init Modal Artifact Infor");


            await Model.Initialize(args);
            await View.Initialize(args);


            View.AnimOpen();

            Model.artifactInfor.ReactiveProperty
                   .CombineLatest(Model.artifactInfor.Value.Level.ReactiveProperty, (artifactInfo, artifactLevel) => (artifactInfo, artifactLevel))
                   .CombineLatest(Model.artifactInfor.Value.PiecesAmount.ReactiveProperty, (artifact, artifactAmount) => (artifact.artifactInfo, artifact.artifactLevel, artifactAmount))
                   .Subscribe(ChangeData)
                   .AddTo(View.MainView);

            View.btnExit.onClick.AddListener(OnCloseModal);
            View.btnUpgrade.onClick.AddListener(UpgradeArtifact);
        }

        void ChangeData((ArtifactInfor artifactInfo, int artifactLevel, int artifactAmount) obj)
        {
            View.InitData(Model.artifactConfig, obj.artifactInfo);
        }

        void UpgradeArtifact() { ArtifactManager.Instance.UpgradeLevelArtifact(); }

        void OnCloseModal() {
            View.AnimClose();
          
            
        }

        public UniTask Cleanup(Memory<object> args) {
            Events.SampleEvent = null;
            return UniTask.CompletedTask;
        }
    }
}