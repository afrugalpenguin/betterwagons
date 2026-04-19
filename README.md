# Better Wagons

A MelonLoader mod for **Farthest Frontier** that rebalances wagon workshops: cheaper early game, more worker slots per upgrade, a new Tier-4 **Cart Depot** with on-site storage, persistent per-wagon assignments, per-workshop resource priority, and tuned logistics thresholds. All changes are runtime Harmony patches — no game files are modified.

## Install

1. Install [MelonLoader](https://melonwiki.xyz/#/) for Farthest Frontier (use the **Mono** build — the game ships a Mono version in `Farthest Frontier (Mono)\`).
2. Download the latest `BetterWagons.dll` from releases (or build locally — see below).
3. Copy `BetterWagons.dll` into `Farthest Frontier (Mono)\Mods\`.
4. Launch the game from `Farthest Frontier (Mono)\Farthest Frontier.exe`.

On first launch the mod creates `UserData\MelonPreferences.cfg` with a `[BetterWagons]` section — edit values there to tune the mod.

## Features

### Wagon workshop tiers
- **Tier 1 (Workshop)** — cheaper base build cost (`Tier1CostMultiplier`, default 0.8).
- **Tier 2 (Upgraded)** — +1 worker slot, +30% capacity, +25% service radius (default).
- **Tier 3 (Master)** — +2 worker slots (cumulative), +50% capacity, +40% radius.
- **Tier 4 (Cart Depot)** — unlockable via the upgrade panel once at max vanilla tier. +5 worker slots (cumulative), +60% capacity, +50% radius, +500 weight of on-site storage.

The Tier-4 upgrade button appears in the building's upgrade panel once the shop is fully upgraded; clicking it promotes the existing shop to a Cart Depot (no rebuild).

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
Select a WagonShop and press **Ctrl+R** to cycle its resource preference through: **General → Raw Materials → Food & Agriculture → Manufactured Goods → General**. Wagons from that shop get a small priority boost for tasks involving matching items — still take other work when idle, but prefer category-aligned runs.

## Hotkeys

| Keys    | Context                      | Effect                                         |
| ------- | ---------------------------- | ---------------------------------------------- |
| Ctrl+R  | Selected WagonShop(s)        | Cycle resource priority category               |
| Ctrl+A  | Selected TransportWagon(s)   | Toggle persistent assignment to priority pickup |

Feedback lands in the MelonLoader console overlay.

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

- The Tier-4 upgrade button uses the vanilla upgrade widget's text/title — localization is English-only.
- Resource-priority matching inspects `ItemRequest.reservationsRO`; early-assignment requests with no reservations yet get the base priority (no boost) rather than a blocked boost.
- Persistent assignments are tracked by runtime GameObject instance IDs — they do **not** survive save/load. Re-apply Ctrl+A after loading.

## License

See [LICENSE](LICENSE).
