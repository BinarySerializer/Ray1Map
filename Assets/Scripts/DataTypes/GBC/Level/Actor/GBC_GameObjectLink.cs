namespace R1Engine
{
    public class GBC_GameObjectLink : R1Serializable
    {
        public byte MessageID { get; set; }
        public ushort MessageTarget { get; set; }

        public bool MessageTargetIsGameObject => BitHelpers.ExtractBits(MessageTarget, 1, 15) == 0;
        public ushort? GameObjectID => MessageTargetIsGameObject ? (ushort?)BitHelpers.ExtractBits(MessageTarget, 15, 0) : null;
        public MessageType Type {
            get {
                var upperByte = BitHelpers.ExtractBits(MessageTarget, 8,8);
                if(upperByte == 0xFF) return MessageType.Game;
                if(upperByte == 0xFE) return MessageType.Sound;
                return MessageType.GameObject;
            }
        }

        public enum MessageType {
            GameObject,
            Sound,
            Game
        }

        public override void SerializeImpl(SerializerObject s)
        {
            MessageID = s.Serialize<byte>(MessageID, name: nameof(MessageID));
            MessageTarget = s.Serialize<ushort>(MessageTarget, name: nameof(MessageTarget));
        }
    }
}