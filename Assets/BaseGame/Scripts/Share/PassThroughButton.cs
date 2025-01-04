using UnityEngine;
using UnityEngine.EventSystems;

public class PassThroughButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Button clicked!");
        
        ExecuteEvents.ExecuteHierarchy(
            eventData.pointerPressRaycast.gameObject,
            eventData,
            ExecuteEvents.pointerClickHandler
        );
    }
}