using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;

namespace BetterWagons.Features
{
    /// <summary>
    /// Expands the WagonShop's storage carryCapacity when promoted to Cart Depot (Tier 4).
    /// The game's Resource base class exposes `storage` as a ReservableItemStorage wrapping
    /// an ItemStorage whose carryCapacity governs how much weight can be stored on-site.
    /// We reach the inner ItemStorage via reflection and add a configured bonus.
    /// </summary>
    public static class DepotStorage
    {
        // Track how much we've added so we can undo on repeat calls
        private static readonly Dictionary<int, float> _appliedBonus = new Dictionary<int, float>();

        public static void EnableForShop(WagonShop shop)
        {
            if (shop == null) return;
            if (shop.storage == null)
            {
                MelonLogger.Warning("[BetterWagons] Cart Depot: WagonShop has no storage — storage bonus skipped");
                return;
            }

            var itemStorage = Traverse.Create(shop.storage).Field("itemStorage").GetValue<ItemStorage>();
            if (itemStorage == null)
            {
                MelonLogger.Warning("[BetterWagons] Cart Depot: could not resolve inner ItemStorage — storage bonus skipped");
                return;
            }

            int id = shop.gameObject.GetInstanceID();
            float desiredBonus = ModConfig.Tier4StorageCapacity.Value;
            _appliedBonus.TryGetValue(id, out float alreadyApplied);
            float delta = desiredBonus - alreadyApplied;
            if (delta == 0f) return;

            itemStorage.carryCapacity += delta;
            _appliedBonus[id] = desiredBonus;

            MelonLogger.Msg($"[BetterWagons] Cart Depot storage expanded by {delta} (new total: {itemStorage.carryCapacity})");
        }

        public static void Clear() => _appliedBonus.Clear();
    }
}
