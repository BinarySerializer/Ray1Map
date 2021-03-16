using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_TrackManager_GBARRR_Mode7 : Unity_TrackManager
    {
        protected Unity_Object_GBARRRMode7Waypoint CurrentWaypoint { get; set; }

		protected override float Height => 10f;

		public override bool Loop => true;

		public override bool IsAvailable(Context context, Unity_Level level)
        {
            // Make sure the level has waypoints
            return level.EventData.OfType<Unity_Object_GBARRRMode7Waypoint>().Any();
        }

		protected override Vector3[] GetPoints(Unity_Level level) {
            // Get the first waypoint
            List<Unity_Object_GBARRRMode7Waypoint> wps = new List<Unity_Object_GBARRRMode7Waypoint>();
            var obj = level.EventData.OfType<Unity_Object_GBARRRMode7Waypoint>().First();
            var firstObj = obj;
            Unity_Object_GBARRRMode7Waypoint lastWP = null;
            while (wps.Count == 0 || obj != firstObj) {
                var nextObj = (Unity_Object_GBARRRMode7Waypoint)level.EventData[obj.LinkedWayPointIndex];
                if (lastWP == null || Vector3.Distance(lastWP.Position, obj.Position) > 8*8f) {
                    wps.Add(obj);
                    lastWP = obj;
                }
                obj = nextObj;
            }
            return wps.Select(wp => wp.Position).ToArray();
        }
	}
}