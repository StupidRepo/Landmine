using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Landmine.ItemUtils;

[HarmonyPatch(typeof(SteamLobbyHandler))]
public class SteamLobbyHandlerPatches
{
    [HarmonyPatch(nameof(SteamLobbyHandler.OnLobbyEnterCallback))]
    [HarmonyPostfix]
    public static void RegisterPunPrefabOnEnterPatch()
    {
        Debug.LogError("ON LOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" +
                       "OOOOOOOOOOOOOENTER");
        LandminePlugin.Instance.PUNPoolAddOurRegisteredItems();
    }
}