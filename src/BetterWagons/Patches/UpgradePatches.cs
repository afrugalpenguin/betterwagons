using HarmonyLib;
using BetterWagons.Helpers;
using BetterWagons.Features;
using MelonLoader;

namespace BetterWagons.Patches
{
    /// <summary>
    /// Since building upgrades replace the GameObject entirely,
    /// our Start() postfixes on WagonShop already handle re-applying bonuses.
    /// This file handles Cart Depot promotion logic on load/upgrade.
    /// </summary>
    [HarmonyPatch(typeof(WagonShop))]
    public static class UpgradePatches
    {
        /// <summary>
        /// After WagonShop.Start, check if this is a max-tier shop that
        /// was previously promoted (e.g. on save load) and re-apply Tier 4 stats.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void Start_UpgradeCheck_Postfix(WagonShop __instance)
        {
            int tier = TierHelper.GetTier(__instance);
            MelonLogger.Msg($"[BetterWagons] WagonShop detected at Tier {tier}");

            // If already promoted to depot (from save), re-apply Tier 4 stats
            if (TierHelper.IsDepot(__instance))
            {
                WagonShopPatches.ApplyWorkerBonus(__instance);
                BuildingPatches.ApplyRadiusBonus(__instance);
                foreach (var wagon in __instance.registeredWagonsRO)
                {
                    wagon.CalculateCarryCapacity();
                }
                DepotStorage.EnableForShop(__instance);
                MelonLogger.Msg("[BetterWagons] Cart Depot stats re-applied on load");
            }
        }
    }
}
