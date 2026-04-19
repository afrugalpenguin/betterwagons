using HarmonyLib;
using BetterWagons.Features;
using UnityEngine;

namespace BetterWagons.Patches
{
    /// <summary>
    /// When a wagon becomes available for work (task completed, parked idle),
    /// re-apply its persistent assignment as priorityPickup so it returns
    /// to its assigned building on the next task search.
    /// </summary>
    [HarmonyPatch(typeof(TransportWagon))]
    public static class PersistentAssignmentPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("availableForWork", MethodType.Setter)]
        public static void AvailableForWork_Setter_Postfix(TransportWagon __instance, bool value)
        {
            if (!value) return;
            if (!PersistentAssignment.HasAssignment(__instance)) return;

            GameObject building = PersistentAssignment.GetAssignedBuilding(__instance);
            if (building == null) return;

            // Only re-apply when priorityPickup has been cleared (task completed).
            if (__instance.priorityPickup == null)
            {
                __instance.SetPriorityPickup(building);
            }
        }
    }
}
