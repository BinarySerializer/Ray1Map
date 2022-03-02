using UnityEngine;

namespace Ray1Map
{
    public interface IUnity_Texture
    {
        int Width { get; }
        int Height { get; }

        Color[] GetColors();
    }
}