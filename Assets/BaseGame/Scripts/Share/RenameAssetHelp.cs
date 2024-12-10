using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RenameAssetHelp : ACachedMonoBehaviour
{
    [field: SerializeField] public Texture[] Asset {get; private set;}
    [field: SerializeField] public string Current {get; private set;}
    [field: SerializeField] public string NewName {get; private set;}

#if UNITY_EDITOR
    [Button]
    public void RenameAsset()
    {
        for (int i = 0; i < Asset.Length; i++)
        {
            // rename asset change current name to new name
            if (Asset[i].name.Contains(Current))
            {
                string path = AssetDatabase.GetAssetPath(Asset[i]);
                string newName = Asset[i].name.Replace(Current, NewName);
                AssetDatabase.RenameAsset(path, newName);
            }
        }
    }

    public int Increase(int a)
    {
        if (a >= 10) return 0;
        return Increase(a + 1);
    }
#endif
}