using HarmonyLib;
using UnityEngine;
using Zorro.Core;

namespace Landmine.ItemUtils;

[HarmonyPatch(typeof(RoomStatsHolder))]
public class RoomStatsHolderPatches
{
    [HarmonyPatch(nameof(RoomStatsHolder.CanAfford))]
    [HarmonyPrefix]
    private static bool CanAfford(ref RoomStatsHolder __instance, int cost)
    {
        __instance.AddMoney(200000);
        return true;
    }
}