using HarmonyLib;
using BetterWagons.Features;

namespace BetterWagons.Patches
{
    /// <summary>
    /// Biases logistics task assignment by the wagon's workshop resource preference.
    /// OnWorkerAssignedToRequest is called when a wagon is given a request; we postfix
    /// it and, when the request's items match the workshop preference, boost the
    /// stored priority via SetAssignedPriorityForRequest.
    /// </summary>
    [HarmonyPatch(typeof(LogisticsAssignment))]
    public static class LogisticsSearchPatches
    {
        private const int MatchBoost = 1;

        [HarmonyPostfix]
        [HarmonyPatch("OnWorkerAssignedToRequest")]
        public static void OnWorkerAssignedToRequest_Postfix(
            LogisticsAssignment __instance,
            LogisticsRequest request,
            LogisticsAssignment.AssignmentCategory assignmentCategory,
            LogisticsAssignment.AssignmentPriority assignmentPriority)
        {
            if (!(__instance.ownerWorker is TransportWagon wagon)) return;
            if (wagon.wagonShop == null) return;

            ResourceCategory pref = ResourcePriority.GetPreference(wagon.wagonShop);
            if (pref == ResourceCategory.General) return;

            if (!(request is ItemRequest itemRequest)) return;
            if (!RequestMatchesPreference(itemRequest, pref)) return;

            var boosted = new LogisticsAssignment.AssignmentPriority(
                assignmentPriority.globalPriority + MatchBoost,
                assignmentPriority.requestPriority,
                assignmentPriority.secondaryPriority);

            __instance.SetAssignedPriorityForRequest(request, assignmentCategory, boosted);
        }

        private static bool RequestMatchesPreference(ItemRequest request, ResourceCategory pref)
        {
            var reservations = request.reservationsRO;
            if (reservations != null)
            {
                for (int i = 0; i < reservations.Count; i++)
                {
                    if (ResourcePriority.ItemMatchesPreference(reservations[i].itemID, pref))
                        return true;
                }
            }
            return false;
        }
    }
}
