using BetterWagons.Helpers;
using BetterWagons.Patches;
using MelonLoader;

namespace BetterWagons.Features
{
    public static class DepotPromotion
    {
        public static bool TryPromote(WagonShop shop)
        {
            if (!TierHelper.CanPromoteToDepot(shop))
            {
                MelonLogger.Msg("[BetterWagons] Cannot promote — not at Tier 3 or already promoted");
                return false;
            }

            TierHelper.PromoteToDepot(shop);

            // Re-apply all tier-based bonuses at new tier
            WagonShopPatches.ApplyWorkerBonus(shop);
            BuildingPatches.ApplyRadiusBonus(shop);

            // Recalc capacity for all wagons
            foreach (var wagon in shop.registeredWagonsRO)
            {
                wagon.CalculateCarryCapacity();
            }

            // Expand on-site storage
            DepotStorage.EnableForShop(shop);

            MelonLogger.Msg("[BetterWagons] Successfully promoted to Cart Depot!");
            return true;
        }
    }
}
