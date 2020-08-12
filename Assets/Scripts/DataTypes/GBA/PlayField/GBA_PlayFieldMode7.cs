using System;

namespace R1Engine
{
    /// <summary>
    /// A 3D Mode7 PlayField for GBA
    /// </summary>
    public class GBA_PlayFieldMode7 : R1Serializable
    {
        #region PlayField Data

        public byte UnkOffsetIndex1 { get; set; }
        public byte UnkOffsetIndex2 { get; set; }
        public byte Byte_03 { get; set; }

        // 0-7
        public byte LayerCount { get; set; }

        public byte[] LayerTable { get; set; }

        // numTextLayers (0-4) is somewhere here - Byte_03?

        #endregion

        #region Parsed

        // There are text layers too

        public GBA_TileLayer[] RotScaleLayers { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s)
        {
            UnkOffsetIndex1 = s.Serialize<byte>(UnkOffsetIndex1, name: nameof(UnkOffsetIndex1));
            UnkOffsetIndex2 = s.Serialize<byte>(UnkOffsetIndex2, name: nameof(UnkOffsetIndex2));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            LayerCount = s.Serialize<byte>(LayerCount, name: nameof(LayerCount));

            LayerTable = s.SerializeArray<byte>(LayerTable, 8, name: nameof(LayerTable));

            // TODO: There is more data after this
        }

        public void SerializeOffsetData(SerializerObject s, GBA_OffsetTable offsetTable)
        {
            if (RotScaleLayers == null)
                RotScaleLayers = new GBA_TileLayer[LayerCount];

            // Serialize layers
            for (int i = 0; i < LayerCount; i++)
                RotScaleLayers[i] = s.DoAt(offsetTable.GetPointer(LayerTable[i]), () => s.SerializeObject<GBA_TileLayer>(RotScaleLayers[i], name: $"{nameof(RotScaleLayers)}[{i}]"));
        }

        #endregion
    }
}