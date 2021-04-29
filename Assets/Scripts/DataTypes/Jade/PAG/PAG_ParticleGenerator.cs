using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class PAG_ParticleGenerator : BinarySerializable {
        public int Int_00 { get; set; }
        public int Type { get; set; }

        // Generator struct
        public int Flags { get; set; }
        public byte GenType { get; set; }
        public byte SpeedType { get; set; }

        public float GenCountPerSecond { get; set; }
        public float GenCountPerSecondInit { get; set; }
        public float GenTime { get; set; }
        
        public float GenOffset { get; set; }
        public float GenParam0 { get; set; }
        public float GenParam1 { get; set; }
        public float GenParam2 { get; set; }
        public float Speed0 { get; set; }
        public float Speed1 { get; set; }
        public float Angle1 { get; set; }
        public float Angle2 { get; set; }
        public float SizeXMin { get; set; }
        public float SizeXMax { get; set; }
        public float SizeYMin { get; set; }
        public float SizeYMax { get; set; }
        public float TimeMin { get; set; }
        public float TimeMax { get; set; }
        public float TimeDeathMin { get; set; }
        public float TimeDeathMax { get; set; }
        public float TimeBirthMin { get; set; }
        public float TimeBirthMax { get; set; }
        public float SizeDeathFactor { get; set; }

        public Jade_Vector Acceleration { get; set; }

        public float Friction { get; set; }
        public float ZMin { get; set; }
        public float ZMinStrength { get; set; }
        public float ZMax { get; set; }
        public float ZMaxStrength { get; set; }
        public float RotationMin { get; set; }
        public float RotationMax { get; set; }
        public float RotationSpeedMin { get; set; }
        public float RotationSpeedMax { get; set; }
        public float SinXFactor { get; set; }
        public float SinYFactor { get; set; }
        public float DistConstraint { get; set; }
        public Jade_Color Color { get; set; }

        public byte Byte_270 { get; set; }

        // Editor
        public byte Type0_Byte_03_Editor { get; set; }
        public uint Type0_Uint_18_Editor { get; set; }
        public uint Type0_Uint_1C_Editor { get; set; }
        public uint Type0_Uint_20_Editor { get; set; }
        public uint Type0_Uint_24_Editor { get; set; }
        public uint Type0_Uint_28_Editor { get; set; }
        public uint Type0_Uint_2C_Editor { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
            Type = s.Serialize<int>(Type, name: nameof(Type));

            if (Type == 0)
            {
                Flags = s.Serialize<byte>((byte)Flags, name: nameof(Flags));
                GenType = s.Serialize<byte>(GenType, name: nameof(GenType));
                SpeedType = s.Serialize<byte>(SpeedType, name: nameof(SpeedType));

                if (!Loader.IsBinaryData) Type0_Byte_03_Editor = s.Serialize<byte>(Type0_Byte_03_Editor, name: nameof(Type0_Byte_03_Editor));

                GenParam0 = s.Serialize<float>(GenParam0, name: nameof(GenParam0));
                GenParam1 = s.Serialize<float>(GenParam1, name: nameof(GenParam1));
                GenParam2 = s.Serialize<float>(GenParam2, name: nameof(GenParam2));

                GenCountPerSecondInit = s.Serialize<float>(GenCountPerSecondInit, name: nameof(GenCountPerSecondInit));
                GenTime = s.Serialize<float>(GenTime, name: nameof(GenTime));

                if (!Loader.IsBinaryData)
                {
                    Type0_Uint_18_Editor = s.Serialize<uint>(Type0_Uint_18_Editor, name: nameof(Type0_Uint_18_Editor));
                    Type0_Uint_1C_Editor = s.Serialize<uint>(Type0_Uint_1C_Editor, name: nameof(Type0_Uint_1C_Editor));
                    Type0_Uint_20_Editor = s.Serialize<uint>(Type0_Uint_20_Editor, name: nameof(Type0_Uint_20_Editor));
                    Type0_Uint_24_Editor = s.Serialize<uint>(Type0_Uint_24_Editor, name: nameof(Type0_Uint_24_Editor));
                    Type0_Uint_28_Editor = s.Serialize<uint>(Type0_Uint_28_Editor, name: nameof(Type0_Uint_28_Editor));
                    Type0_Uint_2C_Editor = s.Serialize<uint>(Type0_Uint_2C_Editor, name: nameof(Type0_Uint_2C_Editor));
                }

                Speed0 = s.Serialize<float>(Speed0, name: nameof(Speed0));
                Speed1 = s.Serialize<float>(Speed1, name: nameof(Speed1));
                Angle1 = s.Serialize<float>(Angle1, name: nameof(Angle1));
                Angle2 = s.Serialize<float>(Angle2, name: nameof(Angle2));

                SizeXMin = s.Serialize<float>(SizeXMin, name: nameof(SizeXMin));
                SizeXMax = SizeXMin;
                SizeYMin = s.Serialize<float>(SizeYMin, name: nameof(SizeYMin));
                SizeYMax = SizeYMin;

                TimeMin = s.Serialize<float>(TimeMin, name: nameof(TimeMin));
                TimeMax = s.Serialize<float>(TimeMax, name: nameof(TimeMax));

                Acceleration = s.SerializeObject(Acceleration, name: nameof(Acceleration));
            }
            else if (Type == 1)
            {
                Flags = s.Serialize<byte>((byte)Flags, name: nameof(Flags));
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 2)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 3)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 4)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 5)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 6)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 7)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 8)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Type}");
            }
            else if (Type == 9 || Type == 10 || Type == 11 || Type == 12) {
                if (Type >= 11) {
                    Flags = s.Serialize<int>(Flags, name: nameof(Flags));
                } else {
                    Flags = s.Serialize<ushort>((ushort)Flags, name: nameof(Flags));
                }
                GenType = s.Serialize<byte>(GenType, name: nameof(GenType));
                SpeedType = s.Serialize<byte>(SpeedType, name: nameof(SpeedType));

                GenOffset = s.Serialize<float>(GenOffset, name: nameof(GenOffset));
                GenParam0 = s.Serialize<float>(GenParam0, name: nameof(GenParam0));
                GenParam1 = s.Serialize<float>(GenParam1, name: nameof(GenParam1));
                GenParam2 = s.Serialize<float>(GenParam2, name: nameof(GenParam2));

                GenCountPerSecondInit = s.Serialize<float>(GenCountPerSecondInit, name: nameof(GenCountPerSecondInit));
                GenCountPerSecond = GenCountPerSecondInit;

                Speed0 = s.Serialize<float>(Speed0, name: nameof(Speed0));
                Speed1 = s.Serialize<float>(Speed1, name: nameof(Speed1));
                Angle1 = s.Serialize<float>(Angle1, name: nameof(Angle1));
                Angle2 = s.Serialize<float>(Angle2, name: nameof(Angle2));
                SizeXMin = s.Serialize<float>(SizeXMin, name: nameof(SizeXMin));
                SizeXMax = s.Serialize<float>(SizeXMax, name: nameof(SizeXMax));
                SizeYMin = s.Serialize<float>(SizeYMin, name: nameof(SizeYMin));
                SizeYMax = s.Serialize<float>(SizeYMax, name: nameof(SizeYMax));
                TimeMin = s.Serialize<float>(TimeMin, name: nameof(TimeMin));
                TimeMax = s.Serialize<float>(TimeMax, name: nameof(TimeMax));
                TimeDeathMin = s.Serialize<float>(TimeDeathMin, name: nameof(TimeDeathMin));
                TimeDeathMax = s.Serialize<float>(TimeDeathMax, name: nameof(TimeDeathMax));
                TimeBirthMin = s.Serialize<float>(TimeBirthMin, name: nameof(TimeBirthMin));
                TimeBirthMax = s.Serialize<float>(TimeBirthMax, name: nameof(TimeBirthMax));

                Acceleration = s.SerializeObject(Acceleration, name: nameof(Acceleration));

                Friction = s.Serialize<float>(Friction, name: nameof(Friction));
                SizeDeathFactor = s.Serialize<float>(SizeDeathFactor, name: nameof(SizeDeathFactor));

                Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));

                ZMin = s.Serialize<float>(ZMin, name: nameof(ZMin));
                ZMinStrength = s.Serialize<float>(ZMinStrength, name: nameof(ZMinStrength));
                ZMax = s.Serialize<float>(ZMax, name: nameof(ZMax));
                ZMaxStrength = s.Serialize<float>(ZMaxStrength, name: nameof(ZMaxStrength));
                RotationMin = s.Serialize<float>(RotationMin, name: nameof(RotationMin));
                RotationMax = s.Serialize<float>(RotationMax, name: nameof(RotationMax));
                RotationSpeedMin = s.Serialize<float>(RotationSpeedMin, name: nameof(RotationSpeedMin));
                RotationSpeedMax = s.Serialize<float>(RotationSpeedMax, name: nameof(RotationSpeedMax));
                SinXFactor = s.Serialize<float>(SinXFactor, name: nameof(SinXFactor));
                SinYFactor = s.Serialize<float>(SinYFactor, name: nameof(SinYFactor));

                if (Type >= 10) {
                    DistConstraint = s.Serialize<float>(DistConstraint, name: nameof(DistConstraint));
                }

                if (Type >= 12) {
                    Byte_270 = s.Serialize<byte>(Byte_270, name: nameof(Byte_270));
                }
            }
        }
    }
}