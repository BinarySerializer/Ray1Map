using System;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Common level data
    /// </summary>
    public class Common_Lev
    {
        // TODO: Abstract this more as PC version handles tiles differently
        public Texture2D TileSet { get; set; }

        public ushort Width { get; set; }

        public ushort Height { get; set; }

        public Type[] Tiles { get; set; }

        public Event[] Events { get; set; }


        // TODO: Remove?
        public PxlVec RaymanPos { get; set; }

        public Type TypeFromCoord(Vector3 pos)
        {
            int i = (int)(Math.Floor(-pos.y) * Width + Math.Floor(pos.x));

            if (i < Tiles.Length && i >= 0) 
                return Tiles[i];

            else return new Type();
        }
    }
}