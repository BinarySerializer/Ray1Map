using System;

namespace R1Engine
{
    public class SNES_Proto_State : R1Serializable
    {
        public SNES_Pointer MovementFunctionPointer { get; set; } // Modifying this changes how Rayman moves
        public SNES_Pointer CollisionFunctionPointer { get; set; } // Modifying this changes what kind of platform Rayman thinks he's on
        public SNES_Pointer AnimPointer { get; set; }
        public StateFlags Flags { get; set; } // Flip flag is one of the lower bits. Upper bits contain some state switching flag (states loop forever if nulled)
        public byte Byte_07 { get; set; } // 0 or 0xFF
        public byte Byte_08_AnimRelated { get; set; } // Animation related. Same animation -> same number
        public byte Byte_09 { get; set; }
        public byte VRAMConfigurationID { get; set; } // 0,2,4,6. If 0, vram isn't modified. Making this 0 for all states makes the game not load any extra image block
        public byte FrameCount { get; set; } // Frame count
        public byte AnimSpeed { get; set; }
        public byte Byte_0D { get; set; }
        public byte Byte_0E { get; set; }
        public byte Byte_0F { get; set; } // Always 0

        public R1Jaguar_AnimationDescriptor Animation { get; set; }
        public int VRAMConfigIndex => Math.Max(0, (VRAMConfigurationID / 2) - 1);

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            MovementFunctionPointer = s.SerializeObject<SNES_Pointer>(MovementFunctionPointer, name: nameof(MovementFunctionPointer));
            CollisionFunctionPointer = s.SerializeObject<SNES_Pointer>(CollisionFunctionPointer, name: nameof(CollisionFunctionPointer));
            AnimPointer = s.SerializeObject<SNES_Pointer>(AnimPointer, name: nameof(AnimPointer));
            Flags = s.Serialize<StateFlags>(Flags, name: nameof(Flags));
            Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
            Byte_08_AnimRelated = s.Serialize<byte>(Byte_08_AnimRelated, name: nameof(Byte_08_AnimRelated));
            Byte_09 = s.Serialize<byte>(Byte_09, name: nameof(Byte_09));
            VRAMConfigurationID = s.Serialize<byte>(VRAMConfigurationID, name: nameof(VRAMConfigurationID));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            AnimSpeed = s.Serialize<byte>(AnimSpeed, name: nameof(AnimSpeed));
            Byte_0D = s.Serialize<byte>(Byte_0D, name: nameof(Byte_0D));
            Byte_0E = s.Serialize<byte>(Byte_0E, name: nameof(Byte_0E));
            Byte_0F = s.Serialize<byte>(Byte_0F, name: nameof(Byte_0F));

            // AnimationPointer points to first layer. So, go back 4 bytes to get header
            Animation = s.DoAt(AnimPointer.GetPointer() - 4, () => s.SerializeObject<R1Jaguar_AnimationDescriptor>(Animation, name: nameof(Animation)));
        }



        [Flags]
        public enum StateFlags : byte {
            None = 0,

            HorizontalFlip = 1 << 0,
            UseCurrentFlip = 1 << 1,
            UnkFlag_2 = 1 << 2,
            UnkFlag_3 = 1 << 3,
            UnkFlag_4 = 1 << 4,
            UnkFlag_5 = 1 << 5,
            UnkFlag_6 = 1 << 6,
            UnkFlag_7 = 1 << 7,
        }
    }
}