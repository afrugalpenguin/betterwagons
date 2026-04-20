using System.Linq;
using HarmonyLib;
using MelonLoader;
using UnityEngine.SceneManagement;
using BetterWagons.Features;
using BetterWagons.Helpers;

[assembly: MelonInfo(typeof(BetterWagons.BetterWagonsMod), "BetterWagons", BetterWagons.BuildInfo.Version, "A Frugal Penguin")]
[assembly: MelonGame("Crate Entertainment", "Farthest Frontier")]

namespace BetterWagons
{
    public class BetterWagonsMod : MelonMod
    {
        public static BetterWagonsMod Instance { get; private set; }
        private bool _harmonyDiagLogged;
        private System.DateTime _lastHeartbeat = System.DateTime.UtcNow;

        public override void OnInitializeMelon()
        {
            Instance = this;
            ModConfig.Initialize();
            SceneManager.sceneLoaded += UnitySceneLoaded;
            SceneManager.sceneUnloaded += UnitySceneUnloaded;
            LoggerInstance.Msg($"Better Wagons v{BuildInfo.Version} loaded.");
            LoggerInstance.Msg("Config file: UserData/MelonPreferences.cfg (BetterWagons category)");
#if DEBUG
            LoggerInstance.Warning("DEBUG build: cheat hotkeys enabled. Do not ship this DLL.");
            LoggerInstance.Msg("Hotkeys: Ctrl+P = cycle resource priority (WagonShop), Ctrl+A = toggle persistent assignment (TransportWagon), Ctrl+Shift+U = upgrade under cursor, Ctrl+Shift+F = toggle free build, Ctrl+Shift+V = +10 villagers, Ctrl+Shift+S = force save");
#else
            LoggerInstance.Msg("Hotkeys: Ctrl+P = cycle resource priority (WagonShop), Ctrl+A = toggle persistent assignment (TransportWagon)");
#endif
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            TraceLog.Write($"[Scene] Loaded buildIndex={buildIndex} name='{sceneName}'");
            if (_harmonyDiagLogged) return;
            _harmonyDiagLogged = true;
            try
            {
                var patched = HarmonyLib.Harmony.GetAllPatchedMethods().ToArray();
                LoggerInstance.Msg($"[Harmony] Global patched method count: {patched.Length}");
                var myAssembly = typeof(BetterWagonsMod).Assembly.GetName().Name;
                int ownCount = 0;
                foreach (var m in patched)
                {
                    var info = HarmonyLib.Harmony.GetPatchInfo(m);
                    if (info == null) continue;
                    bool ours =
                        info.Prefixes.Any(p => p.owner.Contains(myAssembly)) ||
                        info.Postfixes.Any(p => p.owner.Contains(myAssembly)) ||
                        info.Transpilers.Any(p => p.owner.Contains(myAssembly)) ||
                        info.Finalizers.Any(p => p.owner.Contains(myAssembly));
                    if (ours)
                    {
                        ownCount++;
                        LoggerInstance.Msg($"[Harmony] ours: {m.DeclaringType?.FullName}.{m.Name}");
                    }
                }
                LoggerInstance.Msg($"[Harmony] Our assembly '{myAssembly}' has {ownCount} patches applied.");
            }
            catch (System.Exception ex)
            {
                LoggerInstance.Warning($"[Harmony] Diagnostic failed: {ex.Message}");
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            TraceLog.Write($"[Scene] Initialized buildIndex={buildIndex} name='{sceneName}'");
        }

        private void UnitySceneLoaded(Scene scene, LoadSceneMode mode)
        {
            TraceLog.Write($"[Unity] sceneLoaded name='{scene.name}' buildIndex={scene.buildIndex} mode={mode}");
        }

        private void UnitySceneUnloaded(Scene scene)
        {
            TraceLog.Write($"[Unity] sceneUnloaded name='{scene.name}' buildIndex={scene.buildIndex}");
        }

        public override void OnGUI()
        {
            Toast.OnGUI();
        }

        public override void OnUpdate()
        {
            var now = System.DateTime.UtcNow;
            if ((now - _lastHeartbeat).TotalSeconds >= 10)
            {
                _lastHeartbeat = now;
                HeartbeatDiagnostic();
            }
            HotkeyHandler.OnUpdate();
#if DEBUG
            CheatHotkeys.OnUpdate();
#endif
        }

        private void HeartbeatDiagnostic()
        {
            try
            {
                int sceneCount = SceneManager.sceneCount;
                string sceneInfo = "";
                int totalRoots = 0;
                for (int i = 0; i < sceneCount; i++)
                {
                    var s = SceneManager.GetSceneAt(i);
                    var roots = s.GetRootGameObjects();
                    totalRoots += roots.Length;
                    sceneInfo += $"{s.name}(loaded={s.isLoaded},roots={roots.Length}) ";
                    for (int r = 0; r < roots.Length; r++)
                    {
                        TraceLog.Write($"[HB]   root[{r}] = '{roots[r].name}' active={roots[r].activeSelf}");
                    }
                }

                var gmAll = UnityEngine.Resources.FindObjectsOfTypeAll<GameManager>();
                var gmActive = UnityEngine.Object.FindObjectsOfType<GameManager>();
                GameManager gmSingleton = null;
                try { gmSingleton = UnitySingleton<GameManager>.Instance; } catch { }

                var mb = UnityEngine.Object.FindObjectsOfType<UnityEngine.MonoBehaviour>();
                var ws = UnityEngine.Object.FindObjectsOfType<WagonShop>();
                var tws = UnityEngine.Object.FindObjectsOfType<TransportWagon>();
                var cm = UnityEngine.Object.FindObjectOfType<CheatManager>();

                TraceLog.Write($"[HB] scenes=[{sceneInfo.TrimEnd()}] totalRoots={totalRoots}");
                TraceLog.Write($"[HB] GameManager all={gmAll.Length} active={gmActive.Length} singleton={(gmSingleton != null ? "set" : "null")}");
                TraceLog.Write($"[HB] MonoBehaviour total={mb.Length} WagonShop={ws.Length} TransportWagon={tws.Length} CheatManager={(cm != null ? "yes" : "no")}");

                // Log unique type names among MonoBehaviours (first 40)
                var typeCounts = new System.Collections.Generic.Dictionary<string, int>();
                foreach (var m in mb)
                {
                    if (m == null) continue;
                    var n = m.GetType().Name;
                    if (typeCounts.ContainsKey(n)) typeCounts[n]++;
                    else typeCounts[n] = 1;
                }
                int shown = 0;
                foreach (var kv in typeCounts)
                {
                    if (shown++ >= 40) break;
                    TraceLog.Write($"[HB]   type {kv.Key} x{kv.Value}");
                }
                TraceLog.Write($"[HB] unique MB types total={typeCounts.Count}");
            }
            catch (System.Exception ex)
            {
                TraceLog.Write($"[HB] diagnostic threw: {ex.Message}");
            }
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
