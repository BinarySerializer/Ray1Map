using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBACrash_Isometric_LevelInfo : R1Serializable
    {
        public bool SerializeData { get; set; } // Set before serializing

        public Pointer NamePointer { get; set; }

        public ushort MapWidth { get; set; }
        public ushort MapHeight { get; set; }
        public ushort TileSetCount_Total { get; set; }
        public ushort TileSetCount_4bpp { get; set; }
        public Pointer TileSetPointer { get; set; }
        public Pointer Pointer_10 { get; set; }
        public Pointer[] MapLayerPointers { get; set; }
        public Pointer TilePalettePointer { get; set; }

        public ushort UnkWidth { get; set; }
        public ushort UnkHeight { get; set; }
        public Pointer Pointer_2C { get; set; } // Ushort array, UnkWidth * UnkHeight
        public Pointer Pointer_30 { get; set; } // Pointer_2C ushorts reference 12 byte structs here
        public Pointer Pointer_34 { get; set; } // Structs of size 0x28 with function pointers, referenced from Pointer_30 structs
        public Pointer Pointer_38 { get; set; }
        public Pointer Pointer_3C { get; set; }
        public Pointer Pointer_40 { get; set; }
        public Pointer Pointer_44 { get; set; }
        public Pointer Pointer_48 { get; set; }
        public Pointer Pointer_4C { get; set; }
        public uint UnkXPos { get; set; }
        public uint UnkYPos { get; set; }

        // Serialized from pointers

        public string Name { get; set; }
        
        public GBACrash_Isometric_TileSet TileSet { get; set; }
        public GBACrash_Isometric_MapLayer[] MapLayers { get; set; }
        public RGBA5551Color[] TilePalette { get; set; }
        
        public ushort[] Pointer_2C_Data { get; set; }
        public GBACrash_Isometric_UnkStruct_1[] Pointer_30_Structs { get; set; }
        public GBACrash_Isometric_UnkStruct_2[] Pointer_34_Structs { get; set; }
        public GBACrash_Isometric_UnkStruct_0[] Pointer_4C_Structs { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            SerializeData = true; // TODO: Remove

            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));
            MapWidth = s.Serialize<ushort>(MapWidth, name: nameof(MapWidth));
            MapHeight = s.Serialize<ushort>(MapHeight, name: nameof(MapHeight));
            TileSetCount_Total = s.Serialize<ushort>(TileSetCount_Total, name: nameof(TileSetCount_Total));
            TileSetCount_4bpp = s.Serialize<ushort>(TileSetCount_4bpp, name: nameof(TileSetCount_4bpp));
            TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));
            Pointer_10 = s.SerializePointer(Pointer_10, name: nameof(Pointer_10));
            MapLayerPointers = s.SerializePointerArray(MapLayerPointers, 4, name: nameof(MapLayerPointers));
            TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
            UnkWidth = s.Serialize<ushort>(UnkWidth, name: nameof(UnkWidth));
            UnkHeight = s.Serialize<ushort>(UnkHeight, name: nameof(UnkHeight));
            Pointer_2C = s.SerializePointer(Pointer_2C, name: nameof(Pointer_2C));
            Pointer_30 = s.SerializePointer(Pointer_30, name: nameof(Pointer_30));
            Pointer_34 = s.SerializePointer(Pointer_34, name: nameof(Pointer_34));
            Pointer_38 = s.SerializePointer(Pointer_38, name: nameof(Pointer_38));
            Pointer_3C = s.SerializePointer(Pointer_3C, name: nameof(Pointer_3C));
            Pointer_40 = s.SerializePointer(Pointer_40, name: nameof(Pointer_40));
            Pointer_44 = s.SerializePointer(Pointer_44, name: nameof(Pointer_44));
            Pointer_48 = s.SerializePointer(Pointer_48, name: nameof(Pointer_48));
            Pointer_4C = s.SerializePointer(Pointer_4C, name: nameof(Pointer_4C));
            UnkXPos = s.Serialize<uint>(UnkXPos, name: nameof(UnkXPos));
            UnkYPos = s.Serialize<uint>(UnkYPos, name: nameof(UnkYPos));

            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));

            if (!SerializeData)
                return;

            TileSet = s.DoAt(TileSetPointer, () => s.SerializeObject<GBACrash_Isometric_TileSet>(TileSet, x =>
            {
                x.TileSetCount_Total = TileSetCount_Total;
                x.TileSetCount_4bpp = TileSetCount_4bpp;
            }, name: nameof(TileSet)));

            if (MapLayers == null)
                MapLayers = new GBACrash_Isometric_MapLayer[MapLayerPointers.Length];

            for (int i = 0; i < MapLayers.Length; i++)
                MapLayers[i] = s.DoAt(MapLayerPointers[i], () => s.SerializeObject<GBACrash_Isometric_MapLayer>(MapLayers[i], name: $"{nameof(MapLayers)}[{i}]"));

            TilePalette = s.DoAt(TilePalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(TilePalette, 256, name: nameof(TilePalette)));

            Pointer_2C_Data = s.DoAt(Pointer_2C, () => s.SerializeArray<ushort>(Pointer_2C_Data, UnkWidth * UnkHeight, name: nameof(Pointer_2C_Data)));
            Pointer_30_Structs = s.DoAt(Pointer_30, () => s.SerializeObjectArray<GBACrash_Isometric_UnkStruct_1>(Pointer_30_Structs, Pointer_2C_Data.Max() + 1, name: nameof(Pointer_30_Structs)));
            Pointer_34_Structs = s.DoAt(Pointer_34, () => s.SerializeObjectArray<GBACrash_Isometric_UnkStruct_2>(Pointer_34_Structs, Pointer_30_Structs.Max(x => x.Pointer_34_StructsIndex) + 1, name: nameof(Pointer_34_Structs)));

            s.DoAt(Pointer_4C, () =>
            {
                if (Pointer_4C_Structs == null)
                {
                    var objects = new List<GBACrash_Isometric_UnkStruct_0>();
                    var index = 0;

                    while (true)
                    {
                        var obj = s.SerializeObject<GBACrash_Isometric_UnkStruct_0>(default, name: $"{nameof(Pointer_4C_Structs)}[{index++}]");

                        if (obj.Int_00 == -1)
                            break;

                        objects.Add(obj);
                    }

                    Pointer_4C_Structs = objects.ToArray();
                }
                else
                {
                    s.SerializeObjectArray<GBACrash_Isometric_UnkStruct_0>(Pointer_4C_Structs, Pointer_4C_Structs.Length, name: nameof(Pointer_4C_Structs));
                }
            });
        }
    }

    public class GBACrash_Isometric_UnkStruct_0 : R1Serializable
    {
        public int Int_00 { get; set; }
        public short XPos_0 { get; set; }
        public short XPos_1 { get; set; }
        public short YPos_0 { get; set; }
        public short YPos_1 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            XPos_0 = s.Serialize<short>(XPos_0, name: nameof(XPos_0));
            XPos_1 = s.Serialize<short>(XPos_1, name: nameof(XPos_1));
            YPos_0 = s.Serialize<short>(YPos_0, name: nameof(YPos_0));
            YPos_1 = s.Serialize<short>(YPos_1, name: nameof(YPos_1));
        }
    }
    public class GBACrash_Isometric_UnkStruct_1 : R1Serializable
    {
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public short Short_02 { get; set; }
        public int Pointer_34_StructsIndex { get; set; }
        public byte Byte_08 { get; set; }
        public byte Byte_09 { get; set; }
        public byte Byte_0A { get; set; }
        public byte Byte_0B { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
            Pointer_34_StructsIndex = s.Serialize<int>(Pointer_34_StructsIndex, name: nameof(Pointer_34_StructsIndex));
            Byte_08 = s.Serialize<byte>(Byte_08, name: nameof(Byte_08));
            Byte_09 = s.Serialize<byte>(Byte_09, name: nameof(Byte_09));
            Byte_0A = s.Serialize<byte>(Byte_0A, name: nameof(Byte_0A));
            Byte_0B = s.Serialize<byte>(Byte_0B, name: nameof(Byte_0B));
        }
    }
    public class GBACrash_Isometric_UnkStruct_2 : R1Serializable
    {
        public Pointer FunctionPointer_0 { get; set; }
        public Pointer FunctionPointer_1 { get; set; }
        public Pointer FunctionPointer_2 { get; set; }
        public byte[] Bytes_10 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FunctionPointer_0 = s.SerializePointer(FunctionPointer_0, name: nameof(FunctionPointer_0));
            FunctionPointer_1 = s.SerializePointer(FunctionPointer_1, name: nameof(FunctionPointer_1));
            FunctionPointer_2 = s.SerializePointer(FunctionPointer_2, name: nameof(FunctionPointer_2));
            Bytes_10 = s.SerializeArray<byte>(Bytes_10, 28, name: nameof(Bytes_10));
        }
    }
}