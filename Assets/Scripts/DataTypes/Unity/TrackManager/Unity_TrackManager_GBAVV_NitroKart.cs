using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

using UnityEngine;

namespace R1Engine
{
    public class Unity_TrackManager_GBAVV_NitroKart : Unity_TrackManager
    {
		public override bool Loop => true;

		protected override float Height => 10f;

		public override bool IsAvailable(Context context, Unity_Level level)
        {
            // Make sure the level has waypoints
            return level.EventData.OfType<Unity_Object_GBAVVNitroKartWaypoint>().Any();
        }

		protected override Vector3[] GetPoints(Unity_Level level) {
            // Get the first waypoint
            List<Unity_Object_GBAVVNitroKartWaypoint> wps = new List<Unity_Object_GBAVVNitroKartWaypoint>();
            var obj = level.EventData.OfType<Unity_Object_GBAVVNitroKartWaypoint>().First();
            var firstObj = obj;
            Unity_Object_GBAVVNitroKartWaypoint lastWP = null;
            while (wps.Count == 0 || obj != firstObj) {
                var nextObj = (Unity_Object_GBAVVNitroKartWaypoint)level.EventData[obj.LinkedWayPointIndex.Value];
                if (lastWP == null || Vector3.Distance(lastWP.Position, obj.Position) > 8 * 8f) {
                    wps.Add(obj);
                    lastWP = obj;
                }
                obj = nextObj;
            }
            return wps.Select(wp => wp.Position).ToArray();
        }
	}
}