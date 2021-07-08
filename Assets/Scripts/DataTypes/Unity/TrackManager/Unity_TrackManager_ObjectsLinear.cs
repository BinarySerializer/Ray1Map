using System.Linq;
using BinarySerializer;

using UnityEngine;

namespace R1Engine
{
    public class Unity_TrackManager_ObjectsLinear : Unity_TrackManager
    {
        public Unity_TrackManager_ObjectsLinear(bool isReversed)
        {
            IsReversed = isReversed;
        }

        public bool IsReversed { get; }

        protected override float Height => 0f;
        public override bool Loop => false;

        public override bool IsAvailable(Context context, Unity_Level level) => GetSortedObjects(level).Length >= 2;

		protected override Vector3[] GetPoints(Unity_Level level) 
        {
            var sortedObjects = GetSortedObjects(level);

            var first = GetFixedHeightPos(sortedObjects.First().Position);
            var last = GetFixedHeightPos(sortedObjects.Last().Position);

            first = new Vector3(first.x, first.y - 50, first.z);

            return new Vector3[]
            {
                IsReversed ? last : first,
                IsReversed ? first : last,
            };
        }

        protected Unity_Object_3D[] GetSortedObjects(Unity_Level level) => level.EventData.OfType<Unity_Object_3D>().OrderBy(x => x.Position.y).ToArray();
        protected Vector3 GetFixedHeightPos(Vector3 v) => new Vector3(v.x, v.y, 0);
    }
}