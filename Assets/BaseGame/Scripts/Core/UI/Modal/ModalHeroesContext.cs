using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;

[Serializable]
public class ModalHeroesContext 
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
        
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
        }      
    }
}