using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlotHeroJob : SlotBase<Hero.Job>
{
    [Header("==== SlotHeroJob ====")]
    public TextMeshProUGUI txtJobName;
    public Image imgBG;
    public override void InitData(Hero.Job data)
    {
        base.InitData(data);
        txtJobName.text = data.ToString();
    }
}
