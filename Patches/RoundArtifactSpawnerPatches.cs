using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Landmines.Patches;

[HarmonyPatch(typeof(RoundArtifactSpawner))]
public class RoundArtifactSpawnerPatches
{
	[HarmonyPatch("CreateArtifactSpawners")]
	[HarmonyPostfix]
	public static void SpawnLandminesPatch(ref RoundArtifactSpawner __instance)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		for (var i = 0; i < LandminePlugin.SpawnCount; i++)
		{
			List<PatrolPoint> pointsInGroups = Level.currentLevel.patrolGroups.SelectMany(x => x.Value).ToList();
			PhotonNetwork.Instantiate("Landmine", pointsInGroups[Random.Range(0, pointsInGroups.Count)].transform.position, Quaternion.identity);
		}
	}
}