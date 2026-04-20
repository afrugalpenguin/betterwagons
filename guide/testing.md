# Better Wagons - In-Game Test Plan

A linear test plan for validating every feature of the mod without waiting hours to progress a vanilla city. Uses Farthest Frontier's built-in cheat console (`CheatManager` via `SickDev.DevConsole`) to skip the grind.

Each phase lists a goal, any cheat commands to run, the steps, and a pass/fail check.

## Pre-flight

1. Confirm `BetterWagons.dll` is in `Farthest Frontier (Mono)\Mods\`.
2. Launch the game. Start a new sandbox or load any save with a Town Center placed.
3. The separate MelonLoader console window should show `[BetterWagons]` banner lines as Harmony patches apply. If not, the mod failed to load - check `MelonLoader\Latest.log`.
4. In-game, press **`** (backtick) to open the cheat console. If nothing opens, try **~** or the key left of **1**. If still nothing, the console UI may be hidden behind a dev build flag; in that case, fall back to reflection-invoking `CheatManager` methods from the mod.
5. Run `SetFreeBuildings true` so you can place buildings without accumulating resources.

## Phase 1 - T1 cost multiplier

- **Goal:** confirm Tier 1 WagonShop is cheaper than vanilla.
- Begin placing a new WagonShop. Hover the placement preview.
- **Pass:** resource costs in the placement tooltip are roughly 80% of the vanilla figures (default `Tier1CostMultiplier = 0.8`).
- Place one and let it build (or leave free-buildings on).

## Phase 2 - T2 progression

- **Goal:** confirm T2 adds worker slots, capacity, service radius.
- Select the new WagonShop. Note current worker slots, wagon capacity, and service radius.
- Hover the building and run `UpgradeBuildingUnderCursor` in the console.
- Select the shop again.
- **Pass:** +1 worker slot, capacity roughly 1.3x, radius roughly 1.25x vs T1.

## Phase 3 - T3 progression

- Run `UpgradeBuildingUnderCursor` again with the cursor over the same shop.
- **Pass:** worker slots are T1 + 2 total, capacity roughly 1.5x T1, radius roughly 1.4x T1.

## Phase 4 - Cart Depot promotion

- With the T3 shop selected, open its upgrade panel.
- **Pass:** a new "Cart Depot" upgrade button is present where there was no further vanilla upgrade before.
- Click it.
- **Pass:** the building promotes in place (no rebuild), worker slots jump by +5 vs T3, capacity roughly 1.6x T1, radius roughly 1.5x T1, and a storage UI appears on the selected building.

## Phase 5 - Cart Depot storage

- Run `AddItem Wood 500` (or any bulk item).
- Let a wagon auto-haul, or order a wagon to deliver to the depot.
- **Pass:** the depot accepts and stores items up to roughly 500 weight. A stockpile-style UI on the depot shows contents.

## Phase 6 - Wagon speed

- Spawn a wagon: `SpawnTransportWagon`.
- Send it on a long errand (pickup to dropoff across the map).
- **Pass:** wagons feel roughly 10% quicker than pre-mod memory. Eyeballed, not precise. A crawling wagon is the failure signal.

## Phase 7 - Persistent assignment (Ctrl+A)

- Select a TransportWagon.
- Set a priority pickup target on a building via the vanilla UI.
- Press **Ctrl+A**. The MelonLoader overlay should print a confirmation line.
- Wait for the wagon to complete one delivery.
- **Pass:** after delivery, the wagon returns to the assigned pickup target instead of going idle.
- Press **Ctrl+A** again. **Pass:** assignment clears; wagon goes idle after the next delivery.

## Phase 8 - Resource priority (Ctrl+R)

- Select a WagonShop. Press **Ctrl+R**.
- **Pass:** the overlay shows the category cycle: `General -> Raw Materials -> Food & Agriculture -> Manufactured Goods -> General`.
- With a non-General category active, set up two WagonShops (one with priority set, one General) and create demand for mixed items.
- **Pass:** the priority-set shop picks up matching items noticeably more often but still takes non-matching work when idle.

## Phase 9 - Logistics thresholds

- Construct a low-demand scenario: a stockpile at around 30% of its target (below the vanilla 0.9 deliver trigger).
- **Pass:** wagons begin hauling earlier than vanilla would. Hard to quantify live; the confirmation is "wagons don't feel lazy".

## Phase 10 - Save/load sanity

- Run `AutoSave` in the console.
- Exit to main menu, reload the save.
- **Pass:** Cart Depot stays at Tier 4 with storage intact; Ctrl+A assignments reset (documented limitation - re-apply them after load).
- **Fail:** any crash on load, a promoted depot reverts to a lower tier, or config values are ignored after reload.

## If any phase fails

Open `MelonLoader\Latest.log` in the game install folder, copy the stacktrace or error line around the failure, and raise an issue on the repo. Include the phase number and expected vs actual behavior.

## Useful cheat commands cheat-sheet

| Command                              | Effect                                              |
| ------------------------------------ | --------------------------------------------------- |
| `SetFreeBuildings true`              | Place any building without paying resources         |
| `UpgradeBuildingUnderCursor`         | Instant-upgrade the building under the cursor       |
| `SpawnTransportWagon`                | Spawn a wagon                                       |
| `SpawnControlledWagonAtCursor`       | Spawn a wagon at cursor position                    |
| `AddItem <itemType> <count>`         | Add resources to the settlement                     |
| `RemoveAllOfItem <itemType>`         | Zero out a resource to force demand                 |
| `AddVillagers <count>`               | Fill worker slots instantly                         |
| `AutoSave`                           | Trigger a save (useful mid-test)                    |
| `CondemnBuildingUnderCursor`         | Condemn for demolition test                         |
| `UncondemnBuildingUnderCursor`       | Reverse the above                                   |
