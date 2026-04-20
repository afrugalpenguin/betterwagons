using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using BetterWagons.Helpers;

namespace BetterWagons.Patches
{
    [HarmonyPatch(typeof(WagonShop))]
    public static class WagonShopPatches
    {
        private static readonly Dictionary<int, int> _vanillaMaxWorkers = new Dictionary<int, int>();

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start_Postfix(WagonShop __instance)
        {
            BetterWagons.Helpers.TraceLog.Write($"[TRACE] WagonShop.Start_Postfix ENTER name={__instance?.name ?? "null"}");
            try
            {
                int id = __instance.gameObject.GetInstanceID();
                if (!_vanillaMaxWorkers.ContainsKey(id))
                    _vanillaMaxWorkers[id] = __instance.maxWorkers;

                ApplyWorkerBonus(__instance);
            }
            catch (System.Exception ex)
            {
                BetterWagons.Helpers.TraceLog.Write($"[TRACE] WagonShop.Start_Postfix THREW: {ex}");
            }
        }

        public static void ApplyWorkerBonus(WagonShop shop)
        {
            int id = shop.gameObject.GetInstanceID();
            if (!_vanillaMaxWorkers.TryGetValue(id, out int vanillaMax))
            {
                vanillaMax = shop.maxWorkers;
                _vanillaMaxWorkers[id] = vanillaMax;
            }

            int tier = TierHelper.GetTier(shop);

            int bonus;
            switch (tier)
            {
                case 2:
                    bonus = ModConfig.Tier2BonusWorkers.Value;
                    break;
                case 3:
                    bonus = ModConfig.Tier3BonusWorkers.Value;
                    break;
                case 4:
                    bonus = ModConfig.Tier4BonusWorkers.Value;
                    break;
                default:
                    bonus = 0;
                    break;
            }

            int newMax = vanillaMax + bonus;
            shop.ModifyMaxWorkers(newMax);
            MelonLogger.Msg($"[BetterWagons] WagonShop {id} tier {tier}: maxWorkers {vanillaMax} + {bonus} = {newMax}");
        }
    }
}
