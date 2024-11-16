using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
