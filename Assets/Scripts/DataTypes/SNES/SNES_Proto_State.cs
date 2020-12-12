namespace R1Engine
{
    public class SNES_Proto_State : R1Serializable
    {
        public SNES_Pointer Pointer_00 { get; set; }
        public SNES_Pointer Pointer_02 { get; set; }
        public SNES_Pointer AnimPointer { get; set; }
        public byte Byte_06 { get; set; }
        public byte Byte_07 { get; set; }
        public byte Byte_08_AnimRelated { get; set; } // Animation related. Same animation -> same number
        public byte Byte_09 { get; set; }
        public byte Byte_0A { get; set; }
        public byte FrameCount { get; set; } // Frame count
        public byte Byte_0C { get; set; }
        public byte Byte_0D { get; set; }
        public byte Byte_0E { get; set; }
        public byte Byte_0F { get; set; }

        public R1Jaguar_AnimationDescriptor Animation { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Pointer_00 = s.SerializeObject<SNES_Pointer>(Pointer_00, name: nameof(Pointer_00));
            Pointer_02 = s.SerializeObject<SNES_Pointer>(Pointer_02, name: nameof(Pointer_02));
            AnimPointer = s.SerializeObject<SNES_Pointer>(AnimPointer, name: nameof(AnimPointer));
            Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
            Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
            Byte_08_AnimRelated = s.Serialize<byte>(Byte_08_AnimRelated, name: nameof(Byte_08_AnimRelated));
            Byte_09 = s.Serialize<byte>(Byte_09, name: nameof(Byte_09));
            Byte_0A = s.Serialize<byte>(Byte_0A, name: nameof(Byte_0A));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            Byte_0C = s.Serialize<byte>(Byte_0C, name: nameof(Byte_0C));
            Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));
            Byte_0E = s.Serialize<byte>(Byte_0E, name: nameof(Byte_0E));
            Byte_0F = s.Serialize<byte>(Byte_0F, name: nameof(Byte_0F));

            // AnimationPointer points to first layer. So, go back 4 bytes to get header
            s.DoAt(AnimPointer.GetPointer() - 4, () => {
                Animation = s.SerializeObject<R1Jaguar_AnimationDescriptor>(Animation, name: nameof(Animation));
            });
        }
    }
}