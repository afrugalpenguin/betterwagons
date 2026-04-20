using HarmonyLib;
using MelonLoader;
using BetterWagons.Helpers;

namespace BetterWagons.Patches
{
    [HarmonyPatch(typeof(TransportWagon))]
    public static class TransportWagonPatches
    {
        [HarmonyPatch("CalculateCarryCapacity")]
        [HarmonyPostfix]
        public static void CalculateCarryCapacity_Postfix(TransportWagon __instance)
        {
            if (__instance.wagonShop == null || __instance.temporaryInventory == null)
                return;

            int tier = TierHelper.GetTier(__instance.wagonShop);

            float multiplier;
            switch (tier)
            {
                case 2:
                    multiplier = ModConfig.Tier2CapacityMultiplier.Value;
                    break;
                case 3:
                    multiplier = ModConfig.Tier3CapacityMultiplier.Value;
                    break;
                case 4:
                    multiplier = ModConfig.Tier4CapacityMultiplier.Value;
                    break;
                default:
                    return;
            }

            __instance.temporaryInventory.carryCapacity *= multiplier;
        }

        private static bool _firstAwakeLogged;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake_Postfix(TransportWagon __instance)
        {
            if (!_firstAwakeLogged)
            {
                _firstAwakeLogged = true;
                BetterWagons.Helpers.TraceLog.Write($"[TRACE] First TransportWagon.Awake_Postfix fired, name={__instance?.name}");
            }
            try
            {
                var traverse = Traverse.Create(__instance).Field("_movementSpeed");
                float baseSpeed = traverse.GetValue<float>();
                traverse.SetValue(baseSpeed * ModConfig.WagonSpeedMultiplier.Value);
            }
            catch (System.Exception ex)
            {
                BetterWagons.Helpers.TraceLog.Write($"[TRACE] TransportWagon.Awake_Postfix THREW: {ex.Message}");
            }
        }
    }
}
