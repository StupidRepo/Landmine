using HarmonyLib;
using ShopUtils;

namespace Landmine.ItemUtils
{
    [HarmonyPatch(typeof(RoundArtifactSpawner))]
    internal static class RoundArtifactSpawnerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(RoundArtifactSpawner.SpawnRound))]
        private static void SpawnRound(RoundArtifactSpawner __instance)
        {
            __instance.possibleSpawns = __instance.possibleSpawns
                .AddRangeToArray(Items.registerItems.Where(i => i.id != 0 && i.spawnable).ToList().ToArray());
        }
    }
}
