using System.Collections.Generic;

namespace BetterWagons.Helpers
{
    public static class TierHelper
    {
        // WagonShops that have been promoted to Cart Depot (Tier 4)
        private static readonly HashSet<int> _promotedToDepot = new HashSet<int>();

        /// <summary>
        /// Returns tier 1-4. Uses Building.tier directly for vanilla tiers.
        /// Tier 4 is our synthetic "Cart Depot" state layered on top of a
        /// max-tier vanilla WagonShop (Building.tier remains its vanilla value).
        /// </summary>
        public static int GetTier(WagonShop shop)
        {
            if (shop == null) return 1;
            if (_promotedToDepot.Contains(shop.gameObject.GetInstanceID())) return 4;

            Building building = shop;
            return building != null ? building.tier : 1;
        }

        /// <summary>
        /// True when the shop is at max vanilla tier (no further upgrade) and has not yet been promoted.
        /// Max vanilla tier is detected by the absence of an upgrade prefab on buildingUpgradeInfo.
        /// </summary>
        public static bool CanPromoteToDepot(WagonShop shop)
        {
            if (shop == null) return false;
            if (_promotedToDepot.Contains(shop.gameObject.GetInstanceID())) return false;

            Building building = shop;
            if (building == null) return false;

            var upgradeInfo = building.buildingUpgradeInfo;
            if (upgradeInfo == null) return true;

            var req = upgradeInfo.upgradeRequirement;
            if (req == null) return true;
            if (req.upgradedPrefab == null) return true;

            return false;
        }

        public static void PromoteToDepot(WagonShop shop)
        {
            if (!CanPromoteToDepot(shop)) return;
            _promotedToDepot.Add(shop.gameObject.GetInstanceID());
            MelonLoader.MelonLogger.Msg("[BetterWagons] WagonShop promoted to Cart Depot (Tier 4)");
        }

        public static bool IsDepot(WagonShop shop)
        {
            if (shop == null) return false;
            return _promotedToDepot.Contains(shop.gameObject.GetInstanceID());
        }

        public static void Clear() => _promotedToDepot.Clear();
    }
}
