using System.Reflection;
using INeedWorkshopDeps.Attributes;
using Itemz;
using Photon.Pun;
using Steamworks;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core.CLI;
using Zorro.Settings;
using SteamTimeline = Steamworks.SteamTimeline;

namespace Landmines;

[ContentWarningPlugin("stupidrepo.Landmines", "0.5", false)]
[ContentWarningDependency(3384690922)] // MyceliumNetworking
[ContentWarningDependency(3397332899)] // Itemz
public static class LandminesPlugin
{
	public static readonly AssetBundleHandler Bundle;

	public const uint ModID = 534233;
	
	public const int PossibleSpawnCount = 250;
	public const int SampleCount = 50;
	
	public const int SpawnRadius = 9;

	public static int SecToRecordBeforeStep = 5;
	public static int SecToRecordAfterStep = 5;

	static LandminesPlugin()
	{
		using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Landmines.lminemod");
		Bundle = new AssetBundleHandler(AssetBundle.LoadFromStream(stream));
		
		Itemz.Itemz.RegisterItem(Bundle.GetAssetByName<Item>("NormalMineItem"), PhotonLandmineTypes.Normal, false);
		Itemz.Itemz.RegisterItem(Bundle.GetAssetByName<Item>("ImpulseMineItem"), PhotonLandmineTypes.Impulse, false);
		
		Callback<LobbyEnter_t>.Create(OnLobbyEnter);
	}

	[ConsoleCommand]
	public static void TrySpawn(string photonName)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("You are not the master client!");
			return;
		}

		var player = Player.localPlayer;
		if (player == null)
		{
			Debug.LogError("You are not in a game!");
			return;
		}

		PhotonNetwork.Instantiate(photonName, MainCamera.instance.GetDebugItemSpawnPos(), Quaternion.identity);
	}

	private static void OnLobbyEnter(LobbyEnter_t callback)
	{
		// Debug.LogWarning("LOBBY ENTERRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRED");
		// SteamTimeline.SetTimelineGameMode(ETimelineGameMode.k_ETimelineGameMode_Playing);
	}
}

[ContentWarningSetting]
public class SecToRecordBeforeStepSetting: IntSetting, IExposedSetting {
	public override void ApplyValue() => LandminesPlugin.SecToRecordBeforeStep = Value;

	public override int GetDefaultValue() => 5;

	// Prefer using the Mods category
	public SettingCategory GetSettingCategory() => SettingCategory.Mods;

	public string GetDisplayName() => "[Landmines] Seconds to record before stepping on a landmine";
}

[ContentWarningSetting]
public class SecToRecordAfterStepSetting: IntSetting, IExposedSetting {
	public override void ApplyValue() => LandminesPlugin.SecToRecordAfterStep = Value;

	public override int GetDefaultValue() => 5;

	// Prefer using the Mods category
	public SettingCategory GetSettingCategory() => SettingCategory.Mods;

	public string GetDisplayName() => "[Landmines] Seconds to record after stepping off a landmine";
}