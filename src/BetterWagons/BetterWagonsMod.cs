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
            LoggerInstance.Msg("Hotkeys: Ctrl+R = cycle resource priority (WagonShop), Ctrl+A = toggle persistent assignment (TransportWagon)");
        }

        public override void OnUpdate()
        {
            HotkeyHandler.OnUpdate();
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
