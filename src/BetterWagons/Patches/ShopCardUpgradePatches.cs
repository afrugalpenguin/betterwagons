using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using BetterWagons.Features;
using BetterWagons.Helpers;

namespace BetterWagons.Patches
{
    /// <summary>
    /// Vanilla WagonShops have no BuildingUpgradeInfo so the progression
    /// widget hides its upgrade button. For WagonShops with mod tiers
    /// remaining, force the button visible/interactable and neutralise the
    /// (now-misleading) tooltip data provider.
    /// </summary>
    [HarmonyPatch(typeof(UIBuildingProgressionSimple), "UpdateBuildingProgression")]
    public static class UpdateBuildingProgressionPatch
    {
        [HarmonyPostfix]
        public static void Postfix(UIBuildingProgressionSimple __instance)
        {
            try
            {
                var building = Traverse.Create(__instance).Field("building").GetValue<Building>();
                if (building == null) return;
                var shop = building.GetComponent<WagonShop>();
                if (shop == null) return;

                var button = Traverse.Create(__instance).Field("upgradeButton").GetValue<Button>();
                if (button == null) return;

                int tier = TierHelper.GetTier(shop);
                bool canUpgrade = TierHelper.CanUpgrade(shop);

                var currentTierText = Traverse.Create(__instance).Field("currentTierText").GetValue();
                if (currentTierText != null)
                {
                    string tierLabel = TierHelper.IsDepot(shop) ? "Cart Depot" : $"Tier {tier}";
                    Traverse.Create(currentTierText).Property("text").SetValue(tierLabel);
                    var cttGo = Traverse.Create(currentTierText).Property("gameObject").GetValue<GameObject>();
                    if (cttGo != null) cttGo.SetActive(true);
                }
                var maxTierText = Traverse.Create(__instance).Field("maxTierText").GetValue();
                if (maxTierText != null)
                {
                    string next = canUpgrade
                        ? ((tier + 1 == TierHelper.MaxTier) ? "Cart Depot" : $"Tier {tier + 1}")
                        : "";
                    Traverse.Create(maxTierText).Property("text").SetValue(next);
                    var mttGo = Traverse.Create(maxTierText).Property("gameObject").GetValue<GameObject>();
                    if (mttGo != null) mttGo.SetActive(canUpgrade);
                }

                button.gameObject.SetActive(canUpgrade);
                if (!canUpgrade) return;

                button.interactable = true;

                var tooltip = Traverse.Create(__instance).Field("upgradeButtonTooltip").GetValue();
                if (tooltip != null)
                {
                    var behaviour = tooltip as Behaviour;
                    if (behaviour != null) behaviour.enabled = true;
                    PopulateUpgradeTooltip(tooltip, tier, canUpgrade);
                }

                // Hide the confusing "next tier preview" overlay bits that assume a BuildingUpgradeInfo exists.
                var lockedOverlay = Traverse.Create(__instance).Field("upgradeLockedOverlay").GetValue<GameObject>();
                if (lockedOverlay != null) lockedOverlay.SetActive(false);
                var availableOverlay = Traverse.Create(__instance).Field("upgradeAvailableOverlay").GetValue<GameObject>();
                if (availableOverlay != null) availableOverlay.SetActive(true);
                var arrow = Traverse.Create(__instance).Field("upgradeArrow").GetValue<GameObject>();
                if (arrow != null) arrow.SetActive(true);
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"[UpdateBuildingProgressionPatch] threw: {ex.Message}");
            }
        }

        private static void PopulateUpgradeTooltip(object tooltip, int currentTier, bool canUpgrade)
        {
            var keyNames = Traverse.Create(tooltip).Property("toolTipRowKeyNames").GetValue() as System.Collections.IList;
            var values = Traverse.Create(tooltip).Property("toolTipRowValues").GetValue() as System.Collections.IList;
            var tags = Traverse.Create(tooltip).Field("tooltipRowKeyLocalizationTags").GetValue() as System.Collections.IList;
            keyNames?.Clear();
            values?.Clear();
            tags?.Clear();

            if (!canUpgrade) return;

            int nextTier = currentTier + 1;
            string nextLabel = (nextTier == TierHelper.MaxTier) ? "Cart Depot (T4)" : $"Tier {nextTier}";
            string bonusLine = (nextTier == TierHelper.MaxTier)
                ? "5 wagon slots + bulk storage"
                : "+1 worker, +25% radius, +1 wagon capacity";

            Traverse.Create(tooltip).Method("AddKeyValue", "Promote to:", nextLabel).GetValue();
            Traverse.Create(tooltip).Method("AddKeyValue", "Bonus:", bonusLine).GetValue();
            Traverse.Create(tooltip).Method("AddKeyValue", "Cost:", "Free (mod)").GetValue();
        }
    }

    /// <summary>
    /// Intercepts the upgrade button click for WagonShops and routes it through
    /// TierHelper.TryUpgrade. Returning false skips the vanilla call to
    /// Building.UpgradeBuilding(), which would NRE on the missing
    /// BuildingUpgradeInfo.
    /// </summary>
    [HarmonyPatch(typeof(UIBuildingProgressionSimple), "OnUpgrade")]
    public static class OnUpgradePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(UIBuildingProgressionSimple __instance)
        {
            try
            {
                var building = Traverse.Create(__instance).Field("building").GetValue<Building>();
                if (building == null) return true;
                var shop = building.GetComponent<WagonShop>();
                if (shop == null) return true;

                int newTier = TierHelper.TryUpgrade(shop);
                if (newTier == 0)
                {
                    Toast.Show($"{shop.name}: already at max tier ({TierHelper.MaxTier})");
                }
                else
                {
                    string label = (newTier == TierHelper.MaxTier) ? "Cart Depot (T4)" : $"Tier {newTier}";
                    Toast.Show($"{shop.name} promoted to {label}");
                    MelonLogger.Msg($"[BetterWagons] {shop.name} promoted via UI -> Tier {newTier}");
                }
                return false;
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"[OnUpgradePatch] threw: {ex.Message}");
                return true;
            }
        }
    }
}
