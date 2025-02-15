﻿using System.Reflection;
using Mono.Cecil;
using MyceliumNetworking;
using MyceliumObjects.Components;
using Photon.Pun;
using Steamworks;
using UnityEngine;
using Zorro.Core.CLI;
using Zorro.Settings;
using Object = UnityEngine.Object;

namespace Landmines;

[ContentWarningPlugin("stupidrepo.Landmines", "0.6.1", false)]
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
		
		var gameObject = new GameObject("LandminesPluginLol")
		{
			hideFlags = HideFlags.HideAndDontSave
		};

		Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<Components.TheActualPluginBecauseThingyModLoaderSucks>();
		
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

		Instantiator.Instance.InstantiateObject(photonName, ModID, MainCamera.instance.GetDebugItemSpawnPos());
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