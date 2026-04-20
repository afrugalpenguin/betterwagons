using HarmonyLib;
using MelonLoader;
using BetterWagons.Features;
using BetterWagons.Helpers;

namespace BetterWagons.Patches
{
    /// <summary>
    /// Appends Better Wagons state (current resource priority, tier, depot status)
    /// to the building info panel's description text when a WagonShop is selected.
    /// Panel refreshes on re-select; Toast covers the transient update case.
    /// </summary>
    [HarmonyPatch(typeof(UIBuildingInfoWindow), "Init")]
    public static class ShopInfoPanelPatch
    {
        [HarmonyPostfix]
        public static void Init_Postfix(UIBuildingInfoWindow __instance, Building _building)
        {
            try
            {
                if (_building == null) return;
                var shop = _building.GetComponent<WagonShop>();
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
                    upgradeHint = $"\n<size=12>Ctrl+Shift+U: upgrade to {nextLabel}</size>";
                }

                string extra =
                    "\n\n<b>[Better Wagons]</b>" +
                    $"\n{tierLabel}" +
                    $"\nResource Priority: {pref}" +
                    storageLine +
                    "\n<size=12>Ctrl+P: cycle priority</size>" +
                    upgradeHint;

                // Read hiResObjs (private) and descriptionText.text via Traverse
                var hiResObjs = Traverse.Create(__instance).Field("hiResObjs").GetValue();
                if (hiResObjs == null) return;
                var descText = Traverse.Create(hiResObjs).Field("descriptionText").GetValue();
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
