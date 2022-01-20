using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Level3D_MapCollision : BinarySerializable
    {
        public uint ItemsCount { get; set; }
        public uint LinesCount { get; set; }
        public GBAIsometric_Ice_Level3D_MapCollisionBox[] Items { get; set; }

        // Below data isn't needed to reconstruct the collision as it's mainly lookup data for the game
        public uint UnkWidth { get; set; } // Fixed point?
        public uint UnkHeight { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public byte[] CountsMap { get; set; } // Determines the number of collision items at this specific cell
        public ushort[] IndexMap { get; set; } // The collision items indexes for the cells

        public override void SerializeImpl(SerializerObject s)
        {
            ItemsCount = s.Serialize<uint>(ItemsCount, name: nameof(ItemsCount));
            LinesCount = s.Serialize<uint>(LinesCount, name: nameof(LinesCount));
            Items = s.SerializeObjectArray<GBAIsometric_Ice_Level3D_MapCollisionBox>(Items, ItemsCount, name: nameof(Items));

            foreach (GBAIsometric_Ice_Level3D_MapCollisionBox d in Items)
                d.Lines = s.SerializeObjectArray<GBAIsometric_Ice_Level3D_MapCollisionLine>(d.Lines, d.LinesDataLength / 8, name: nameof(d.Lines));

            UnkWidth = s.Serialize<uint>(UnkWidth, name: nameof(UnkWidth));
            UnkHeight = s.Serialize<uint>(UnkHeight, name: nameof(UnkHeight));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            CountsMap = s.SerializeArray<byte>(CountsMap, Width * Height, name: nameof(CountsMap));
            IndexMap = s.SerializeArray<ushort>(IndexMap, CountsMap.Sum(x => x), name: nameof(IndexMap));
        }
    }
}