namespace R1Engine
{
    /// <summary>
    /// Event state for Rayman 1 (Jaguar)
    /// </summary>
    public class R1Jaguar_EventState : R1Serializable
    {
        public byte Byte00 { get; set; }
        public byte AnimationSpeed { get; set; }

        public Pointer AnimationPointer { get; set; }
        public Pointer LinkedStatePointer { get; set; }
        public Pointer CodePointer { get; set; }

        public byte Byte0A { get; set; }
        public byte Byte0B { get; set; }

        // Parsed
        public R1Jaguar_AnimationDescriptor Animation { get; set; }
        public R1Jaguar_EventState LinkedState { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Byte00 = s.Serialize<byte>(Byte00, name: nameof(Byte00));
            AnimationSpeed = s.Serialize<byte>(AnimationSpeed, name: nameof(AnimationSpeed));
            if (Byte00 != 0) {
                AnimationPointer = s.SerializePointer(AnimationPointer, name: nameof(AnimationPointer));
            } else {
                LinkedStatePointer = s.SerializePointer(LinkedStatePointer, name: nameof(LinkedStatePointer));
            }
            CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
            Byte0A = s.Serialize<byte>(Byte0A, name: nameof(Byte0A));
            Byte0B = s.Serialize<byte>(Byte0B, name: nameof(Byte0B));

            s.DoAt(AnimationPointer, () => {
                Animation = s.SerializeObject<R1Jaguar_AnimationDescriptor>(Animation, name: nameof(Animation));
                //Animation = s.SerializeObject<Jaguar_R1_AnimationDescriptor>(Animation, onPreSerialize: l => l.FlipFlagInX = this.FlipFlagInX, name: nameof(Animation));
                //Animation = s.SerializeObject<Jaguar_R1_AnimationDescriptor>(Animation, onPreSerialize: l => l.FlipFlagInX = (Byte00 != 200), name: nameof(Animation));
            });
            s.DoAt(LinkedStatePointer, () => {
                LinkedState = s.SerializeObject<R1Jaguar_EventState>(LinkedState, name: nameof(LinkedState));
            });
        }
    }
}