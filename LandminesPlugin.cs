using System.Reflection;
using INeedWorkshopDeps.Attributes;
using Photon.Pun;
using UnityEngine;
using Zorro.Core.CLI;

namespace Landmines;

[ContentWarningPlugin("stupidrepo.Landmines", "0.1", false)]
[ContentWarningDependency(3384690922)]
public static class LandminePlugin
{
	public static readonly AssetBundleHandler Bundle;

	public const uint ModID = 534233;
	public const int SpawnCount = 45;

	static LandminePlugin()
	{
		using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Landmines.AssetBundles.ab");
		var ourAssetBundle = AssetBundle.LoadFromStream(stream);
		Bundle = new AssetBundleHandler(ourAssetBundle);
		
		// Register items
		Itemz.Itemz.RegisterItem(Bundle.GetAssetByName<Item>("SpawnableLandmineItem"), "Landmine");
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

