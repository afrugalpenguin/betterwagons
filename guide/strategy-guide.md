# Better Wagons - Strategy Guide

This guide assumes you've read the vanilla wagon-workshop mechanics. The mod is a rebalance, not a replacement - the same rules about wagon pickup range, town-center logistics demands, and tech-tree capacity bonuses still apply. Everything below is advice for playing well with the mod's parameters.

## When to build your first Workshop

In vanilla the build cost pushes a first workshop to year 3-4. With `Tier1CostMultiplier = 0.8` a basic shop is ~20% cheaper, and you can put one down as early as late year 1 if lumber permits. Build one when **either**:

- You have at least two production chains more than ~25 tiles from storage (quarry + mine, for example), **or**
- You're about to zone housing away from the town center and food distribution is starting to lag.

A lone wagon on vanilla is often worse than a villager walking - two wagons starts paying for itself immediately.

## Placement principles

Workshops do **work within a service radius**, scaled by tier. With the mod:

| Tier | Radius multiplier | Rule of thumb         |
| ---- | ----------------- | --------------------- |
| 1    | 1.00              | Neighborhood          |
| 2    | 1.25              | Ward                  |
| 3    | 1.40              | District              |
| 4    | 1.50              | Town-quarter / hub    |

Place workshops on the **edge** of a cluster, not the center - a depot near the town gate can pull from distant mines and still drop at the market without crossing itself. A Tier-4 Cart Depot on the main road becomes a hub for both ends of the map.

## Upgrade timing

The cumulative worker bonuses are the big prize:

- **Tier 2**: +1 wagon. Cheap (`Tier2CostMultiplier = 0.85`). Upgrade the moment you have the population.
- **Tier 3**: +2 wagons. Worth beelining when any workshop consistently shows "at capacity" status in the logistics overlay.
- **Tier 4 (Cart Depot)**: +5 wagons, +500 storage capacity. Expensive (`Tier4CostMultiplier = 1.5`). Commit to this only for **hub** shops - the ones positioned to serve half the map.

Rule: every town should have 1 Cart Depot by the mid-late game. Don't build two.

## Cart Depot placement

The Tier-4 bonus shines when the Depot sits at a **choke point** on your main roads. A few patterns:

- **Hub-and-spoke**: Depot on the main road, all specialty shops (mine-adjacent, farm-adjacent) stay at Tier 2/3. The Depot picks up finished goods and drops at market; the specialty shops handle their local chain.
- **Edge-of-town**: Depot on the road to a distant resource cluster (e.g. mines uphill). Its 5 wagons keep the long-haul lane saturated.
- **Near the market square**: Depot next to the food market. Its storage absorbs surges at harvest and buffer-feeds the market through winter.

## Persistent wagon assignment

`Ctrl+A` while a wagon is selected locks it to whatever building it's currently targeting as priority pickup. Use it when:

- You have a **remote mine** far from any storage. One wagon ferries ore/stone back to town all day.
- A **livestock pen** needs dedicated carcass/milk/wool runs.
- You want a specific wagon to feed a **single production chain** (e.g. logs → sawmill, nothing else).

Leave most wagons **unassigned**. The general logistics pool reacts to whatever the town needs; over-assigning starves surges.

## Resource priority per shop

`Ctrl+R` cycles a shop's preference: **General → Raw → Food → Manufactured → General**. This is a *soft* preference - wagons still take other work when idle.

Good setups:
- **Lumber-district workshop**: set to Raw Materials. Prefers logs/stone/ore runs, reduces cross-town food interference.
- **Market-adjacent workshop**: set to Food & Agriculture. Prefers keeping the market stocked.
- **Industry-quarter workshop**: set to Manufactured Goods. Prefers hauling tools, weapons, pottery.
- **Cart Depot (Tier 4)**: leave at **General**. It's the hub - it should take whatever's hot.

## Logistics tuning

The lowered thresholds (`MinPercToTriggerDeliver 0.8`, `MinPercToRequestDelivered 0.3`, `MinWeightForBulkTransport 600`) make wagons more responsive but also more active. Watch for:

- **Wagon idle time should drop.** If you still see idle wagons, you're over-provisioned - merge shops or salvage one.
- **Food markets fill faster but empty faster too**. The 0.3 request threshold means markets ask for refills when 70% empty, not 80%. If you run a frontier settlement with low production, *raise* this back toward 0.2 to avoid the market spending all day half-empty chasing trickle deliveries.

## Troubleshooting

- **"My Tier-4 button isn't showing"**: the shop must be at **max vanilla tier** first. Check that the normal upgrade chain is exhausted.
- **"Hotkeys do nothing"**: open the MelonLoader console (F5 in-game by default) and confirm the selection messages appear. Ctrl+R / Ctrl+A are context-sensitive - select the right building first.
- **"Persistent assignment reset after loading"**: this is expected. Re-apply Ctrl+A after each load (see Known Limitations in the README).
