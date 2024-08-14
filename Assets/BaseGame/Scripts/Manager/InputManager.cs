using System;
using System.Collections;
using System.Collections.Generic;
using R3;
using R3.Triggers;
using TW.Utility.DesignPattern;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    [field: SerializeField] public Camera MainCamera {get; private set;}

    [field: SerializeField] public LayerMask WhatIsInteractable {get; private set;}

    private IInteractable StartInteractable { get; set; }
    private IInteractable CurrentInteractable { get; set; }
    private IInteractable EndInteractable { get; set; }
    private readonly RaycastHit[] _hitResults = new RaycastHit[1];
    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Mouse0))
            .Subscribe(OnMouseDownCallback);
        this.UpdateAsObservable()
            .Where(_ => Input.GetKey(KeyCode.Mouse0))
            .Subscribe(OnMouseCallback);
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyUp(KeyCode.Mouse0))
            .Subscribe(OnMouseUpCallback);
    }

    private void OnMouseDownCallback(Unit _)
    {
        StartInteractable = null;
        int hitCount = Physics.RaycastNonAlloc(MainCamera.ScreenPointToRay(Input.mousePosition), _hitResults, Mathf.Infinity, WhatIsInteractable);
        if (hitCount <= 0) return;
        if (!_hitResults[0].collider.TryGetComponent(out IInteractable interactable)) return;
        StartInteractable = interactable;
        StartInteractable.OnMouseDownCallback();
        
    }
    private void OnMouseCallback(Unit _)
    {
        CurrentInteractable = null;
        int hitCount = Physics.RaycastNonAlloc(MainCamera.ScreenPointToRay(Input.mousePosition), _hitResults, Mathf.Infinity, WhatIsInteractable);
        if (hitCount <= 0) return;
        if (!_hitResults[0].collider.TryGetComponent(out IInteractable interactable)) return;
        CurrentInteractable = interactable;
        CurrentInteractable.OnMouseCallback();
    }
    private void OnMouseUpCallback(Unit _)
    {
        EndInteractable = null;
        int hitCount = Physics.RaycastNonAlloc(MainCamera.ScreenPointToRay(Input.mousePosition), _hitResults, Mathf.Infinity, WhatIsInteractable);
        if (hitCount <= 0) return;
        if (!_hitResults[0].collider.TryGetComponent(out IInteractable interactable)) return;
        EndInteractable = interactable;
        EndInteractable.OnMouseUpCallback();

        if (EndInteractable == StartInteractable && EndInteractable != null)
        {
            EndInteractable.OnMouseClickCallback();
        }
        
        // EndInteractable = null;
        // StartInteractable = null;
        // CurrentInteractable = null;
    }
}
public interface IInteractable
{
    void OnMouseDownCallback();
    void OnMouseCallback();
    void OnMouseUpCallback();
    void OnMouseClickCallback();
}
