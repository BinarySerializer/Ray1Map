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
            // TODO: Implement
        }

        #endregion
    }

    public class GBA_RotScaleLayerMode7 : GBA_BaseBlock
    {
        /*
        
        Bool_01: IsCompressed
        Byte_08: LayerId (0-1)
        Bool_09: ShouldSetBGAlphaBlending
        Byte_0A: Related to BG Alpha Blending
        UInt_14: Map?
         */

        public override void SerializeImpl(SerializerObject s)
        {
            throw new NotImplementedException();
        }
    }
    public class GBA_TextLayerMode7 : GBA_BaseBlock
    {
        /*
        
        Byte_08: LayerId (0-1)
        Bool_09: ShouldSetBGAlphaBlending
        Byte_0A: Related to BG Alpha Blending
        Bool_1D: Priority (0-3)
        Bool_1f: ColorMode
         */

        public override void SerializeImpl(SerializerObject s)
        {
            throw new NotImplementedException();
        }
    }
}