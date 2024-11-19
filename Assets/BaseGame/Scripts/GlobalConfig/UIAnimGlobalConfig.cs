using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "UIAnimGlobalConfig", menuName = "GlobalConfigs/UIAnimGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class UIAnimGlobalConfig : GlobalConfig<UIAnimGlobalConfig>
{
    public List<UIAnimData> uIAnimDatas = new();

    public UIAnimData GetAnimData(UIAnimType uIAnimType)
    {
        for (int i = 0; i < uIAnimDatas.Count; i++)
        {
            if (uIAnimDatas[i].animType == uIAnimType)
                return uIAnimDatas[i];
        }
        return null;
    }
}

public enum UIAnimType { 
    OpenPopup,
    ClosePopup,
    ButtonBasic
}

[System.Serializable]
public class UIAnimData {
    public UIAnimType animType;
    public AnimationCurve easeCurve;
    public float duration;
}
