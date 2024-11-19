using UnityEngine;
using Sirenix.Utilities;
using TW.Utility.CustomType;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ArtifactGlobalConfig", menuName = "GlobalConfigs/ArtifactGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class ArtifactGlobalConfig : GlobalConfig<ArtifactGlobalConfig>
{
    public List<ArtifactDataConfig> artifactDataConfigs = new();

    public ArtifactDataConfig GetArtifactDataConfig(ArtifactType artifactType)
    {
        Debug.Log(artifactType);
        return artifactDataConfigs.Find(e => e.artifactType == artifactType);
    }
}

public enum ArtifactType { 
    None = 0,
    PowerPotion = 1,
    FairyBow = 2,
    PunchingGlove = 3,
    Greatsword = 4,
    ArcaneTome = 5,
    Snail = 6,
    Bat = 7,
    WizardsHat = 8,
    Wallet = 9,
    CastleWall = 10
}

[System.Serializable]
public class ArtifactDataConfig {
    public ArtifactType artifactType;
    public string strName;
    public string strDes;
    public string strFunDes;
    public List<BigNumber> priceUpgrade = new();
    public List<int> piecesRequire = new();
}