using System.Linq;

namespace R1Engine
{
    public class GBAVV_Isometric_TileSet : R1Serializable
    {
        public ushort TileSetCount_Total { get; set; } // Set before serializing
        public ushort TileSetCount_4bpp { get; set; } // Set before serializing

        public byte[] TileSet_4bpp { get; set; }
        public byte[] TileSet_8bpp { get; set; }

        public ushort[] TileSet_4bpp_ConvertIndexTable { get; set; }
        public ConvertData[] TileSet_4bpp_ConvertDatas { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileSet_4bpp = s.SerializeArray<byte>(TileSet_4bpp, TileSetCount_4bpp * 0x20, name: nameof(TileSet_4bpp));
            TileSet_8bpp = s.SerializeArray<byte>(TileSet_8bpp, (TileSetCount_Total - TileSetCount_4bpp) * 0x40, name: nameof(TileSet_8bpp));
            TileSet_4bpp_ConvertIndexTable = s.SerializeArray<ushort>(TileSet_4bpp_ConvertIndexTable, TileSetCount_4bpp, name: nameof(TileSet_4bpp_ConvertIndexTable));
            s.Align();
            TileSet_4bpp_ConvertDatas = s.SerializeObjectArray<ConvertData>(TileSet_4bpp_ConvertDatas, TileSet_4bpp_ConvertIndexTable.Max() + 1, name: nameof(TileSet_4bpp_ConvertDatas));
        }

        public class ConvertData : R1Serializable
        {
            public byte[] Data { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Data = s.SerializeArray<byte>(Data, 0x10, name: nameof(Data));
            }
        }
    }
}