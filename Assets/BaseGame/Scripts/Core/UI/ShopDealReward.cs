using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopDealReward : MonoBehaviour
{
    public Image imgBG;
    public Image imgIcon;
    public TextMeshProUGUI txtReward;
    public Button btnChoose;

    private void Awake()
    {
        btnChoose.onClick.AddListener(OnChoose);
    }

    void OnChoose() { 
    
    }

    public void InitData(Resource resource)
    {
        txtReward.text = resource.value.Value.ToString();
    }

    public void InitData(int heroID) 
    { 
    
    }
}
