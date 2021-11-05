using BinarySerializer.Klonoa.DTP;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public class KlonoaBackgroundLayer
    {
        public KlonoaBackgroundLayer(BackgroundGameObject obj, Texture2D[] frames, int speed)
        {
            Object = obj;
            Frames = frames;
            Speed = speed;
        }

        public BackgroundGameObject Object { get; }
        public Texture2D[] Frames { get; }
        public int Speed { get; }
    }
}