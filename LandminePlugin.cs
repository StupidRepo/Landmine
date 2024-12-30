using System.Reflection;
using BepInEx;
using HarmonyLib;
using MyceliumNetworking;
using UnityEngine;
using Zorro.Core;
using Random = UnityEngine.Random;

namespace Landmine;

[BepInPlugin("stupidrepo-Landmine", "Landmine", "0.1")]
public class LandminePlugin : BaseUnityPlugin
{
	public static LandminePlugin Instance;
	
	private static AssetBundle OurAssetBundle;
	public static AssetBundleHandler Bundle;

	public const uint ModID = 95142;
	public const int SpawnCount = 30; // How many mines to spawn in a map.

	private void Awake()
	{
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		Debug.Log("Harmony patches loaded");
		
		Instance = this;
		
		// Load embedded asset bundle
		using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Landmine.AssetBundles.ab");
		OurAssetBundle = AssetBundle.LoadFromStream(stream);
		Bundle = new AssetBundleHandler(OurAssetBundle);
		
		// Register items
		RegisterItem(Bundle.GetAssetByName<Item>("SpawnableLandmineItem"));
		
		MyceliumNetwork.RegisterNetworkObject(this, ModID);
	}

	private void RegisterItem(Item? item)
	{
		if (item == null)
		{
			Debug.LogError("Item is null");
			return;
		}
		
		Debug.LogWarning("Adding item: " + item.displayName); 
			
		// item.purchasable = true;
		// item.spawnable = true;
		
		item.price = item.price > 0 ? item.price : 5;
		
		SingletonAsset<ItemDatabase>.Instance.AddRuntimeEntry(item);
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

		item.itemObject.AddComponent<T>();
		item.price = item.price > 0 ? item.price : 5;
		
		SingletonAsset<ItemDatabase>.Instance.AddRuntimeEntry(item);
	}

	public void DoRPC(Vector3 position)
	{
		MyceliumNetwork.RPC(ModID, nameof(DoLandmineSpawn), ReliableType.Reliable, position);
	}

	[CustomRPC]
	public void DoLandmineSpawn(Vector3 position)
	{
		PickupHandler.CreatePickup(
			Bundle.GetAssetByName<Item>("SpawnableLandmineItem")!.id,
			new ItemInstanceData(Guid.NewGuid()), 
			position,
			Quaternion.identity
		);
	}
}