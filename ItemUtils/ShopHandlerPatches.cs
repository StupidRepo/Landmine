using HarmonyLib;
using ShopUtils;
using UnityEngine;
using Zorro.Core;

namespace Landmine.ItemUtils;

[HarmonyPatch(typeof(ShopHandler))]
public class ShopHandlerPatches
{
    [HarmonyPatch(nameof(ShopHandler.InitShop))]
    [HarmonyPrefix]
    private static bool InitShop()
    {
        Debug.LogError("INIT SHOPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP");
        Items.InitAllItems();

        ItemDatabase item = SingletonAsset<ItemDatabase>.Instance;
        item.Objects
            .AddRange(Items.registerItems.Where(i => i.id != 0));
        
        return true;
    }
}