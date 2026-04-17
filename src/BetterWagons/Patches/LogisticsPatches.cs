using HarmonyLib;
using MelonLoader;

namespace BetterWagons.Patches
{
    [HarmonyPatch(typeof(LogisticsConfiguration))]
    public static class LogisticsPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake_Postfix(LogisticsConfiguration __instance)
        {
            var traverse = Traverse.Create(__instance);

            float minWeight = ModConfig.MinWeightForBulkTransport.Value;
            traverse.Field("_minWeightForBulkTransport").SetValue(minWeight);
            MelonLogger.Msg($"[Logistics] MinWeightForBulkTransport set to {minWeight}");

            float minDeliver = ModConfig.MinPercToTriggerDeliver.Value;
            traverse.Field("_minPercToTriggerDeliver").SetValue(minDeliver);
            MelonLogger.Msg($"[Logistics] MinPercToTriggerDeliver set to {minDeliver}");

            float minRequest = ModConfig.MinPercToRequestDelivered.Value;
            traverse.Field("_minPercToRequestDelivered").SetValue(minRequest);
            MelonLogger.Msg($"[Logistics] MinPercToRequestDelivered set to {minRequest}");
        }
    }
}
