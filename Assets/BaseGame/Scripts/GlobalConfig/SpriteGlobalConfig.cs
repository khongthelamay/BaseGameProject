using Core;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteGlobalConfig", menuName = "GlobalConfigs/SpriteGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class SpriteGlobalConfig : GlobalConfig<SpriteGlobalConfig>
{
    public List<Sprite> sprFramesRarity = new();

    public Sprite GetFrameSprite(Hero.Rarity rarity) { 
        for (int i = 0; i < sprFramesRarity.Count; i++)
        {
            if (sprFramesRarity[i].name == "Frame_" + rarity.ToString())
                return sprFramesRarity[i];
        }
        return null;
    }
}
