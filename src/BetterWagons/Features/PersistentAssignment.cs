using System.Collections.Generic;
using UnityEngine;
using MelonLoader;

namespace BetterWagons.Features
{
    public static class PersistentAssignment
    {
        private static readonly Dictionary<int, int> _assignments = new Dictionary<int, int>();
        private static readonly Dictionary<int, GameObject> _assignedBuildings = new Dictionary<int, GameObject>();

        public static void SetAssignment(TransportWagon wagon, GameObject building)
        {
            if (wagon == null) return;
            int wagonId = wagon.gameObject.GetInstanceID();
            if (building == null)
            {
                _assignments.Remove(wagonId);
                _assignedBuildings.Remove(wagonId);
                MelonLogger.Msg($"[BetterWagons] Cleared persistent assignment for wagon {wagonId}");
            }
            else
            {
                _assignments[wagonId] = building.GetInstanceID();
                _assignedBuildings[wagonId] = building;
                MelonLogger.Msg($"[BetterWagons] Wagon {wagonId} persistently assigned to {building.name}");
            }
        }

        public static void ToggleAssignment(TransportWagon wagon, GameObject building)
        {
            if (wagon == null || building == null) return;
            int wagonId = wagon.gameObject.GetInstanceID();
            if (_assignments.TryGetValue(wagonId, out int existingId) && existingId == building.GetInstanceID())
                SetAssignment(wagon, null);
            else
                SetAssignment(wagon, building);
        }

        public static bool HasAssignment(TransportWagon wagon)
        {
            if (wagon == null) return false;
            return _assignments.ContainsKey(wagon.gameObject.GetInstanceID());
        }

        public static GameObject GetAssignedBuilding(TransportWagon wagon)
        {
            if (wagon == null) return null;
            int wagonId = wagon.gameObject.GetInstanceID();
            if (_assignedBuildings.TryGetValue(wagonId, out GameObject building))
            {
                if (building == null)
                {
                    _assignments.Remove(wagonId);
                    _assignedBuildings.Remove(wagonId);
                    return null;
                }
                return building;
            }
            return null;
        }

        public static bool IsAssignedTo(TransportWagon wagon, GameObject building)
        {
            if (wagon == null || building == null) return false;
            int wagonId = wagon.gameObject.GetInstanceID();
            return _assignments.TryGetValue(wagonId, out int buildingId)
                && buildingId == building.GetInstanceID();
        }

        public static void Clear()
        {
            _assignments.Clear();
            _assignedBuildings.Clear();
        }
    }
}
