using MelonLoader;
using BetterWagons.Features;
using BetterWagons.Helpers;

[assembly: MelonInfo(typeof(BetterWagons.BetterWagonsMod), "BetterWagons", BetterWagons.BuildInfo.Version, "A Frugal Penguin")]
[assembly: MelonGame("Crate Entertainment", "Farthest Frontier")]

namespace BetterWagons
{
    public class BetterWagonsMod : MelonMod
    {
        public static BetterWagonsMod Instance { get; private set; }

        public override void OnInitializeMelon()
        {
            Instance = this;
            ModConfig.Initialize();
            LoggerInstance.Msg($"Better Wagons v{BuildInfo.Version} loaded.");
            LoggerInstance.Msg("Config file: UserData/MelonPreferences.cfg (BetterWagons category)");
#if DEBUG
            LoggerInstance.Warning("DEBUG build: cheat hotkeys enabled. Do not ship this DLL.");
            LoggerInstance.Msg("Hotkeys: Ctrl+P = cycle resource priority (WagonShop), Ctrl+A = toggle persistent assignment (TransportWagon), Ctrl+Shift+U = upgrade under cursor, Ctrl+Shift+F = toggle free build, Ctrl+Shift+V = +10 villagers, Ctrl+Shift+S = force save");
#else
            LoggerInstance.Msg("Hotkeys: Ctrl+P = cycle resource priority (WagonShop), Ctrl+A = toggle persistent assignment (TransportWagon)");
#endif
        }

        public override void OnGUI()
        {
            Toast.OnGUI();
        }

        public override void OnUpdate()
        {
            HotkeyHandler.OnUpdate();
#if DEBUG
            CheatHotkeys.OnUpdate();
#endif
        }

        public override void OnDeinitializeMelon()
        {
            TierHelper.Clear();
            PersistentAssignment.Clear();
            ResourcePriority.Clear();
            DepotStorage.Clear();
        }
    }
}
