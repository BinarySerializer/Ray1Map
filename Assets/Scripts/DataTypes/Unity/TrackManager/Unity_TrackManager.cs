using Cinemachine;
using R1Engine.Serialize;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public abstract class Unity_TrackManager {
        public abstract bool IsAvailable(Context context, Unity_Level level);

        protected abstract Vector3[] GetPoints(Unity_Level level);
        public abstract bool Loop { get; }
        protected abstract float Height { get; }
        protected virtual float Lookahead { get; } = 5f;
        protected virtual int Resolution { get; } = 20;
        protected CinemachinePathBase CinemachinePath { get; set; }
        public float TrackLength { get; protected set; }
        public virtual bool UseBezierPath => true;

        public void CreatePath(Unity_Level level) {
            Vector3[] points = GetPoints(level);
            Vector3 isometricScale = level.IsometricData.AbsoluteObjectScale;
            var transformedPoints = points.Select(p => Vector3.Scale(new Vector3(p.x, p.z + Height, -p.y), isometricScale));
            GameObject path = new GameObject("Path");
            if (UseBezierPath) {
                var bezierPath = path.AddComponent<CinemachineSmoothPath>();
                bezierPath.m_Resolution = Resolution;
                if (Loop) bezierPath.m_Looped = true;
                bezierPath.m_Waypoints = transformedPoints.Select(p => new CinemachineSmoothPath.Waypoint() { position = p }).ToArray();
                CinemachinePath = bezierPath;
            } else {
                var regularPath = path.AddComponent<CinemachinePath>();
                regularPath.m_Resolution = Resolution;
                if (Loop) regularPath.m_Looped = true;
                regularPath.m_Waypoints = transformedPoints.Select(p => new CinemachinePath.Waypoint() { position = p }).ToArray();
                CinemachinePath = regularPath;
            }

            TrackLength = CinemachinePath.PathLength;
        }
        public Vector3 GetPointAtDistance(Unity_Level level, float distance) {
            if(CinemachinePath == null) CreatePath(level);
            return CinemachinePath.EvaluatePositionAtUnit(distance, CinemachinePathBase.PositionUnits.Distance);
        }
        public Quaternion GetRotationAtDistance(Unity_Level level, float distance) {
            if (CinemachinePath == null) CreatePath(level);
            if (UseBezierPath) {
                Quaternion rot0 = CinemachinePath.EvaluateOrientationAtUnit(distance + Lookahead, CinemachinePathBase.PositionUnits.Distance);
                Quaternion rot1 = CinemachinePath.EvaluateOrientationAtUnit(distance + Lookahead * 0.75f, CinemachinePathBase.PositionUnits.Distance);
                Quaternion rot2 = CinemachinePath.EvaluateOrientationAtUnit(distance + Lookahead * 1.5f, CinemachinePathBase.PositionUnits.Distance);
                return Quaternion.Lerp(Quaternion.Lerp(rot0, rot1, 0.5f), Quaternion.Lerp(rot1, rot2, 0.5f), 0.5f);
            } else {
                return CinemachinePath.EvaluateOrientationAtUnit(distance + Lookahead, CinemachinePathBase.PositionUnits.Distance);
            }
        }
    }
}