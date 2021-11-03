using UnityEngine;

namespace Ray1Map
{
    public class Unity_CameraClear
    {
        public Unity_CameraClear(Color solidColor)
        {
            SolidColor = solidColor;
        }

        public Color SolidColor { get; }

        public void Apply()
        {
            Camera.main.backgroundColor = SolidColor;
        }
    }
}