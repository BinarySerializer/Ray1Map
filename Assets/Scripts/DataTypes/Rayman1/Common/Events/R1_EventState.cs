using System;
using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Common event state (etat) data
    /// </summary>
    public class R1_EventState : BinarySerializable
    {
        // Right and left speed?
        public byte[] UnkR2_1 { get; set; }

        public byte[] UnkR2_2 { get; set; }

        public byte[] UnkR2_4 { get; set; }

        /// <summary>
        /// The right speed
        /// </summary>
        public sbyte RightSpeed { get; set; }

        public byte UnkDemo1 { get; set; }
        
        /// <summary>
        /// The left speed
        /// </summary>
        public sbyte LeftSpeed { get; set; }

        public byte UnkDemo2 { get; set; }

        /// <summary>
        /// The animation index
        /// </summary>
        public byte AnimationIndex { get; set; }

        public byte UnkDemo3 { get; set; }

        /// <summary>
        /// The etat value of the linked state
        /// </summary>
        public byte LinkedEtat { get; set; }
        
        /// <summary>
        /// The sub-etat value of the linked state
        /// </summary>
        public byte LinkedSubEtat { get; set; }

        public byte UnkDemo4 { get; set; }

        public byte Unk { get; set; }

        /// <summary>
        /// The amount of frames to skip in the animation each second, or 0 for it to not animate
        /// </summary>
        public byte AnimationSpeed { get; set; }

        public byte UnkDemo5 { get; set; }
        public byte UnkDemo6 { get; set; }
        public byte UnkDemo7 { get; set; }
        public byte UnkDemo8 { get; set; }

        public byte SoundIndex { get; set; } // Is it really a sound index?
        public R1_ZDCFlags ZDCFlags { get; set; }

        // For GBA
        public bool IsFlippedHorizontally { get; set; }
        public bool IsFlippedVertically { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            if (s.GetR1Settings().EngineVersion == EngineVersion.R2_PS1)
            {
                UnkR2_1 = s.SerializeArray<byte>(UnkR2_1, 4, name: nameof(UnkR2_1));
                AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
                UnkR2_2 = s.SerializeArray<byte>(UnkR2_2, 5, name: nameof(UnkR2_2));
                LinkedEtat = s.Serialize<byte>(LinkedEtat, name: nameof(LinkedEtat));
                LinkedSubEtat = s.Serialize<byte>(LinkedSubEtat, name: nameof(LinkedSubEtat));
                ZDCFlags = s.Serialize<R1_ZDCFlags>(ZDCFlags, name: nameof(ZDCFlags));
                AnimationSpeed = s.Serialize<byte>(AnimationSpeed, name: nameof(AnimationSpeed));
                UnkR2_4 = s.SerializeArray<byte>(UnkR2_4, 2, name: nameof(UnkR2_4));
            }
            else
            {
                RightSpeed = s.Serialize<sbyte>(RightSpeed, name: nameof(RightSpeed));

                if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 ||
                    s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
                    UnkDemo1 = s.Serialize<byte>(UnkDemo1, name: nameof(UnkDemo1));

                LeftSpeed = s.Serialize<sbyte>(LeftSpeed, name: nameof(LeftSpeed));

                if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 ||
                    s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
                    UnkDemo2 = s.Serialize<byte>(UnkDemo2, name: nameof(UnkDemo2));

                AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));

                if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 ||
                    s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol6)
                    UnkDemo3 = s.Serialize<byte>(UnkDemo3, name: nameof(UnkDemo3));

                LinkedEtat = s.Serialize<byte>(LinkedEtat, name: nameof(LinkedEtat));
                LinkedSubEtat = s.Serialize<byte>(LinkedSubEtat, name: nameof(LinkedSubEtat));

                if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol3)
                    UnkDemo4 = s.Serialize<byte>(UnkDemo4, name: nameof(UnkDemo4));

                if (s.GetR1Settings().EngineVersion == EngineVersion.R1_Saturn)
                {
                    s.SerializeBitValues<byte>(bitFunc =>
                    {
                        Unk = (byte)bitFunc(Unk, 4, name: nameof(Unk));
                        AnimationSpeed = (byte)bitFunc(AnimationSpeed, 4, name: nameof(AnimationSpeed));
                    });
                }
                else
                {
                    s.SerializeBitValues<byte>(bitFunc =>
                    {
                        AnimationSpeed = (byte)bitFunc(AnimationSpeed, 4, name: nameof(AnimationSpeed));
                        Unk = (byte)bitFunc(Unk, 4, name: nameof(Unk));
                    });
                }

                if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol3)
                {
                    UnkDemo5 = s.Serialize<byte>(UnkDemo5, name: nameof(UnkDemo5));
                    UnkDemo6 = s.Serialize<byte>(UnkDemo6, name: nameof(UnkDemo6));
                    UnkDemo7 = s.Serialize<byte>(UnkDemo7, name: nameof(UnkDemo7));
                    UnkDemo8 = s.Serialize<byte>(UnkDemo8, name: nameof(UnkDemo8));
                }
                else {
                    if (s.GetR1Settings().EngineVersion == EngineVersion.R1_PS1_JPDemoVol6) {
                        UnkDemo4 = s.Serialize<byte>(UnkDemo4, name: nameof(UnkDemo4));
                    }
                    SoundIndex = s.Serialize<byte>(SoundIndex, name: nameof(SoundIndex));
                    ZDCFlags = s.Serialize<R1_ZDCFlags>(ZDCFlags, name: nameof(ZDCFlags));
                }
            }
        }

        // Might not be correct
        [Flags]
        public enum R1_ZDCFlags : byte
        {
            None = 0,

            Flag_00 = 1 << 0,
            Flag_01 = 1 << 1,
            Flag_02 = 1 << 2,

            DetectFist = 1 << 3,
            Flag_04 = 1 << 4,
            DetectRay = 1 << 5,

            Param1 = 1 << 6,
            Param2 = 1 << 7,
        }
    }
}