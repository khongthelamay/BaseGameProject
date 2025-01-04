using System;
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
public class ActivityUpgradeInMatchContext 
{
    public static class Events
    {
        
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public GameResource CoinResource { get; private set; }
        [field: SerializeField] public GameResource StoneResource { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            CoinResource = BattleManager.Instance.CoinResource;
            StoneResource = BattleManager.Instance.StoneResource;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}  
        [field: SerializeField] public Button btnExit {get; private set;}  
        
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            View.btnExit.onClick.AddListener(CloseActivity);
        }

        void CloseActivity() {
            ActivityContainer.Find(ContainerKey.Activities).Hide(nameof(ActivityUpgradeInMatch));
        }
    }
}