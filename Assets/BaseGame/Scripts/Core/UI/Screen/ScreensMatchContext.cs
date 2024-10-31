using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;
using UnityEngine.UI;
using TMPro;
[Serializable]
public class ScreensMatchContext 
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
        [field: SerializeField] public Button btnReroll {get; private set;}  
        [field: SerializeField] public Button btnUpgrade {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtCoin {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtCurrentGem {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtCountChamp {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtCountEnemy {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtDifficult {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtTimeWave {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtWave {get; private set;}  
        
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
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

            View.btnReroll.onClick.AddListener(Reroll);
        }

        void Reroll()
        {
            UIAnimation.BasicButton(View.btnReroll.transform);
            Debug.Log("Reroll");
        }
    }
}