using System;
using R3;
using R3.Triggers;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Manager
{
    public delegate Action<Vector3> OnMouseDownEvent(Vector3 screenPoint);
    public class CameraManager : Singleton<CameraManager>
    {
        [field: SerializeField] private Camera MainCamera {get; set;}
        private OnMouseDownEvent OnMouseDownEventCallback {get; set;}

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Mouse0))
                .Select(_ => WorldToScreenPoint(Input.mousePosition))
                .Subscribe(OnMouseDownEvent)
                .AddTo(this);
        }
        
        public Vector3 ScreenToWorldPoint(Vector3 screenPoint)
        {
            return MainCamera.ScreenToWorldPoint(screenPoint);
        }
        public Vector3 WorldToScreenPoint(Vector3 worldPoint)
        {
            return MainCamera.WorldToScreenPoint(worldPoint);
        }
        private void OnMouseDownEvent(Vector3 screenPoint)
        {
            OnMouseDownEventCallback?.Invoke(screenPoint);
        }
        public void AddListenerOnMouseClickUI(OnMouseDownEvent onMouseDownEvent)
        {
            OnMouseDownEventCallback += onMouseDownEvent;
        }
        public void RemoveListenerOnMouseClickUI(OnMouseDownEvent onMouseDownEvent)
        {
            OnMouseDownEventCallback -= onMouseDownEvent;
        }
    }
}