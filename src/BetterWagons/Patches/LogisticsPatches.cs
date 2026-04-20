using HarmonyLib;
using MelonLoader;

namespace BetterWagons.Patches
{
    [HarmonyPatch(typeof(GameManager), "Awake")]
    public static class LogisticsPatches
    {
        [HarmonyPostfix]
        public static void Awake_Postfix(GameManager __instance)
        {
            var config = __instance.logisticsConfiguration;
            if (config == null)
            {
                MelonLogger.Warning("[Logistics] logisticsConfiguration is null after GameManager.Awake; skipping tuning.");
                return;
            }

            var traverse = Traverse.Create(config);

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
