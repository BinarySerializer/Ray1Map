using BinarySerializer;
using BinarySerializer.Klonoa.DTP;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ray1Map.PSKlonoa
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

                    points.Add(lastPoint + PSKlonoaHelpers.GetPosition(frame.RelativePositionX, frame.RelativePositionY, frame.RelativePositionZ, Scale));
                }
                else
                {
                    points.Add(PSKlonoaHelpers.GetPosition(frame.AbsolutePositionX, frame.AbsolutePositionY, frame.AbsolutePositionZ, Scale));
                }
            }

            return points.ToArray();
        }

        protected override Quaternion[] GetRotations(Unity_Level level)
        {
            var rotations = new List<Quaternion>();
            int prevRotX = 0;
            int prevRotY = 0;

            Quaternion GetQuaternion(float rotX, float rotY) {
                return Quaternion.Euler(
                    PSKlonoaHelpers.GetRotationInDegrees(rotX),
                    -PSKlonoaHelpers.GetRotationInDegrees(rotY), 0f);
            }

            foreach (CameraAnimationFrame frame in Animation.Frames)
            {
                if (frame.Pre_IsRelative)
                {
                    rotations.Add(GetQuaternion(frame.RelativeRotationX + prevRotX, frame.RelativeRotationY + prevRotY));

                    prevRotX += frame.RelativeRotationX;
                    prevRotY += frame.RelativeRotationY;
                }
                else
                {
                    rotations.Add(GetQuaternion(frame.AbsoluteRotationX, frame.AbsoluteRotationY));

                    prevRotX = frame.AbsoluteRotationX;
                    prevRotY = frame.AbsoluteRotationY;
                }
            }

            return rotations.ToArray();
        }

        public override bool IsAvailable(Context context, Unity_Level level) => true;
    }
}