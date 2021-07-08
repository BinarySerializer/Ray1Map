using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Unity_TrackManager_Linear : Unity_TrackManager
    {
        public Unity_TrackManager_Linear(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }

        public Vector3 Start { get; }
        public Vector3 End { get; }

        protected override float Height => 0f;
        public override bool Loop => false;

        public override bool IsAvailable(Context context, Unity_Level level) => true;

        protected override Vector3[] GetPoints(Unity_Level level) 
        {
            return new Vector3[]
            {
                Start,
                End
            };
        }
    }
}