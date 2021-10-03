using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Klonoa.DTP;
using UnityEngine;

namespace R1Engine
{
    public class Unity_TrackManager_PSKlonoaDTP : Unity_TrackManager
    {
        public Unity_TrackManager_PSKlonoaDTP(CameraAnimations_File animation, float scale)
        {
            Animation = animation;
            Scale = scale;
        }

        public CameraAnimations_File Animation { get; }
        public float Scale { get; }

        public override bool Loop => false;
        
        protected override Vector3[] GetPoints(Unity_Level level)
        {
            var points = new List<Vector3>();

            foreach (CameraAnimationFrame frame in Animation.Frames)
            {
                if (frame.Pre_IsRelative)
                {
                    var lastPoint = points.LastOrDefault();

                    points.Add(lastPoint + PSKlonoa_DTP_BaseManager.GetPosition(frame.RelativePositionX, frame.RelativePositionY, frame.RelativePositionZ, Scale));
                }
                else
                {
                    points.Add(PSKlonoa_DTP_BaseManager.GetPosition(frame.AbsolutePositionX, frame.AbsolutePositionY, frame.AbsolutePositionZ, Scale));
                }
            }

            return points.ToArray();
        }

        protected override Quaternion[] GetRotations(Unity_Level level)
        {
            var rotations = new List<Quaternion>();
            int prevRotX = 0;
            int prevRotY = 0;

            foreach (CameraAnimationFrame frame in Animation.Frames)
            {
                if (frame.Pre_IsRelative)
                {
                    rotations.Add(PSKlonoa_DTP_BaseManager.GetQuaternion(frame.RelativeRotationX + prevRotX, frame.RelativeRotationY + prevRotY, 0));

                    prevRotX += frame.RelativeRotationX;
                    prevRotY += frame.RelativeRotationY;
                }
                else
                {
                    rotations.Add(PSKlonoa_DTP_BaseManager.GetQuaternion(frame.AbsoluteRotationX, frame.AbsoluteRotationY, 0));

                    prevRotX = frame.AbsoluteRotationX;
                    prevRotY = frame.AbsoluteRotationY;
                }
            }

            return rotations.ToArray();
        }

        public override bool IsAvailable(Context context, Unity_Level level) => true;
    }
}