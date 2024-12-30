using System.Reflection;
using BepInEx;
using HarmonyLib;
using Landmine.Components;
using MyceliumNetworking;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Random = UnityEngine.Random;

namespace Landmine;

[BepInPlugin("stupidrepo-Landmine", "Landmine", "0.1")]
public class LandminePlugin : BaseUnityPlugin
{
	public static LandminePlugin Instance;
	
	private static AssetBundle OurAssetBundle;
	public static AssetBundleHandler Bundle;
	
	public Dictionary<string, Item> RegisteredItems = new();

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
		RegisterItem(Bundle.GetAssetByName<Item>("SpawnableLandmineItem"), "Landmine");
		// PUNPoolAddOurRegisteredItems();
		
		// MyceliumNetwork.LobbyCreated += PUNPoolAddOurRegisteredItems;
		// MyceliumNetwork.LobbyEntered += PUNPoolAddOurRegisteredItems;
	}

	private void RegisterItem(Item? item, string photonName, bool addToDB = false)
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
		
		if(addToDB)
			SingletonAsset<ItemDatabase>.Instance.AddRuntimeEntry(item);
		
		RegisteredItems.Add(photonName, item);
	}

	void RegisterItem<T>(Item? item, string photonName, bool addToDB = false) where T : Component
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

		if (addToDB)
			SingletonAsset<ItemDatabase>.Instance.AddRuntimeEntry(item);
		
		RegisteredItems.Add(photonName, item);
	}
	
	public void PUNPoolAddOurRegisteredItems()
	{
		foreach (var item in RegisteredItems)
		{
			((DefaultPool)PhotonNetwork.prefabPool).ResourceCache.Add(item.Key, item.Value.itemObject);
			Debug.LogWarning("Registering item with PUN pool: " + item.Value.displayName);
		}
	}
	
	[ConsoleCommand]
	public static void SpawnLandmine()
	{
		// spawn landmine a little bit in front of player
		var player = Player.localPlayer;
		if (player == null)
		{
			Debug.LogError("Player is null");
			return;
		}
		
		if(!PhotonNetwork.IsMasterClient)
			return;

		var go = PhotonNetwork.Instantiate("Landmine", MainCamera.instance.GetDebugItemSpawnPos(), Quaternion.identity);
	}
}

