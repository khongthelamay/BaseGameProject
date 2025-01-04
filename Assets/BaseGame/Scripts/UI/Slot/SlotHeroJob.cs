using Core;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SlotHeroJob : SlotBase<Hero.Job>
{
    [Header("==== SlotHeroJob ====")]
    public TextMeshProUGUI txtJobName;
    [FormerlySerializedAs("imgBG")] public Image imgBg;
    public override void InitData(Hero.Job data)
    {
        base.InitData(data);
        txtJobName.text = data.ToString();
        //imgBg.sprite = 
    }
}
