using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Landmine.ItemUtils;

[HarmonyPatch(typeof(RoundArtifactSpawner))]
public class RoundArtifactSpawnerPatches
{
    [HarmonyPatch(nameof(RoundArtifactSpawner.CreateArtifactSpawners))]
    [HarmonyPostfix]
    public static void CreateOurLandmine(ref RoundArtifactSpawner __instance)
    {
        Debug.LogWarning("Creating landmines!");
        
        for (var i = 0; i < LandminePlugin.SpawnCount; i++)
        {
            List<PatrolPoint> pointsInGroups = Level.currentLevel.GetPointsInGroups(new List<PatrolPoint.PatrolGroup>
            {
                PatrolPoint.PatrolGroup.Bear, // tf are these names??
                PatrolPoint.PatrolGroup.Dog,
                PatrolPoint.PatrolGroup.Ant,
                PatrolPoint.PatrolGroup.Bird,
                PatrolPoint.PatrolGroup.Cat,
                PatrolPoint.PatrolGroup.Fish,
                PatrolPoint.PatrolGroup.Wolf
            });
            var lol = PickupHandler.CreatePickup(
                LandminePlugin.Bundle.GetAssetByName<Item>("SpawnableLandmineItem")!.id,
                new ItemInstanceData(Guid.NewGuid()), 
                pointsInGroups[Random.Range(0, pointsInGroups.Count)].transform.position,
                Random.rotation);
        }
    }
}