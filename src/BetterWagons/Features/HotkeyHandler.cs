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
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            if (!ctrl || shift || alt) return;

            // Ctrl+P: cycle resource priority on selected WagonShop.
            if (Input.GetKeyDown(KeyCode.P)) CycleResourcePriority();
            // Ctrl+A: toggle persistent assignment on selected TransportWagon.
            else if (Input.GetKeyDown(KeyCode.A)) TogglePersistentAssignment();
        }

        private static GameObject GetSelectedObject()
        {
            var gm = UnitySingleton<GameManager>.Instance;
            if (gm == null) return null;
            var input = gm.inputManager;
            if (input == null) return null;
            return input.selectedObject;
        }

        private static T FindOnOrAround<T>(GameObject obj) where T : Component
        {
            if (obj == null) return null;
            var c = obj.GetComponent<T>();
            if (c != null) return c;
            c = obj.GetComponentInParent<T>();
            if (c != null) return c;
            c = obj.GetComponentInChildren<T>();
            return c;
        }

        private static void CycleResourcePriority()
        {
            var obj = GetSelectedObject();
            if (obj == null)
            {
                Toast.Show("Ctrl+P: select a WagonShop first");
                return;
            }
            var shop = FindOnOrAround<WagonShop>(obj);
            if (shop == null)
            {
                Toast.Show($"Ctrl+P: '{obj.name}' is not a WagonShop");
                return;
            }
            ResourcePriority.CyclePreference(shop);
            var pref = ResourcePriority.GetPreference(shop);
            Toast.Show($"{shop.name}: priority -> {pref}");
            MelonLogger.Msg($"[BetterWagons] {shop.name}: resource priority -> {pref}");
        }

        private static void TogglePersistentAssignment()
        {
            var obj = GetSelectedObject();
            if (obj == null)
            {
                Toast.Show("Ctrl+A: select a TransportWagon first");
                return;
            }
            var wagon = FindOnOrAround<TransportWagon>(obj);
            if (wagon == null)
            {
                Toast.Show($"Ctrl+A: '{obj.name}' is not a TransportWagon");
                return;
            }

            if (PersistentAssignment.HasAssignment(wagon))
            {
                PersistentAssignment.SetAssignment(wagon, null);
                Toast.Show($"{wagon.name}: persistent assignment CLEARED");
                return;
            }

            var target = wagon.priorityPickup;
            if (target == null)
            {
                Toast.Show($"{wagon.name}: set a priority pickup first, then Ctrl+A");
                return;
            }
            PersistentAssignment.SetAssignment(wagon, target);
            Toast.Show($"{wagon.name}: persistent assignment -> {target.name}");
        }
    }
}
