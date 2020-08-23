using System.Collections.Generic;

namespace R1Engine
{
    public class GBA_BatmanVengeance_AnimationFrame : R1Serializable {
        #region Data
        public byte Byte_00 { get; set; }
        public byte LayerCount { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }

        #endregion

        #region Parsed

        public GBA_BatmanVengeance_AnimationLayer[] Layers { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s) {
            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            LayerCount = s.Serialize<byte>(LayerCount, name: nameof(LayerCount));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            Layers = s.SerializeObjectArray<GBA_BatmanVengeance_AnimationLayer>(Layers, LayerCount, name: nameof(Layers));
        }

        #endregion
    }
}