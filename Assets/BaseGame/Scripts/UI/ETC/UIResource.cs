using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using R3;

public class UIResource : MonoBehaviour
{
    public ResourceType resourceType;
    public Image imgIcon;
    public TextMeshProUGUI txtResource;

    public Resource resourceValue;

    public void SetResourceType(ResourceType resourceType) { 
        this.resourceType = resourceType;
        //imgIcon.sprite = ?;
        resourceValue = PlayerResourceManager.Instance.GetResource(resourceType);
        resourceValue.value.ReactiveProperty.Subscribe(ChangeValue).AddTo(this);
    }

    public void ChangeValue(BigNumber value) { txtResource.text = value.ToStringUIFloor(); }
}
