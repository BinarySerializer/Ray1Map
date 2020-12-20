namespace R1Engine
{
    public class GBC_Action : R1Serializable {
        public byte ActionID { get; set; }
        public byte Byte_01 { get; set; }
        public byte AnimIndex { get; set; } // Block index to use in GBC_Puppet. 0 = no animation?
        public byte Byte_03 { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            ActionID = s.Serialize<byte>(ActionID, name: nameof(ActionID));
            Byte_01 = s.Serialize<byte>(Byte_01, name: nameof(Byte_01));
            AnimIndex = s.Serialize<byte>(AnimIndex, name: nameof(AnimIndex));

            if (s.GameSettings.MajorEngineVersion == MajorEngineVersion.GBA)
                Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
        }
    }
}