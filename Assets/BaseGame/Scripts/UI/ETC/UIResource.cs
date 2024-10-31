using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TW.Utility.CustomType;
using TW.Reactive.CustomComponent;

public class UIResource : MonoBehaviour
{
    public ResourceType resourceType;
    public Image imgIcon;
    public TextMeshProUGUI txtResource;

    public void SetResourceType(ResourceType resourceType) { 
        this.resourceType = resourceType;
        //imgIcon.sprite = ?;
    }

    public void ChangeValue(string value) { txtResource.text = value; }
}
