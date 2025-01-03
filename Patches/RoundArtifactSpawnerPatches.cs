using HarmonyLib;
using Landmines.Extensions;
using Photon.Pun;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Landmines.Patches;

[HarmonyPatch(typeof(RoundArtifactSpawner))]
public class RoundArtifactSpawnerPatches
{
	[HarmonyPatch(nameof(RoundArtifactSpawner.CreateArtifactSpawners))]
	[HarmonyPostfix]
	public static void SpawnLandminesPatch(ref RoundArtifactSpawner __instance)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		var overallSpawnCount = 0;
		NavMeshSurface.s_NavMeshSurfaces.ForEach(nmSurface =>
		{
			var bounds = nmSurface.navMeshData.sourceBounds;

			var minY = bounds.max.y;
			var maxY = bounds.min.y;

			if(!SceneManager.GetActiveScene().name.Contains("Factory")) DoSampling(ref bounds, ref minY, ref maxY);

			Debug.LogWarning($"About to spawn landmines - minY: {minY}, maxY: {maxY}");

			var possibleSpawnLocations = new List<Vector3>();

			for (var i = 0; i < LandminesPlugin.PossibleSpawnCount / NavMeshSurface.s_NavMeshSurfaces.Count; i++)
			{
				var randomPosition = nmSurface.GetRandomPosition(minY, maxY);
				if (randomPosition == Vector3.zero) continue;
				if (IsLocationOccupied(randomPosition, LandminesPlugin.SpawnRadius, possibleSpawnLocations)) continue;

				possibleSpawnLocations.Add(randomPosition);
			}

			foreach (var spawnLocation in possibleSpawnLocations)
			{
				// 50% chance for normal mine, 50% chance for impulse mine
				var photonName = Random.value < 0.5f ? PhotonLandmineTypes.Normal : PhotonLandmineTypes.Impulse;
				
				PhotonNetwork.Instantiate(photonName, spawnLocation, Quaternion.identity);
				overallSpawnCount++;
			}
		});

		Debug.LogWarning($"Spawned {overallSpawnCount} of {LandminesPlugin.PossibleSpawnCount} landmines");
	}

	private static void DoSampling(ref Bounds bounds, ref float minY, ref float maxY)
	{
		for (var i = 0; i < LandminesPlugin.SampleCount; i++)
		{
			var samplePosition = new Vector3(
				Random.Range(bounds.min.x, bounds.max.x),
				Random.Range(bounds.min.y, bounds.max.y),
				Random.Range(bounds.min.z, bounds.max.z)
			);

			if (!NavMesh.SamplePosition(samplePosition, out var hit, Mathf.Infinity, NavMesh.AllAreas)) continue;
            
			if (hit.position.y < minY) minY = hit.position.y;
			if (hit.position.y > maxY) maxY = hit.position.y;
		}
	}
	
	// check if the location is surrounded by other spawnLocations
	private static bool IsLocationOccupied(Vector3 spawnLocation, float radius, List<Vector3> possibleSpawnLocations)
	{
		return possibleSpawnLocations.Any(otherSpawnLocation => Vector3.Distance(spawnLocation, otherSpawnLocation) < radius);
	}
}