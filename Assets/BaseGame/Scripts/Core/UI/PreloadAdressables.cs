using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PreloadAdressables : MonoBehaviour
{
    [SerializeField] private List<AssetReference> addressableReferences;

    private Dictionary<string, GameObject> preloadedAssets = new Dictionary<string, GameObject>();

    private void Start()
    {
        PreloadAssets();
    }

    private void PreloadAssets()
    {
        foreach (var assetReference in addressableReferences)
        {
            assetReference.LoadAssetAsync<GameObject>().Completed += OnAssetLoaded;
        }
    }

    private void OnAssetLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Load success: " + obj.Result.name);
            GameObject asset = obj.Result;
            preloadedAssets[asset.name] = asset;
            //DontDestroyOnLoad(asset);
        }
        else
        {
            Debug.LogError("Failed to load asset: " + obj.OperationException);
        }
    }

    public GameObject GetPreloadedAsset(string assetName)
    {
        if (preloadedAssets.TryGetValue(assetName, out GameObject asset))
        {
            return asset;
        }
        else
        {
            Debug.LogError("Asset not preloaded: " + assetName);
            return null;
        }
    }

    public void ShowAsset(string assetName)
    {
        GameObject asset = GetPreloadedAsset(assetName);
        if (asset != null)
        {
            asset.SetActive(true);
        }
    }

    public void HideAsset(string assetName)
    {
        GameObject asset = GetPreloadedAsset(assetName);
        if (asset != null)
        {
            asset.SetActive(false);
        }
    }
}
