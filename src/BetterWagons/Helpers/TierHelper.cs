using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using BetterWagons.Features;
using BetterWagons.Patches;

namespace BetterWagons.Helpers
{
    /// <summary>
    /// Mod-side tier system for WagonShops. Vanilla FF caps WagonShops at Tier 1,
    /// so we track tiers 1-4 ourselves keyed on GameObject instance ID.
    /// Tier 4 is the "Cart Depot" promotion.
    /// Per-session (resets on game reload — persistence TBD).
    /// </summary>
    public static class TierHelper
    {
        public const int MinTier = 1;
        public const int MaxTier = 4;

        private static readonly Dictionary<int, int> _modTier = new Dictionary<int, int>();

        public static int GetTier(WagonShop shop)
        {
            if (shop == null) return MinTier;
            int id = shop.gameObject.GetInstanceID();
            return _modTier.TryGetValue(id, out int t) ? t : MinTier;
        }

        public static bool IsDepot(WagonShop shop) => GetTier(shop) == MaxTier;

        public static bool CanUpgrade(WagonShop shop) => GetTier(shop) < MaxTier;

        /// <summary>Bumps the mod tier by 1 (up to MaxTier) and reapplies all bonuses. Returns new tier, or 0 if at cap.</summary>
        public static int TryUpgrade(WagonShop shop)
        {
            if (shop == null) return 0;
            int current = GetTier(shop);
            if (current >= MaxTier) return 0;
            int next = current + 1;
            _modTier[shop.gameObject.GetInstanceID()] = next;
            ReapplyAllBonuses(shop);
            return next;
        }

        public static void SetTier(WagonShop shop, int tier)
        {
            if (shop == null) return;
            if (tier < MinTier) tier = MinTier;
            if (tier > MaxTier) tier = MaxTier;
            _modTier[shop.gameObject.GetInstanceID()] = tier;
            ReapplyAllBonuses(shop);
        }

        public static void ReapplyAllBonuses(WagonShop shop)
        {
            if (shop == null) return;
            WagonShopPatches.ApplyWorkerBonus(shop);
            BuildingPatches.ApplyRadiusBonus(shop);
            if (shop.registeredWagonsRO != null)
            {
                foreach (var wagon in shop.registeredWagonsRO)
                {
                    if (wagon != null) wagon.CalculateCarryCapacity();
                }
            }
            if (IsDepot(shop)) DepotStorage.EnableForShop(shop);
            RefreshSelectionPanel(shop);
        }

        /// <summary>
        /// Forces the open UIBuildingInfoWindow to re-run Init so vanilla UI elements
        /// (worker count, tier text, etc.) pick up our freshly-applied bonuses.
        /// </summary>
        private static void RefreshSelectionPanel(WagonShop shop)
        {
            try
            {
                var panels = Object.FindObjectsOfType<UIBuildingInfoWindow>();
                foreach (var panel in panels)
                {
                    if (!panel.isActiveAndEnabled) continue;
                    var bound = Traverse.Create(panel).Field("building").GetValue<Building>();
                    if (bound != (Building)shop) continue;
                    panel.Init(shop, shop.GetInfoWindowFlags());
                }
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"[TierHelper] panel refresh failed: {ex.Message}");
            }
        }

        public static void Clear() => _modTier.Clear();
    }
}
