using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public abstract class Unity_TrackManager
    {
        public abstract bool IsAvailable(Context context, Unity_Level level);
        public abstract Vector3 GetStartPosition(Unity_Level level);
        public abstract Vector3 GetDirection(Unity_Level level, Vector3 pos);
        public abstract bool HasReachedEnd(Unity_Level level, Vector3 pos);
    }
}