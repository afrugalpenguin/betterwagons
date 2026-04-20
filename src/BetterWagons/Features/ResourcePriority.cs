using System.Collections.Generic;

namespace BetterWagons.Features
{
    public enum ResourceCategory
    {
        General = 0,
        RawMaterials,
        FoodAgriculture,
        ManufacturedGoods,
    }

    public static class ResourcePriority
    {
        private static readonly Dictionary<int, ResourceCategory> _preferences = new Dictionary<int, ResourceCategory>();
        private static readonly Dictionary<ItemID, ResourceCategory> _itemCategories = new Dictionary<ItemID, ResourceCategory>();

        static ResourcePriority()
        {
            ItemID[] raw = {
                ItemID.Logs, ItemID.Stone, ItemID.IronOre, ItemID.GoldOre,
                ItemID.Clay, ItemID.Sand, ItemID.Coal, ItemID.Flax,
                ItemID.Willow, ItemID.Hide, ItemID.Hay, ItemID.Water,
                ItemID.Carcass, ItemID.BoarCarcass, ItemID.HealthyCarcass,
                ItemID.SmallCarcass, ItemID.WolfCarcass, ItemID.SicklyCarcass,
                ItemID.UnhealthyCarcass,
            };
            foreach (var id in raw) _itemCategories[id] = ResourceCategory.RawMaterials;

            ItemID[] food = {
                ItemID.Berries, ItemID.Bread, ItemID.Meat, ItemID.Fish,
                ItemID.Eggs, ItemID.Milk, ItemID.Cheese, ItemID.Grain,
                ItemID.Flour, ItemID.Greens, ItemID.Beans, ItemID.Fruit,
                ItemID.Nuts, ItemID.Mushroom, ItemID.Roots, ItemID.RootVegetable,
                ItemID.Herbs, ItemID.SmokedMeat, ItemID.SmokedFish,
                ItemID.PreservedVeg, ItemID.Preserves, ItemID.Honey,
                ItemID.Clover, ItemID.WheatBeer, ItemID.Spice, ItemID.Pastry,
            };
            foreach (var id in food) _itemCategories[id] = ResourceCategory.FoodAgriculture;

            ItemID[] manufactured = {
                ItemID.Planks, ItemID.Brick, ItemID.Iron, ItemID.GoldIngot,
                ItemID.Tool, ItemID.HeavyTool, ItemID.Furniture,
                ItemID.Weapon, ItemID.SimpleWeapon, ItemID.HeavyWeapon,
                ItemID.Shield, ItemID.Hauberk, ItemID.Platemail,
                ItemID.Arrow, ItemID.Bow, ItemID.Crossbow,
                ItemID.Shoes, ItemID.LinenClothes, ItemID.HideCoat,
                ItemID.Pottery, ItemID.Glass, ItemID.Basket, ItemID.Barrel,
                ItemID.Soap, ItemID.Candle, ItemID.Medicine,
                ItemID.Firewood, ItemID.Tallow, ItemID.Wax, ItemID.Compost,
                ItemID.AnimalTrap, ItemID.Books, ItemID.Paper, ItemID.KnowledgeTome,
            };
            foreach (var id in manufactured) _itemCategories[id] = ResourceCategory.ManufacturedGoods;
        }

        public static void SetPreference(WagonShop shop, ResourceCategory category)
        {
            if (shop == null) return;
            _preferences[shop.gameObject.GetInstanceID()] = category;
        }

        public static ResourceCategory GetPreference(WagonShop shop)
        {
            if (shop == null) return ResourceCategory.General;
            if (_preferences.TryGetValue(shop.gameObject.GetInstanceID(), out var cat))
                return cat;
            return ResourceCategory.General;
        }

        public static ResourceCategory GetCategoryForItem(ItemID itemId)
        {
            if (_itemCategories.TryGetValue(itemId, out var cat))
                return cat;
            return ResourceCategory.General;
        }

        public static bool ItemMatchesPreference(ItemID itemId, ResourceCategory preference)
        {
            if (preference == ResourceCategory.General) return true;
            return GetCategoryForItem(itemId) == preference;
        }

        public static void CyclePreference(WagonShop shop)
        {
            if (shop == null) return;
            var current = GetPreference(shop);
            var next = (ResourceCategory)(((int)current + 1) % 4);
            SetPreference(shop, next);
        }

        public static void Clear()
        {
            _preferences.Clear();
        }
    }
}
