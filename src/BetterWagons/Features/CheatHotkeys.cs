#if DEBUG
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace BetterWagons.Features
{
    public static class CheatHotkeys
    {
        private static bool _freeBuildings;
        private static bool _firstCtrlShiftLogged;

        public static void OnUpdate()
        {
            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (!ctrl || !shift) return;

            if (!_firstCtrlShiftLogged)
            {
                _firstCtrlShiftLogged = true;
                MelonLogger.Msg("[Cheat] Ctrl+Shift detected — input is reaching the mod.");
            }

            if (Input.GetKeyDown(KeyCode.U)) UpgradeUnderCursor();
            else if (Input.GetKeyDown(KeyCode.V)) Invoke("AddVillagers", 10);
            else if (Input.GetKeyDown(KeyCode.S)) Invoke("AutoSave");
            else if (Input.GetKeyDown(KeyCode.F))
            {
                _freeBuildings = !_freeBuildings;
                Invoke("SetFreeBuildings", _freeBuildings);
            }
        }

        /// <summary>
        /// Ctrl+Shift+U: if hovering a WagonShop, bump our mod tier.
        /// Otherwise delegate to CheatManager.UpgradeBuildingUnderCursor (works on vanilla-upgradeable buildings).
        /// </summary>
        private static void UpgradeUnderCursor()
        {
            var gm = UnitySingleton<GameManager>.Instance;
            var hovered = gm?.inputManager?.GetBuildingUnderCursor();
            var shop = hovered as WagonShop;
            if (shop != null)
            {
                int newTier = BetterWagons.Helpers.TierHelper.TryUpgrade(shop);
                if (newTier == 0)
                {
                    Features.Toast.Show($"{shop.name}: already at max tier ({BetterWagons.Helpers.TierHelper.MaxTier})");
                }
                else
                {
                    string label = (newTier == BetterWagons.Helpers.TierHelper.MaxTier) ? "Cart Depot (T4)" : $"Tier {newTier}";
                    Features.Toast.Show($"{shop.name} upgraded to {label}");
                    MelonLogger.Msg($"[Cheat] {shop.name} mod tier -> {newTier}");
                }
                return;
            }
            Invoke("UpgradeBuildingUnderCursor");
        }

        private static CheatManager GetOrCreateCheatManager()
        {
            var cm = Object.FindObjectOfType<CheatManager>();
            if (cm != null) return cm;

            var gm = UnitySingleton<GameManager>.Instance;
            if (gm == null)
            {
                MelonLogger.Warning("[Cheat] GameManager not available; cannot attach CheatManager.");
                return null;
            }
            try
            {
                cm = gm.gameObject.AddComponent<CheatManager>();
                MelonLogger.Msg("[Cheat] CheatManager component attached at runtime.");
            }
            catch (System.Exception ex)
            {
                MelonLogger.Warning($"[Cheat] AddComponent<CheatManager> threw: {ex.Message}");
            }
            return cm;
        }

        private static void Invoke(string methodName, params object[] args)
        {
            var cheatManager = GetOrCreateCheatManager();
            if (cheatManager == null)
            {
                Features.Toast.Show($"Cheat failed: no CheatManager");
                return;
            }
            try
            {
                Traverse.Create(cheatManager).Method(methodName, args).GetValue();
                var argStr = string.Join(", ", System.Array.ConvertAll(args, a => a?.ToString() ?? "null"));
                Features.Toast.Show($"Cheat: {methodName}({argStr})");
                MelonLogger.Msg($"[Cheat] {methodName}({argStr}) invoked.");
            }
            catch (System.Exception ex)
            {
                Features.Toast.Show($"Cheat failed: {methodName}: {ex.Message}");
                MelonLogger.Warning($"[Cheat] {methodName} threw: {ex.Message}");
            }
        }
    }
}
#endif
