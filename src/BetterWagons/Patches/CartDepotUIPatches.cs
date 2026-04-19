using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using BetterWagons.Features;
using BetterWagons.Helpers;

namespace BetterWagons.Patches
{
    /// <summary>
    /// Repurposes the vanilla upgrade widget on a max-tier WagonShop to offer
    /// a "Promote to Cart Depot" button instead of hiding. When the user clicks
    /// it, DepotPromotion.TryPromote applies Tier 4 stats to the shop.
    /// </summary>
    [HarmonyPatch(typeof(UIBuildingUpgradeWidget))]
    public static class CartDepotUIPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("Init")]
        public static void Init_Postfix(
            UIBuildingUpgradeWidget __instance,
            Building _building,
            BuildingUpgradeInfo _upgradeInfo)
        {
            if (_upgradeInfo != null) return;
            if (!(_building is WagonShop shop)) return;
            if (!TierHelper.CanPromoteToDepot(shop)) return;
            ApplyCartDepotState(__instance, shop);
        }

        [HarmonyPostfix]
        [HarmonyPatch("UpdateBuildingProgression")]
        public static void UpdateBuildingProgression_Postfix(UIBuildingUpgradeWidget __instance)
        {
            var t = Traverse.Create(__instance);
            var currentBuilding = t.Field("currentBuilding").GetValue<Building>();
            var upgradeInfo = t.Field("upgradeInfo").GetValue<BuildingUpgradeInfo>();
            if (upgradeInfo != null) return;
            if (!(currentBuilding is WagonShop shop)) return;
            if (!TierHelper.CanPromoteToDepot(shop)) return;
            ApplyCartDepotState(__instance, shop);
        }

        private static void ApplyCartDepotState(UIBuildingUpgradeWidget widget, WagonShop shop)
        {
            var t = Traverse.Create(widget);

            Action promote = () =>
            {
                DepotPromotion.TryPromote(shop);
                // After promotion, CanPromoteToDepot becomes false; a later refresh
                // will skip our postfix and the widget reverts to its normal hidden state.
            };
            t.Field("onUpgradeButtonClicked").SetValue(promote);
            t.Field("showDetails").SetValue(true);

            var upgradeInfoParent = t.Field("upgradeInfoParent").GetValue<GameObject>();
            if (upgradeInfoParent != null) upgradeInfoParent.SetActive(true);
            widget.gameObject.SetActive(true);

            var titleText = t.Field("titleText").GetValue<object>();
            if (titleText != null) Traverse.Create(titleText).Property("text").SetValue("Promote to Cart Depot");

            var tierText = t.Field("tierText").GetValue<object>();
            if (tierText != null) Traverse.Create(tierText).Property("text").SetValue("Tier 4");

            var upgradeButton = t.Field("upgradeButton").GetValue<Button>();
            if (upgradeButton != null) upgradeButton.interactable = true;

            t.Field("upgradeAvailableOverlay").GetValue<GameObject>()?.SetActive(true);
            t.Field("upgradeLockedOverlay").GetValue<GameObject>()?.SetActive(false);
        }
    }
}
