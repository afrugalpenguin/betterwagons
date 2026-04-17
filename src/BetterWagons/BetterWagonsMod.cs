using MelonLoader;

[assembly: MelonInfo(typeof(BetterWagons.BetterWagonsMod), "Better Wagons", "0.1.0", "BetterWagons Team")]
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
            LoggerInstance.Msg("Better Wagons v0.1.0 loaded.");
        }
    }
}
