using HarmonyLib;
using MyceliumNetworking;
using Photon.Pun;
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
        if (!PhotonNetwork.IsMasterClient) return; // only the master client should spawn, we'll RPC to the clients
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
            LandminePlugin.Instance.DoRPC(pointsInGroups[Random.Range(0, pointsInGroups.Count)].transform.position);
        }
    }
}