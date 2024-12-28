using UnityEngine;
using Sirenix.Utilities;
using TW.Utility.CustomType;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TW.Utility.Extension;

[CreateAssetMenu(fileName = "ArtifactGlobalConfig", menuName = "GlobalConfigs/ArtifactGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class ArtifactGlobalConfig : GlobalConfig<ArtifactGlobalConfig>
{
    public List<ArtifactDataConfig> artifactDataConfigs = new();
//1-HkinUwSW4A4SkuiLGtl0Tm8771jFPVZB5ZpLs5pxz4
    public ArtifactDataConfig GetArtifactDataConfig(int id)
    {
        return artifactDataConfigs.Find(e => e.id == id);
    }
    
#if UNITY_EDITOR
    string linkSheetId = "1-HkinUwSW4A4SkuiLGtl0Tm8771jFPVZB5ZpLs5pxz4";
    string requestedData;

    [Button]
    public void FetchQuestData()
    {
        if (string.IsNullOrEmpty(linkSheetId)) return;
        FetchAchievement();
    }
    
    async void FetchAchievement()
    {
        requestedData = await ABakingSheet.GetCsv(linkSheetId, "Artifact");
        artifactDataConfigs.Clear();
        List<Dictionary<string, string>> data = ACsvReader.ReadDataFromString(requestedData);
        for (int i = 0; i < data.Count; i++)
        {
            int id = int.Parse((data[i]["ID"]));
            if (string.IsNullOrEmpty(data[i]["Description"]))
                return;
            string des = data[i]["Description"];
            float baseValue = float.Parse(data[i]["BaseValue"]);
            float increaseValue = float.Parse(data[i]["IncreaseValue"]);
            ArtifactDataConfig newArtifactDataConfig = new();
            newArtifactDataConfig.id = id;
            newArtifactDataConfig.strDes = des;
            newArtifactDataConfig.priceUpgrade = new();
            newArtifactDataConfig.piecesRequire = new();
            newArtifactDataConfig.baseValue = baseValue;
            newArtifactDataConfig.increaseValue = increaseValue;
            artifactDataConfigs.Add(newArtifactDataConfig);
        }
    }
#endif
}

[System.Serializable]
public class ArtifactDataConfig
{
    public int id;
    public string strDes;
    public string strFunDes;
    public List<BigNumber> priceUpgrade = new();
    public List<int> piecesRequire = new();
    public float baseValue;
    public float increaseValue;
}