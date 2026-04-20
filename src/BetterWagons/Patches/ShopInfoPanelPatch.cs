using HarmonyLib;
using MelonLoader;
using BetterWagons.Features;
using BetterWagons.Helpers;

namespace BetterWagons.Patches
{
    /// <summary>
    /// Appends Better Wagons state (current resource priority, tier, depot status)
    /// to the building info panel's description text when a WagonShop is selected.
    /// Targets UIBuildingInfoWindow_New (the modern card); the legacy
    /// UIBuildingInfoWindow class exists in the assembly but is not instantiated
    /// in shipping builds.
    /// </summary>
    [HarmonyPatch(typeof(UIBuildingInfoWindow_New), "InitBasicInfo")]
    public static class ShopInfoPanelPatch
    {
        [HarmonyPostfix]
        public static void InitBasicInfo_Postfix(UIBuildingInfoWindow_New __instance)
        {
            try
            {
                var building = Traverse.Create(__instance).Field("building").GetValue<Building>();
                if (building == null) return;
                var shop = building.GetComponent<WagonShop>();
                if (shop == null) return;

                int tier = TierHelper.GetTier(shop);
                var pref = ResourcePriority.GetPreference(shop);
                bool isDepot = TierHelper.IsDepot(shop);
                bool canUpgrade = TierHelper.CanUpgrade(shop);

                string tierLabel = isDepot
                    ? $"Tier {tier} / {TierHelper.MaxTier} — Cart Depot"
                    : $"Tier {tier} / {TierHelper.MaxTier}";

                string storageLine = "";
                if (shop.storage != null)
                {
                    var itemStorage = Traverse.Create(shop.storage).Field("itemStorage").GetValue<ItemStorage>();
                    if (itemStorage != null)
                    {
                        float used = itemStorage.GetWeightCurrentlyCarrying();
                        float cap = itemStorage.carryCapacity;
                        storageLine = $"\nStorage: {used:0}/{cap:0}";
                    }
                }

                string upgradeHint = "";
                if (canUpgrade)
                {
                    int next = tier + 1;
                    string nextLabel = (next == TierHelper.MaxTier) ? "Cart Depot (T4)" : $"Tier {next}";
                    upgradeHint = $"\n<size=12>Click upgrade button or Ctrl+Shift+U to promote to {nextLabel}</size>";
                }

                string extra =
                    "\n\n<b>[Better Wagons]</b>" +
                    $"\n{tierLabel}" +
                    $"\nResource Priority: {pref}" +
                    storageLine +
                    "\n<size=12>Ctrl+P: cycle priority</size>" +
                    upgradeHint;

                var descText = Traverse.Create(__instance).Field("descriptionText").GetValue();
                if (descText == null) return;
                var textProp = Traverse.Create(descText).Property("text");
                string current = textProp.GetValue<string>() ?? "";
                textProp.SetValue(current + extra);
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"[ShopInfoPanelPatch] threw: {ex.Message}");
            }
        }
    }
}
