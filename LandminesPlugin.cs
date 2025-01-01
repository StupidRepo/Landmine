using System.Reflection;
using INeedWorkshopDeps.Attributes;
using Itemz;
using Photon.Pun;
using UnityEngine;
using Zorro.Core.CLI;

namespace Landmines;

[ContentWarningPlugin("stupidrepo.Landmines", "0.2", false)]
[ContentWarningDependency(3384690922)] // MyceliumNetworking
[ContentWarningDependency(3397332899)] // Itemz
public static class LandminesPlugin
{
	public static readonly AssetBundleHandler Bundle;

	public const uint ModID = 534233;
	
	public const int SpawnCount = 300;
	public const int SampleCount = 50;
	
	public const int SpawnRadius = 9;

	static LandminesPlugin()
	{
		using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Landmines.AssetBundles.ab");
		var ourAssetBundle = AssetBundle.LoadFromStream(stream);
		Bundle = new AssetBundleHandler(ourAssetBundle);
		
		Itemz.Itemz.RegisterItem(Bundle.GetAssetByName<Item>("SpawnableLandmineItem"), "Landmine", false);
	}
	
	[ConsoleCommand]
	public static void SpawnLandmine()
	{
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

