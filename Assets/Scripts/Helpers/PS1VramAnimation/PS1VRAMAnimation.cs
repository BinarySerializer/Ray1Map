using System.Collections.Generic;
using System.Linq;
using BinarySerializer.PS1;
using UnityEngine;

namespace R1Engine
{
    public class PS1VRAMAnimation
    {
        public PS1VRAMAnimation(RectInt region, byte[][] frames, int speed, bool pingPong)
        {
            UsesSingleRegion = true;
            Region = region;
            Frames = frames;
            Speed = speed;
            PingPong = pingPong;
        }
        public PS1VRAMAnimation(PS1_VRAMRegion region, byte[][] frames, int speed, bool pingPong)
        {
            UsesSingleRegion = true;
            Region = RectIntFromVRAMRegion(region);
            Frames = frames;
            Speed = speed;
            PingPong = pingPong;
        }
        public PS1VRAMAnimation(RectInt[] regions, byte[][] frames, int speed, bool pingPong)
        {
            UsesSingleRegion = false;
            Regions = regions;
            Frames = frames;
            Speed = speed;
            PingPong = pingPong;
        }
        public PS1VRAMAnimation(IEnumerable<PS1_VRAMRegion> regions, byte[][] frames, int speed, bool pingPong)
        {
            UsesSingleRegion = false;
            Regions = regions.Select(RectIntFromVRAMRegion).ToArray();
            Frames = frames;
            Speed = speed;
            PingPong = pingPong;
        }
        public PS1VRAMAnimation(PS1_TIM[] timFiles, int speed, bool pingPong)
        {
            UsesSingleRegion = false;
            Regions = timFiles.Select(x => RectIntFromVRAMRegion(x.Region)).ToArray();
            Frames = timFiles.Select(x => x.ImgData).ToArray();
            Speed = speed;
            PingPong = pingPong;
        }

        public bool UsesSingleRegion { get; }
        public RectInt Region { get; }
        public RectInt[] Regions { get; }
        public byte[][] Frames { get; }
        public int Speed { get; }
        public bool PingPong { get; }
        private int? _key;
        public int Key => _key ??= Speed | (PingPong ? 1 : 0) << 15 | (Frames.Length << 16);

        public int ActualLength => PingPong ? Frames.Length + (Frames.Length - 2) : Frames.Length;

        private static RectInt RectIntFromVRAMRegion(PS1_VRAMRegion region) => new RectInt(region.XPos * 2, region.YPos, region.Width * 2, region.Height);

        public bool Overlaps(RectInt region)
        {
            if (UsesSingleRegion)
                return Region.Overlaps(region);
            else
                return Regions.Any(x => x.Overlaps(region));
        }
    }
}