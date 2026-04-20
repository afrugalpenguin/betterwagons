using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using BetterWagons.Helpers;

namespace BetterWagons.Patches
{
    [HarmonyPatch(typeof(WagonShop))]
    public static class BuildingPatches
    {
        private static readonly Dictionary<int, float> _vanillaRadius = new Dictionary<int, float>();

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start_Radius_Postfix(WagonShop __instance)
        {
            int id = __instance.gameObject.GetInstanceID();
            float currentRadius = Traverse.Create(__instance).Field("_strategicPlanningRadius").GetValue<float>();

            if (!_vanillaRadius.ContainsKey(id))
                _vanillaRadius[id] = currentRadius;

            ApplyRadiusBonus(__instance);
        }

        public static void ApplyRadiusBonus(WagonShop shop)
        {
            int id = shop.gameObject.GetInstanceID();
            if (!_vanillaRadius.TryGetValue(id, out float vanillaRadius))
            {
                vanillaRadius = Traverse.Create(shop).Field("_strategicPlanningRadius").GetValue<float>();
                _vanillaRadius[id] = vanillaRadius;
            }

            int tier = TierHelper.GetTier(shop);

            float multiplier;
            switch (tier)
            {
                case 2:
                    multiplier = ModConfig.Tier2RadiusMultiplier.Value;
                    break;
                case 3:
                    multiplier = ModConfig.Tier3RadiusMultiplier.Value;
                    break;
                case 4:
                    multiplier = ModConfig.Tier4RadiusMultiplier.Value;
                    break;
                default:
                    multiplier = 1.0f;
                    break;
            }

            float newRadius = vanillaRadius * multiplier;
            Traverse.Create(shop).Field("_strategicPlanningRadius").SetValue(newRadius);
            MelonLogger.Msg($"[BetterWagons] WagonShop {id} tier {tier}: radius {vanillaRadius} * {multiplier} = {newRadius}");
        }
    }
}
