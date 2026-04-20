# Better Wagons

<p align="center">
  <img src="https://img.shields.io/badge/game-Farthest%20Frontier-8B6F47?style=for-the-badge&logo=steam&logoColor=white" alt="Farthest Frontier"/>
  <img src="https://img.shields.io/badge/MelonLoader-required-39FF14?style=for-the-badge&logo=unity&logoColor=black" alt="MelonLoader"/>
  <img src="https://img.shields.io/badge/Harmony-2.x-5865F2?style=for-the-badge" alt="Harmony"/>
  <img src="https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET Framework 4.7.2"/>
  <img src="https://img.shields.io/badge/platform-Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white" alt="Windows"/>
</p>

<p align="center">
  <img src="https://img.shields.io/github/last-commit/afrugalpenguin/betterwagons?style=flat-square&color=orange" alt="last commit"/>
  <img src="https://img.shields.io/github/issues/afrugalpenguin/betterwagons?style=flat-square&color=blue" alt="open issues"/>
  <img src="https://img.shields.io/github/issues-pr/afrugalpenguin/betterwagons?style=flat-square&color=purple" alt="open PRs"/>
  <img src="https://img.shields.io/badge/PRs-welcome-brightgreen?style=flat-square" alt="PRs welcome"/>
  <img src="https://img.shields.io/github/license/afrugalpenguin/betterwagons?style=flat-square&color=lightgrey" alt="license"/>
</p>

A MelonLoader mod for **Farthest Frontier** that rebalances wagon workshops: cheaper early game, more worker slots per upgrade, a new Tier-4 **Cart Depot** with on-site storage, persistent per-wagon assignments, per-workshop resource priority, and tuned logistics thresholds. All changes are runtime Harmony patches - no game files are modified.

## Status

**Beta / work in progress.** The mod is playable end-to-end on the current FF Mono build but still has rough edges and is not yet published to the Steam Workshop. Active work and known issues live in [GitHub Issues](https://github.com/afrugalpenguin/betterwagons/issues).

What's working today:

- Mod-owned Tier 1-4 progression for WagonShops (vanilla caps at T1).
- In-card upgrade button on the selected-shop panel with a "Promote to: Tier N" tooltip - no hotkey needed.
- `[Better Wagons]` info block appended to the shop description (tier, resource priority, storage, hotkey hint).
- Persistent per-wagon assignments (`Ctrl+A`), per-workshop resource priority (`Ctrl+P`), and logistics threshold tuning.
- In-game toast notifications styled to match FF's UI.

What's in flight / deferred:

- Tier and assignment persistence across save/load ([#3](https://github.com/afrugalpenguin/betterwagons/issues/3)).
- Cart Depot visual swap (bespoke T4 model with loading slots).
- Resource-priority info line refresh without a deselect/reselect.

## Install

1. Install [MelonLoader](https://melonwiki.xyz/#/) for Farthest Frontier (use the **Mono** build - the game ships a Mono version in `Farthest Frontier (Mono)\`).
2. Download the latest `BetterWagons.dll` from releases (or build locally - see below).
3. Copy `BetterWagons.dll` into `Farthest Frontier (Mono)\Mods\`.
4. Launch the game from `Farthest Frontier (Mono)\Farthest Frontier.exe`.

On first launch the mod creates `UserData\MelonPreferences.cfg` with a `[BetterWagons]` section - edit values there to tune the mod.

## Features

### Wagon workshop tiers
- **Tier 1 (Workshop)** - cheaper base build cost (`Tier1CostMultiplier`, default 0.8).
- **Tier 2 (Upgraded)** - +1 worker slot, +30% capacity, +25% service radius (default).
- **Tier 3 (Master)** - +2 worker slots (cumulative), +50% capacity, +40% radius.
- **Tier 4 (Cart Depot)** - the final promotion. +5 worker slots (cumulative), +60% capacity, +50% radius, +500 weight of on-site storage.

Tier promotion uses the vanilla upgrade button on the selected-shop info card. Hover the button for a "Promote to: Tier N / Bonus / Cost" tooltip, click to promote in place (no rebuild, no wagon loss). At T4 the button disappears.

### Wagon improvements
- Flat +10% wagon movement speed (`WagonSpeedMultiplier`).
- Per-shop capacity scaling stacks with vanilla tech-tree bonuses.

### Logistics tuning
Lower the vanilla thresholds so wagons respond to demand earlier and more often:
- `MinWeightForBulkTransport` 1000 → 600
- `MinPercToTriggerDeliver` 0.9 → 0.8
- `MinPercToRequestDelivered` 0.2 → 0.3

### Persistent wagon assignment
Select a wagon, set its priority pickup target (vanilla), then press **Ctrl+A** to make the assignment stick. After each delivery the wagon automatically returns to the assigned target instead of going idle. Press **Ctrl+A** again to clear.

### Per-workshop resource priority
Select a WagonShop and press **Ctrl+P** to cycle its resource preference through: **General → Raw Materials → Food & Agriculture → Manufactured Goods → General**. Wagons from that shop get a small priority boost for tasks involving matching items - still take other work when idle, but prefer category-aligned runs.

## Hotkeys

| Keys   | Context                    | Effect                                          |
| ------ | -------------------------- | ----------------------------------------------- |
| Ctrl+P | Selected WagonShop         | Cycle resource priority category                |
| Ctrl+A | Selected TransportWagon    | Toggle persistent assignment to priority pickup |

Feedback is shown via an in-game toast overlay styled to match FF's UI, and also logged to the MelonLoader console.

## Configuration

Every multiplier and threshold is exposed in `UserData\MelonPreferences.cfg` under `[BetterWagons]`. Examples:

```
[BetterWagons]
Tier2CapacityMultiplier = 1.3
Tier4StorageCapacity = 500.0
WagonSpeedMultiplier = 1.1
MinWeightForBulkTransport = 600.0
```

Restart the game for changes to take effect.

## Building from source

Requires .NET SDK 8 and the game installed.

```bat
scripts\setup-libs.bat       REM copies reference DLLs from the game install into lib\
scripts\build.bat            REM produces src\BetterWagons\bin\Release\BetterWagons.dll
scripts\install.bat          REM copies the DLL into the game's Mods folder
```

## Compatibility

- Targets the Mono build of Farthest Frontier only. The IL2CPP build is not supported.
- Developed against game version on 2026-04-19. Later game updates may break UI/class hooks.
- No save-file mutations. Disabling the mod reverts to vanilla behavior, though Cart Depot promotions will revert to Tier-3 stats (the building itself remains a WagonShop).

## Known limitations

- Tier promotions and persistent wagon assignments are tracked by runtime GameObject instance IDs - they do **not** survive save/load. Re-promote shops and re-apply `Ctrl+A` after loading. Tracked in [#3](https://github.com/afrugalpenguin/betterwagons/issues/3).
- Resource-priority info line on the shop card doesn't re-render after `Ctrl+P`; deselect and reselect the shop to see the updated value.
- Upgrade-button label and tier text are English-only (not localized).
- Resource-priority matching inspects `ItemRequest.reservationsRO`; early-assignment requests with no reservations yet get the base priority (no boost) rather than a blocked boost.

## License

See [LICENSE](LICENSE).
