using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
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
            List<PatrolPoint> pointsInGroups = Level.currentLevel.patrolGroups.SelectMany(x => x.Value).ToList();
            PhotonNetwork.Instantiate("Landmine", pointsInGroups.GetRandom().transform.position, Quaternion.identity);
        }
    }
}