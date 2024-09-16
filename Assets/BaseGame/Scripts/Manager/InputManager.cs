using R3;
using R3.Triggers;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{
    [field: SerializeField] public Camera MainCamera {get; private set;}

    [field: SerializeField] public LayerMask WhatIsInteractable {get; private set;}
    
    private bool IsTouchingUI { get; set; }
    private IInteractable CurrentSelectedInteractable { get; set; }

    public void Select(IInteractable interactable)
    {
        
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

public interface IInteractable : IPointerClickHandler
{

}

