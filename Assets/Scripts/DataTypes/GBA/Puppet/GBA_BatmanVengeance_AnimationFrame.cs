using System.Collections.Generic;

namespace R1Engine
{
    public class GBA_BatmanVengeance_AnimationFrame : R1Serializable {
        #region Data
        public byte Byte_00 { get; set; }
        public byte Byte_01 { get; set; }
        public byte LayerCount { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }

        public byte[] Shanghai_Bytes { get; set; }

        #endregion

        #region Parsed

        public GBA_BatmanVengeance_AnimationChannel[] Layers { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s) 
        {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));

            if (s.GameSettings.EngineVersion < EngineVersion.GBA_BatmanVengeance)
            {
                Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
                LayerCount = s.Serialize<byte>(LayerCount, name: nameof(LayerCount));
            }
            else
            {
                LayerCount = s.Serialize<byte>(LayerCount, name: nameof(LayerCount));
                Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            }
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));

            Layers = s.SerializeObjectArray<GBA_BatmanVengeance_AnimationChannel>(Layers, LayerCount, name: nameof(Layers));

            if (s.GameSettings.EngineVersion < EngineVersion.GBA_BatmanVengeance)
                Shanghai_Bytes = s.SerializeArray<byte>(Shanghai_Bytes, 12, name: nameof(Shanghai_Bytes));
        }

        #endregion
    }
}