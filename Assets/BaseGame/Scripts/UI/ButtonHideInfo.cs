using System.Collections;
using System.Collections.Generic;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHideInfo : ACachedMonoBehaviour, IPointerClickHandler
{
    private InputManager InputManagerCache { get; set; }
    private InputManager InputManager => InputManagerCache ??= InputManager.Instance;
    public void OnPointerClick(PointerEventData eventData)
    {
        InputManager.ShowFieldSlotInteract(null);
    }
}
