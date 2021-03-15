﻿using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_TrackManager_GBAVV_NitroKart : Unity_TrackManager_TargetBase
    {
        protected Unity_Object_GBAVVNitroKartWaypoint CurrentWaypoint { get; set; }

        public override bool IsAvailable(Context context, Unity_Level level)
        {
            // Make sure the level has waypoints
            return level.EventData.OfType<Unity_Object_GBAVVNitroKartWaypoint>().Any();
        }

        public override Vector3 GetStartPosition(Unity_Level level)
        {
            // Get the first waypoint
            var obj = level.EventData.OfType<Unity_Object_GBAVVNitroKartWaypoint>().First();

            CurrentWaypoint = obj;

            // Set the next waypoint
            NextTarget(level);

            // Return position
            return GetPosition(obj.Position);
        }

        public override void NextTarget(Unity_Level level)
        {
            CurrentWaypoint = (Unity_Object_GBAVVNitroKartWaypoint) level.EventData[CurrentWaypoint.LinkedWayPointIndex.Value];
            TargetPosition = CurrentWaypoint.Position;
        }
    }
}