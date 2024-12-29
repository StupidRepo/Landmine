using System.Reflection;
using BepInEx;
using HarmonyLib;
using Landmine.Components;
using UnityEngine;
using Zorro.Core;

namespace Landmine;

[BepInPlugin("stupidrepo-Landmine", "Landmine", "0.1")]
public class LandminePlugin : BaseUnityPlugin
{
	public static AssetBundle OurAssetBundle;
	public static AssetBundleHandler Bundle;
	
	void Awake()
	{
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		Debug.Log("Harmony patches loaded");
		
		// Load embedded asset bundle
		using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Landmine.AssetBundles.ab");
		OurAssetBundle = AssetBundle.LoadFromStream(stream);
		Bundle = new AssetBundleHandler(OurAssetBundle);
		
		// Register items
		RegisterItem<Dart>(Bundle.GetAssetByName<Item>("NerfDart"));
	}

	void RegisterItem<T>(Item? item) where T : Component
	{
		if (item == null)
		{
			Debug.LogError("Item is null");
			return;
		}
		
		Debug.LogWarning("Adding item: " + item.displayName); 
			
		item.purchasable = true;
		item.spawnable = true;

		item.itemObject.AddComponent<T>();
			
		item.price = item.price > 0 ? item.price : 5;
			
		SingletonAsset<ItemDatabase>.Instance.AddRuntimeEntry(item);
	}
}