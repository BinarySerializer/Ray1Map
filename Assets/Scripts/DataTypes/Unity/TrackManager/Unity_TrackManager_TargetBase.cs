using System;
using UnityEngine;

namespace R1Engine
{
    public abstract class Unity_TrackManager_TargetBase : Unity_TrackManager
    {
        protected Vector3 TargetPosition { get; set; }
        protected Vector3? PrevDirection { get; set; }

        public override Vector3 GetDirection(Unity_Level level, Vector3 pos)
        {
            // Get the direction to move in
            var dir = (GetPosition(TargetPosition) - pos).normalized;

            // Check if we've moved past the target based on if we're moving backwards from the previous direction
            if (PrevDirection != null &&
                DifferentSignOr0(dir.x, PrevDirection.Value.x) &&
                DifferentSignOr0(dir.y, PrevDirection.Value.y) &&
                DifferentSignOr0(dir.z, PrevDirection.Value.z))
            {
                // Update the target to the next one
                NextTarget(level);

                // Get a new direction
                dir = (GetPosition(TargetPosition) - pos).normalized;
            }

            // Set the previous direction
            PrevDirection = dir;

            return dir;
        }

        public override bool HasReachedEnd(Unity_Level level, Vector3 pos) => false;

        public abstract void NextTarget(Unity_Level level);

        public virtual Vector3 GetPosition(Vector3 pos)
        {
            pos.z += 10;
            return pos;
        }

        public bool DifferentSignOr0(float v1, float v2)
        {
            return Math.Sign(v1) != Math.Sign(v2) || v1 == 0 || v2 == 0;
        }
    }
}