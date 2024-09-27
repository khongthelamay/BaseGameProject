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
        [field: SerializeField] public TextMeshProUGUI txtName {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtLevel {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtFunDes {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtDes {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtUpgradeRequire {get; private set;}  
        [field: SerializeField] public ProgressBar piecesProgress {get; private set;}  
        [field: SerializeField] public Button btnExit {get; private set;}  
        [field: SerializeField] public Button btnUpgrade {get; private set;}  
        
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void InitData(ArtifactDataConfig artifactDataConfig, ArtifactInfor artifactInfor) {
            Debug.Log("Change Data");
            txtName.text = artifactDataConfig.strName;
            txtDes.text = artifactDataConfig.strDes;
            txtFunDes.text = artifactDataConfig.strFunDes;
            txtLevel.text = $"Lv. {artifactInfor.Level.Value}";
            txtUpgradeRequire.text = artifactDataConfig.priceUpgrade[0].ToStringUI();
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

            Model.artifactInfor.ReactiveProperty.Subscribe(ChangeArtifactData);

            View.btnExit.onClick.AddListener(OnCloseModal);
            View.btnUpgrade.onClick.AddListener(UpgradeArtifact);
            View.InitData(Model.artifactConfig, Model.artifactInfor);
        }

        void ChangeArtifactData(ArtifactInfor artifactInfor) { View.InitData(Model.artifactConfig, artifactInfor); }

        void UpgradeArtifact() { ArtifactManager.Instance.UpgradeLevelArtifact(); }

        void OnCloseModal() {
            Debug.Log("Close ModalArtifactInfor");
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
        }

        public UniTask Cleanup(Memory<object> args) {
            Events.SampleEvent = null;
            return UniTask.CompletedTask;
        }
    }
}