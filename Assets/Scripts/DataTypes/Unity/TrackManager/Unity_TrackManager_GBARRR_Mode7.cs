using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_TrackManager_GBARRR_Mode7 : Unity_TrackManager_TargetBase
    {
        protected Unity_Object_GBARRRMode7Waypoint CurrentWaypoint { get; set; }

        public override bool IsAvailable(Context context, Unity_Level level)
        {
            // Make sure the level has waypoints
            return level.EventData.OfType<Unity_Object_GBARRRMode7Waypoint>().Any();
        }

        public override Vector3 GetStartPosition(Unity_Level level)
        {
            // Get the first waypoint
            var obj = level.EventData.OfType<Unity_Object_GBARRRMode7Waypoint>().First();

            CurrentWaypoint = obj;

            // Set the next waypoint
            NextTarget(level);

            // Return position
            return GetPosition(obj.Position);
        }

        public override void NextTarget(Unity_Level level)
        {
            CurrentWaypoint = (Unity_Object_GBARRRMode7Waypoint) level.EventData[CurrentWaypoint.LinkedWayPointIndex];
            TargetPosition = CurrentWaypoint.Position;
        }
    }
}