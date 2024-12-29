using System.Reflection;
using BepInEx;
using BepInEx.Unity.Mono;
using HarmonyLib;
using Landmine.Components;
using UnityEngine;
using Zorro.Core;

namespace Landmine;

[BepInPlugin("stupidrepo-Landmine", "Landmine", "0.1")]
public class LandminePlugin : BaseUnityPlugin
{
	private static AssetBundle OurAssetBundle;
	public static AssetBundleHandler Bundle;

	private void Awake()
	{
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		Debug.Log("Harmony patches loaded");
		
		// Load embedded asset bundle
		using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Landmine.AssetBundles.ab");
		OurAssetBundle = AssetBundle.LoadFromStream(stream);
		Bundle = new AssetBundleHandler(OurAssetBundle);
		
		// Register items
		RegisterItem<BuyableLandmineItem>(Bundle.GetAssetByName<Item>("BuyableLandmineItem"));
		RegisterItem<ThisIsALandmine>(Bundle.GetAssetByName<Item>("SpawnableLandmineItem"));
	}

	void RegisterItem<T>(Item? item) where T : Component
	{
		if (item == null)
		{
			Debug.LogError("Item is null");
			return;
		}
		
		Debug.LogWarning("Adding item: " + item.displayName); 
			
		// item.purchasable = true;
		// item.spawnable = true;

		// item.itemObject.AddComponent<T>();
		item.price = item.price > 0 ? item.price : 5;
		
		SingletonAsset<ItemDatabase>.Instance.AddRuntimeEntry(item);
	}
}