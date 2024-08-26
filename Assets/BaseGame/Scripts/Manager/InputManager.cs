using R3;
using R3.Triggers;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{
    [field: SerializeField] public Camera MainCamera {get; private set;}

    [field: SerializeField] public LayerMask WhatIsInteractable {get; private set;}

    private IInteractable StartInteractable { get; set; }
    private IInteractable CurrentInteractable { get; set; }
    private IInteractable EndInteractable { get; set; }
    private bool IsTouchingUI { get; set; }
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
        
        if (IsPointerOverGameObject()) return;
        if (EndInteractable == StartInteractable && EndInteractable != null)
        {
            EndInteractable.OnMouseClickCallback();
        }
        
        // EndInteractable = null;
        // StartInteractable = null;
        // CurrentInteractable = null;
    }

    private bool IsPointerOverGameObject()
    {
        //check mouse
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        //check touch
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
            {
                IsTouchingUI = true;
                return true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && IsTouchingUI)
        {
            IsTouchingUI = false;
            return true;
        }

        return IsTouchingUI;
    }
}
public interface IInteractable
{
    void OnMouseDownCallback();
    void OnMouseCallback();
    void OnMouseUpCallback();
    void OnMouseClickCallback();
}
