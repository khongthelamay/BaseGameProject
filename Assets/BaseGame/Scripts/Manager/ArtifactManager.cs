using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

public class ArtifactManager : Singleton<ArtifactManager>
{
    public ReactiveValue<ArtifactDataConfig> currentArtifactChoose;

    public void ChooseArtifact(ArtifactDataConfig artifactDataConfig) { currentArtifactChoose.Value = artifactDataConfig; }

    public ArtifactDataConfig GetArtifactDataConfig(ArtifactType artifactType) {
        return ArtifactGlobalConfig.Instance.GetArtifactDataConfig(artifactType);
    }
}
