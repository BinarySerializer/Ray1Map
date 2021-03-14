using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_TrackManager_GBAVV_NitroKart : Unity_TrackManager
    {
        protected Unity_Object_GBAVVNitroKartWaypoint CurrentWaypoint { get; set; }

        public override bool IsAvailable(Context context, Unity_Level level)
        {
            return level.EventData.OfType<Unity_Object_GBAVVNitroKartWaypoint>().Any();
        }

        public override Vector3 GetStartPosition(Unity_Level level)
        {
            // Get the first waypoint
            var obj = level.EventData.OfType<Unity_Object_GBAVVNitroKartWaypoint>().First();

            // Set the next waypoint
            NextWaypoint(level, obj);

            // Return position
            return GetWaypointPos(obj);
        }

        public override Vector3 GetDirection(Unity_Level level, Vector3 pos)
        {
            if (Vector3.Angle(pos, GetWaypointPos(CurrentWaypoint)) == 0)
                NextWaypoint(level);

            return (GetWaypointPos(CurrentWaypoint) - pos).normalized;
        }

        public override bool HasReachedEnd(Unity_Level level, Vector3 pos) => false;

        public void NextWaypoint(Unity_Level level, Unity_Object_GBAVVNitroKartWaypoint current = null)
        {
            CurrentWaypoint = (Unity_Object_GBAVVNitroKartWaypoint)level.EventData[(current ?? CurrentWaypoint).LinkedWayPointIndex.Value];
        }

        public Vector3 GetWaypointPos(Unity_Object_GBAVVNitroKartWaypoint obj)
        {
            var pos = obj.Position;
            pos.z += 10;
            return pos;
        }
    }
}