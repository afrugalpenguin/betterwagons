using System.Collections.Generic;

namespace BetterWagons.Helpers
{
    public static class TierHelper
    {
        private static readonly Dictionary<int, int> _upgradeCounts = new Dictionary<int, int>();

        public static void RegisterUpgrade(int buildingInstanceId)
        {
            if (_upgradeCounts.ContainsKey(buildingInstanceId))
                _upgradeCounts[buildingInstanceId]++;
            else
                _upgradeCounts[buildingInstanceId] = 1;
        }

        public static int GetTier(int buildingInstanceId)
        {
            if (_upgradeCounts.TryGetValue(buildingInstanceId, out int count))
                return System.Math.Min(count + 1, 4);
            return 1;
        }

        public static int GetTier(WagonShop shop)
        {
            if (shop == null) return 1;
            return GetTier(shop.gameObject.GetInstanceID());
        }

        public static void Clear()
        {
            _upgradeCounts.Clear();
        }
    }
}
