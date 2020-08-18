using UnityEngine;

namespace R1Engine
{
    public class GBA_Actor : R1Serializable
    {
        #region Actor Data

        public ushort XPos { get; set; }
        public ushort YPos { get; set; }

        public byte Byte_04 { get; set; }
        
        public GBA_R3_ActorID ActorID { get; set; }
        
        public byte GraphicsDataIndex { get; set; }

        public byte StateIndex { get; set; }

        public byte Link_0 { get; set; }
        public byte Link_1 { get; set; }
        public byte Link_2 { get; set; }
        public byte Link_3 { get; set; }

        // Only in Prince of Persia
        public short Short_0C { get; set; }
        public short Short_0E { get; set; }
        public byte[] ExtraData { get; set; }

        #endregion

        #region Parsed

        public GBA_ActorGraphicData GraphicData { get; set; }

        #endregion

        #region Public Methods

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<ushort>(XPos, name: nameof(XPos));
            YPos = s.Serialize<ushort>(YPos, name: nameof(YPos));

            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            ActorID = (GBA_R3_ActorID)s.Serialize<byte>((byte)ActorID, name: nameof(ActorID));
            GraphicsDataIndex = s.Serialize<byte>(GraphicsDataIndex, name: nameof(GraphicsDataIndex));
            StateIndex = s.Serialize<byte>(StateIndex, name: nameof(StateIndex));

            if (s.GameSettings.EngineVersion != EngineVersion.GBA_BatmanVengeance) {
                Link_0 = s.Serialize<byte>(Link_0, name: nameof(Link_0));
                Link_1 = s.Serialize<byte>(Link_1, name: nameof(Link_1));
                Link_2 = s.Serialize<byte>(Link_2, name: nameof(Link_2));
                Link_3 = s.Serialize<byte>(Link_3, name: nameof(Link_3));
            }

            if (s.GameSettings.EngineVersion == EngineVersion.GBA_PrinceOfPersia)
            {
                Short_0C = s.Serialize<short>(Short_0C, name: nameof(Short_0C));
                Short_0E = s.Serialize<short>(Short_0E, name: nameof(Short_0E));
                int len = Short_0E & 0xF;
                ExtraData = s.SerializeArray<byte>(ExtraData, len, name: nameof(ExtraData));
            }
        }

        #endregion
    }
}