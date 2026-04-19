using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BetterWagons.Helpers
{
    public static class TierHelper
    {
        // Buildings promoted to Cart Depot (Tier 4)
        private static readonly HashSet<int> _promotedToDepot = new HashSet<int>();

        public static int GetTier(WagonShop shop)
        {
            if (shop == null) return 1;
            int instanceId = shop.gameObject.GetInstanceID();

            if (_promotedToDepot.Contains(instanceId))
                return 4;

            // Detect vanilla tier by checking the upgrade chain
            Building building = shop as Building;
            if (building == null) return 1;

            // Strategy 1: Check building name for tier indicators
            string name = building.buildingDataRecordName ?? "";
            if (name.Contains("_T3") || name.Contains("Tier3") || name.Contains("_t3"))
                return 3;
            if (name.Contains("_T2") || name.Contains("Tier2") || name.Contains("_t2"))
                return 2;

            // Strategy 2: Check upgrade chain via buildingUpgradeInfo
            if (building.buildingUpgradeInfo != null)
            {
                var req = building.buildingUpgradeInfo.upgradeRequirement;
                if (req != null)
                {
                    // Access upgradedPrefab via Traverse (may be a field or property)
                    var prefab = Traverse.Create(req).Property("upgradedPrefab").GetValue<GameObject>();
                    if (prefab == null)
                    {
                        // Try as field if property access failed
                        prefab = Traverse.Create(req).Field("_upgradedPrefab").GetValue<GameObject>();
                    }

                    if (prefab == null)
                        return 3; // No more upgrades = max vanilla tier
                    else
                        return DetectTierFromUpgradeDepth(building);
                }
            }

            return 1;
        }

        private static int DetectTierFromUpgradeDepth(Building building)
        {
            // If building has upgrade info with a valid next prefab, it's not at max tier
            var req = building.buildingUpgradeInfo?.upgradeRequirement;
            if (req == null) return 1;

            var nextPrefab = Traverse.Create(req).Property("upgradedPrefab").GetValue<GameObject>();
            if (nextPrefab == null)
            {
                nextPrefab = Traverse.Create(req).Field("_upgradedPrefab").GetValue<GameObject>();
            }
            if (nextPrefab == null) return 3;

            var nextBuilding = nextPrefab.GetComponent<Building>();
            if (nextBuilding?.buildingUpgradeInfo != null)
            {
                var nextReq = nextBuilding.buildingUpgradeInfo.upgradeRequirement;
                if (nextReq != null)
                {
                    var nextNextPrefab = Traverse.Create(nextReq).Property("upgradedPrefab").GetValue<GameObject>();
                    if (nextNextPrefab == null)
                    {
                        nextNextPrefab = Traverse.Create(nextReq).Field("_upgradedPrefab").GetValue<GameObject>();
                    }
                    if (nextNextPrefab == null)
                        return 2; // Next is max = we're tier 2
                }
            }
            return 1;
        }

        public static bool CanPromoteToDepot(WagonShop shop)
        {
            if (shop == null) return false;
            int tier = GetTier(shop);
            if (tier != 3) return false;
            if (_promotedToDepot.Contains(shop.gameObject.GetInstanceID())) return false;
            return true;
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

        public static void Clear()
        {
            _promotedToDepot.Clear();
        }
    }
}
