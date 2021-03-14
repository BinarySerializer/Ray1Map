using System;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_TrackManager_GBAVV_NitroKart : Unity_TrackManager
    {
        protected Unity_Object_GBAVVNitroKartWaypoint CurrentWaypoint { get; set; }
        public Vector3? PrevDirection { get; set; }

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
            // Get the direction to move in
            var dir = (GetWaypointPos(CurrentWaypoint) - pos).normalized;

            // Check if we've moved past the waypoint based on if we're moving backwards from the previous direction
            if (PrevDirection != null &&
                DifferentSignOr0(dir.x, PrevDirection.Value.x) &&
                DifferentSignOr0(dir.y, PrevDirection.Value.y) &&
                DifferentSignOr0(dir.z, PrevDirection.Value.z))
            {
                // Update the waypoint to the next one
                NextWaypoint(level);

                // Get a new direction
                dir = (GetWaypointPos(CurrentWaypoint) - pos).normalized;
            }

            // Set the previous direction
            PrevDirection = dir;

            return dir;
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

        public bool DifferentSignOr0(float v1, float v2)
        {
            return Math.Sign(v1) != Math.Sign(v2) || v1 == 0 || v2 == 0;
        }
    }
}