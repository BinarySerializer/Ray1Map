using System;
using Ray1Map;
using Ray1Map.PSKlonoa;
using UnityEngine;

public class KlonoaDTPConstantRotationComponent : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        if (Controller.LoadState != Controller.State.Finished || !Settings.AnimateTiles) 
            return;

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
            case RotationAxis.X: rotX = value; break;
            case RotationAxis.Y: rotY = value; break;
            case RotationAxis.Z: rotZ = value; break;
        }

        animatedTransform.localRotation = initialRotation * PSKlonoaHelpers.GetQuaternion(rotX, rotY, rotZ);
    }

    public enum RotationAxis
    {
        X, Y, Z,
    }
}