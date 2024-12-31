using HarmonyLib;

namespace Landmines.Patches;

[HarmonyPatch(typeof(RoomStatsHolder))]
public class RoomStatsHolderPatches
{
    #if DEBUG
        [HarmonyPatch(nameof(RoomStatsHolder.CanAfford))]
        [HarmonyPrefix]
        private static bool CanAfford(ref RoomStatsHolder __instance, int cost)
        {
            __instance.AddMoney(500);
            return true;
        }
    #endif
}