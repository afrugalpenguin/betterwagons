using MelonLoader;

namespace BetterWagons
{
    public static class ModConfig
    {
        private static MelonPreferences_Category _category;

        // Tier 1
        public static MelonPreferences_Entry<float> Tier1CostMultiplier;

        // Tier 2
        public static MelonPreferences_Entry<float> Tier2CostMultiplier;
        public static MelonPreferences_Entry<int> Tier2BonusWorkers;
        public static MelonPreferences_Entry<float> Tier2CapacityMultiplier;
        public static MelonPreferences_Entry<float> Tier2RadiusMultiplier;

        // Tier 3
        public static MelonPreferences_Entry<int> Tier3BonusWorkers;
        public static MelonPreferences_Entry<float> Tier3CapacityMultiplier;
        public static MelonPreferences_Entry<float> Tier3RadiusMultiplier;

        // Tier 4
        public static MelonPreferences_Entry<float> Tier4CostMultiplier;
        public static MelonPreferences_Entry<int> Tier4BonusWorkers;
        public static MelonPreferences_Entry<float> Tier4CapacityMultiplier;
        public static MelonPreferences_Entry<float> Tier4RadiusMultiplier;
        public static MelonPreferences_Entry<float> Tier4StorageCapacity;

        // Global
        public static MelonPreferences_Entry<float> WagonSpeedMultiplier;
        public static MelonPreferences_Entry<float> DesirabilityPenaltyMultiplier;

        // Logistics
        public static MelonPreferences_Entry<float> MinWeightForBulkTransport;
        public static MelonPreferences_Entry<float> MinPercToTriggerDeliver;
        public static MelonPreferences_Entry<float> MinPercToRequestDelivered;

        public static void Initialize()
        {
            _category = MelonPreferences.CreateCategory("BetterWagons", "Better Wagons");

            // Tier 1
            Tier1CostMultiplier = _category.CreateEntry("Tier1CostMultiplier", 0.8f,
                "Tier 1 Cost Multiplier",
                "Multiplier applied to Tier 1 trading post construction costs");

            // Tier 2
            Tier2CostMultiplier = _category.CreateEntry("Tier2CostMultiplier", 0.85f,
                "Tier 2 Cost Multiplier",
                "Multiplier applied to Tier 2 trading post construction costs");
            Tier2BonusWorkers = _category.CreateEntry("Tier2BonusWorkers", 1,
                "Tier 2 Bonus Workers",
                "Additional worker slots granted at Tier 2");
            Tier2CapacityMultiplier = _category.CreateEntry("Tier2CapacityMultiplier", 1.3f,
                "Tier 2 Capacity Multiplier",
                "Multiplier applied to wagon carry capacity at Tier 2");
            Tier2RadiusMultiplier = _category.CreateEntry("Tier2RadiusMultiplier", 1.25f,
                "Tier 2 Radius Multiplier",
                "Multiplier applied to trading post service radius at Tier 2");

            // Tier 3
            Tier3BonusWorkers = _category.CreateEntry("Tier3BonusWorkers", 2,
                "Tier 3 Bonus Workers",
                "Additional worker slots granted at Tier 3");
            Tier3CapacityMultiplier = _category.CreateEntry("Tier3CapacityMultiplier", 1.5f,
                "Tier 3 Capacity Multiplier",
                "Multiplier applied to wagon carry capacity at Tier 3");
            Tier3RadiusMultiplier = _category.CreateEntry("Tier3RadiusMultiplier", 1.4f,
                "Tier 3 Radius Multiplier",
                "Multiplier applied to trading post service radius at Tier 3");

            // Tier 4
            Tier4CostMultiplier = _category.CreateEntry("Tier4CostMultiplier", 1.5f,
                "Tier 4 Cost Multiplier",
                "Multiplier applied to Tier 4 trading post construction costs");
            Tier4BonusWorkers = _category.CreateEntry("Tier4BonusWorkers", 5,
                "Tier 4 Bonus Workers",
                "Additional worker slots granted at Tier 4");
            Tier4CapacityMultiplier = _category.CreateEntry("Tier4CapacityMultiplier", 1.6f,
                "Tier 4 Capacity Multiplier",
                "Multiplier applied to wagon carry capacity at Tier 4");
            Tier4RadiusMultiplier = _category.CreateEntry("Tier4RadiusMultiplier", 1.5f,
                "Tier 4 Radius Multiplier",
                "Multiplier applied to trading post service radius at Tier 4");
            Tier4StorageCapacity = _category.CreateEntry("Tier4StorageCapacity", 500f,
                "Tier 4 Storage Capacity",
                "Maximum storage capacity for Tier 4 trading posts");

            // Global
            WagonSpeedMultiplier = _category.CreateEntry("WagonSpeedMultiplier", 1.1f,
                "Wagon Speed Multiplier",
                "Global multiplier applied to wagon movement speed");
            DesirabilityPenaltyMultiplier = _category.CreateEntry("DesirabilityPenaltyMultiplier", 0.7f,
                "Desirability Penalty Multiplier",
                "Multiplier applied to trading post desirability penalties");

            // Logistics
            MinWeightForBulkTransport = _category.CreateEntry("MinWeightForBulkTransport", 600f,
                "Min Weight for Bulk Transport",
                "Minimum total weight before bulk transport mode is triggered");
            MinPercToTriggerDeliver = _category.CreateEntry("MinPercToTriggerDeliver", 0.8f,
                "Min Percent to Trigger Delivery",
                "Minimum storage fill percentage to trigger outgoing delivery");
            MinPercToRequestDelivered = _category.CreateEntry("MinPercToRequestDelivered", 0.3f,
                "Min Percent to Request Delivery",
                "Minimum storage fill percentage below which incoming delivery is requested");
        }
    }
}
