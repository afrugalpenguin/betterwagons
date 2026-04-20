using System.Collections.Generic;
using UnityEngine;
using MelonLoader;

namespace BetterWagons.Features
{
    /// <summary>
    /// Per-frame hotkey dispatch. Invoked from BetterWagonsMod.OnUpdate.
    /// - Ctrl+R on a selected WagonShop: cycle resource-priority category.
    /// - Ctrl+A on a selected TransportWagon: toggle persistent assignment to its
    ///   current priorityPickup target. Clearing works by pressing Ctrl+A twice
    ///   (first toggle sets, second toggle clears — wagon's own priorityPickup may
    ///   have been cleared by the game after a delivery, so we also track by
    ///   explicit toggle state).
    /// </summary>
    public static class HotkeyHandler
    {
        public static void OnUpdate()
        {
            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (!ctrl) return;

            if (Input.GetKeyDown(KeyCode.R)) CycleResourcePriority();
            else if (Input.GetKeyDown(KeyCode.A)) TogglePersistentAssignment();
        }

        private static List<GameObject> GetSelectedObjects()
        {
            var selection = Object.FindObjectOfType<UIObjectSelection>();
            return selection != null ? selection.selectedObjs : null;
        }

        private static void CycleResourcePriority()
        {
            var selected = GetSelectedObjects();
            if (selected == null) return;
            int count = 0;
            foreach (var obj in selected)
            {
                if (obj == null) continue;
                var shop = obj.GetComponent<WagonShop>();
                if (shop == null) continue;
                ResourcePriority.CyclePreference(shop);
                count++;
                MelonLogger.Msg($"[BetterWagons] {shop.name}: resource priority -> {ResourcePriority.GetPreference(shop)}");
            }
            if (count == 0) MelonLogger.Msg("[BetterWagons] Ctrl+R ignored: no WagonShop selected");
        }

        private static void TogglePersistentAssignment()
        {
            var selected = GetSelectedObjects();
            if (selected == null) return;
            int count = 0;
            foreach (var obj in selected)
            {
                if (obj == null) continue;
                var wagon = obj.GetComponent<TransportWagon>();
                if (wagon == null) continue;
                count++;

                if (PersistentAssignment.HasAssignment(wagon))
                {
                    PersistentAssignment.SetAssignment(wagon, null);
                    continue;
                }

                var target = wagon.priorityPickup;
                if (target == null)
                {
                    MelonLogger.Msg($"[BetterWagons] {wagon.name}: set a priority pickup first, then Ctrl+A to persist it");
                    continue;
                }
                PersistentAssignment.SetAssignment(wagon, target);
            }
            if (count == 0) MelonLogger.Msg("[BetterWagons] Ctrl+A ignored: no TransportWagon selected");
        }
    }
}
