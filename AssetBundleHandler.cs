using UnityEngine;
using Object = UnityEngine.Object;

namespace Landmines;

public class AssetBundleHandler
{
	private readonly Dictionary<string, Object> loadedAssets;

	public AssetBundleHandler(AssetBundle bundle)
	{
		loadedAssets = new Dictionary<string, Object>();

		foreach (var assetName in bundle.GetAllAssetNames())
		{
			var objectName = Path.GetFileNameWithoutExtension(assetName.ToLower());
			loadedAssets[objectName] = bundle.LoadAsset(assetName);
			
			Debug.Log($"Loaded asset: {objectName}");
		}
	}

	public T? GetAssetByName<T>(string name) where T : Object
	{
		if (loadedAssets.TryGetValue(name.ToLower(), out var asset))
		{
			return asset as T;
		}
		Debug.LogWarning($"Asset with name {name} not found in the asset bundle.");
		return null;
	}
}