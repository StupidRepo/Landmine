using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;
using Logger = BepInEx.Logging.Logger;

namespace ShopUtils
{
    public static class Items
    {
        internal static List<Item> registerItems = new List<Item>();

        ///<summary>
        ///Add Shop Item
        /// </summary>
        public static void RegisterShopItem(Item item, ShopItemCategory category = ShopItemCategory.Invalid, int price = -1)
        {
            if (item == null) {
                throw new Exception("Item is null");
            }

            if (price >= 0)
            {
                item.price = price;
            }

            if (category != ShopItemCategory.Invalid)
            {
                item.Category = category;
            }

            if (item.Category == ShopItemCategory.Invalid)
            {
                throw new Exception(
                    $"Error Category Invalid \n" +
                    $"Items Name: {item.displayName} \n"
                    );
            }

            if (item.price < 0)
            {
                throw new Exception(
                    $"Error Item Price \n" +
                    $"Items Name: {item.displayName} \n"
                    );
            }

            item.purchasable = true;

            RegisterItem(item);
        }

        ///<summary>
        ///Add Spawnable Item
        /// </summary>
        public static void RegisterSpawnableItem(Item item, float Rarity = 1, int BudgetCost = 1)
        {
            if (item == null) {
                throw new Exception("Item is null");
            }

            // Artifact
            item.rarity = Rarity;
            item.budgetCost = BudgetCost;

            item.spawnable = true;

            RegisterItem(item);
        }

        ///<summary>
        ///Remove Spawnable Item
        /// </summary>
        public static void UnRegisterItem(Item item)
        {
            if (registerItems.Contains(item)) {
                registerItems.Remove(item);
            }
        }

        internal static void RegisterItem(Item item)
        {
            if (!registerItems.Contains(item)) {
                registerItems.Add(item);
            }
        }

        internal static void InitAllItems()
        {
            registerItems.ForEach(item => item.id = 0);

            foreach (Item item in registerItems)
            {
                if (TryGetMaxItemID(out byte id))
                {
                    item.id = id;

                    Debug.Log(
                        "[" +
                        $"Item: {item.displayName}, " +
                        $"ItemId: {item.id}, " +
                        $"Guid: {item.persistentID}" +
                        "]"
                    );

                    continue;
                }

                Debug.LogError("Max Item Id Out of range > 255");
                return;
            }
        }

        internal static bool TryGetMaxItemID(out byte itemId)
        {
            int id = 0;

            foreach (Item item in SingletonAsset<ItemDatabase>.Instance.Objects)
            {
                id = Math.Max(item.id, id);
            }

            foreach (Item item in registerItems)
            {
                id = Math.Max(item.id, id);
            }

            if (id == byte.MaxValue) {
                itemId = 0;
                return false;
            }

            itemId = (byte) (id + 1);
            return true;
        }
    }
}
