using HarmonyLib;

namespace Landmine.ItemUtils
{
    // [HarmonyPatch(typeof(ItemInstanceData))]
    // internal static class ItemInstanceDataPatch
    // {
    //     internal static int EntryCount;
    //
    //     [HarmonyPatch(nameof(ItemInstanceData.GetEntryIdentifier))]
    //     [HarmonyPrefix]
    //     private static bool GetEntryIdentifier(ref byte __result, Type type)
    //     {
    //         if (!Entries.registerEntries.Contains(type)) {
    //             return true;
    //         }
    //
    //         int begin = EntryCount;
    //         foreach (Type type1 in Entries.registerEntries)
    //         {
    //             if (begin == byte.MaxValue)
    //             {
    //                 throw new ShopUtilsException("Item Instance Data Out of range > 255");
    //             }
    //
    //             begin += 1;
    //             if (type1 == type)
    //             {
    //                 __result = (byte)begin;
    //                 return false;
    //             }
    //         }
    //
    //         return true;
    //     }
    //
    //     [HarmonyPatch(nameof(ItemInstanceData.GetEntryType))]
    //     [HarmonyPrefix]
    //     private static bool GetEntryType(ref ItemDataEntry __result, byte identifier)
    //     {
    //         int begin = EntryCount;
    //         foreach (Type type1 in Entries.registerEntries)
    //         {
    //             begin += 1;
    //             if (identifier == (byte) begin)
    //             {
    //                 __result = (ItemDataEntry) type1.GetConstructor(new Type[0]).Invoke(null);
    //                 return false;
    //             }
    //         }
    //
    //         return true;
    //     }
    // }
}
