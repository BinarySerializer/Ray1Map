using Ray1Map.PSKlonoa;
using System;
using UnityEngine;

namespace Ray1Map
{
    public class KlonoaDTPConstantRotationComponent : ObjectAnimationComponent
    {
        public Transform animatedTransform;
        public Quaternion initialRotation;
        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotationSpeed = 1;
        public RotationAxis axis;
        public float minValue = -0x800;
        public float length = 0x1000;

        protected override void UpdateAnimation()
        {
            var value = axis switch
            {
                RotationAxis.X => rotX,
                RotationAxis.Y => rotY,
                RotationAxis.Z => rotZ,
                _ => throw new Exception(),
            };

            value += rotationSpeed * Time.deltaTime * LevelEditorData.FramesPerSecond;

            if (value > minValue + length)
                value = minValue;
            else if (value < minValue)
                value = minValue + length;

            switch (axis)
            {
                case RotationAxis.X:
                    rotX = value;
                    break;
                case RotationAxis.Y:
                    rotY = value;
                    break;
                case RotationAxis.Z:
                    rotZ = value;
                    break;
            }

            animatedTransform.localRotation = initialRotation * KlonoaHelpers.GetQuaternion(rotX, rotY, rotZ);
        }

        public enum RotationAxis
        {
            X,
            Y,
            Z,
        }
    }
}