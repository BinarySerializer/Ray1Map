namespace R1Engine
{
    public class GBC_Action : R1Serializable {
        public byte ActionID { get; set; }
        public byte Byte_01 { get; set; }
        public byte AnimIndex { get; set; } // Block index to use in GBC_Puppet. 0 = no animation?
        public bool FlipX { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion != EngineVersion.GBA_CrouchingTiger)
            {
                ActionID = s.Serialize<byte>(ActionID, name: nameof(ActionID));
                Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
                AnimIndex = s.Serialize<byte>(AnimIndex, name: nameof(AnimIndex));

                if (s.GameSettings.EngineVersion == EngineVersion.GBA_DonaldDuck)
                    FlipX = s.Serialize<bool>(FlipX, name: nameof(FlipX));
            }
            else
            {
                Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
                AnimIndex = s.Serialize<byte>(AnimIndex, name: nameof(AnimIndex));
                FlipX = s.Serialize<bool>(FlipX, name: nameof(FlipX));
                ActionID = s.Serialize<byte>(ActionID, name: nameof(ActionID));
            }
        }
    }
}