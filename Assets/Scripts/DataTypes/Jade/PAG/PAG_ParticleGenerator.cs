using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class PAG_ParticleGenerator : BinarySerializable {
        public uint ObjectVersion { get; set; }

        public int InstancesNbMaxP { get; set; }
        public int Version { get; set; }

        // Generator struct
        public PAG_ParticleGeneratorFlags Flags { get; set; }
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

        // Phoenix
        public float Phoenix_Float_0 { get; set; }
        public float Phoenix_Float_1 { get; set; }
        public int Phoenix_Int_0 { get; set; }
        public int Phoenix_Int_1 { get; set; }

        // Montreal
        public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
        public uint MoreFlags { get; set; }
        public float GenOffsetX { get; set; }
        public float GenOffsetY { get; set; }
        public float ZMinRotationStrength { get; set; }
        public float ZMaxRotationStrength { get; set; }
        public float FrictionRotation { get; set; }
        public float ZMinRotationStrengthY { get; set; }
        public float ZMaxRotationStrengthY { get; set; }
        public float FrictionRotationY { get; set; }
        public float ZMinRotationStrengthZ { get; set; }
        public float ZMaxRotationStrengthZ { get; set; }
        public float FrictionRotationZ { get; set; }
        public float SpeedOfPAGScale { get; set; }
        public float WindForce { get; set; }
        public float SizeSpeedScaleX { get; set; }
        public float SizeSpeedScaleY { get; set; }
        public float FeatherMovementSpeedMin { get; set; }
        public Jade_Color BirthColor { get; set; }
        public Jade_Color ColorEndLife { get; set; }
        public Jade_Color DeathColor { get; set; }
        public Jade_Color BirthColor2 { get; set; }
        public Jade_Color Color2 { get; set; }
        public Jade_Color ColorEndLife2 { get; set; }
        public Jade_Color DeathColor2 { get; set; }
        public byte FramesCountX { get; set; }
        public byte FramesCountY { get; set; }
        public byte AnimTicksCountPerAnimFrame { get; set; }
        public Jade_Reference<OBJ_GameObject> EndLifeSpawnGenerator { get; set; }
        public ushort AmountToSpawnOnEndLife { get; set; }
        public Jade_Reference<OBJ_GameObject> DeathSpawnGeneratorA { get; set; }
        public ushort AmountToSpawnOnDeathA { get; set; }
        public Jade_Reference<OBJ_GameObject> DeathSpawnGeneratorB { get; set; }
        public ushort AmountToSpawnOnDeathB { get; set; }
        public Jade_Reference<OBJ_GameObject> Particle3DObject { get; set; }
        public float RotationMinY { get; set; }
        public float RotationMaxY { get; set; }
        public float RotationSpeedMinY { get; set; }
        public float RotationSpeedMaxY { get; set; }
        public float RotationMinZ { get; set; }
        public float RotationMaxZ { get; set; }
        public float RotationSpeedMinZ { get; set; }
        public float RotationSpeedMaxZ { get; set; }
        public byte SpecialLookAtX { get; set; }
        public byte SpecialLookAtY { get; set; }
        public uint SoundID { get; set; }
        public int PoolsCount { get; set; }
        public byte AttractorType { get; set; }
        public Jade_Reference<OBJ_GameObject> Attractor { get; set; }
        public float AttractorParam1 { get; set; }
        public float AttractorParam2 { get; set; }
        public float AttractorParam3 { get; set; }
        public float AttractorParam4 { get; set; }
        public float AttractorParam5 { get; set; }
        public float AttractorParam6 { get; set; }
        public Jade_Vector AttractorVector1 { get; set; }
        public float DispersionTransForceDamping { get; set; }
        public float DispersionRotForceDamping { get; set; }
        public float FPSCalibration { get; set; }
        public float FrontForceMax { get; set; }
        public float RearForceMax { get; set; }
        public float LODMinPercentageOfParticles { get; set; }


        public override void SerializeImpl(SerializerObject s) {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            InstancesNbMaxP = s.Serialize<int>(InstancesNbMaxP, name: nameof(InstancesNbMaxP));
            Version = (int)ObjectVersion;
            if (ObjectVersion < 21 || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
                Version = s.Serialize<int>(Version, name: nameof(Version));
            }

            if (Version == 0)
            {
                Flags = (PAG_ParticleGeneratorFlags)s.Serialize<byte>((byte)Flags, name: nameof(Flags));
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
            else if (Version == 1)
            {
                Flags = (PAG_ParticleGeneratorFlags)s.Serialize<byte>((byte)Flags, name: nameof(Flags));
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Version}");
            }
            else if (Version == 2)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Version}");
            }
            else if (Version == 3)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Version}");
            }
            else if (Version == 4)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Version}");
            }
            else if (Version == 5)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Version}");
            }
            else if (Version == 6)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Version}");
            }
            else if (Version == 7)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Version}");
            }
            else if (Version == 8)
            {
                throw new NotImplementedException($"TODO: Implement ModifierMPAG Type {Version}");
            }
            else if (Version == 9 || Version == 10 || Version == 11 || (Version == 12 && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier))) {
                if (Version >= 11 || (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Version == 10)) {
                    Flags = s.Serialize<PAG_ParticleGeneratorFlags>(Flags, name: nameof(Flags));
                } else {
                    Flags = (PAG_ParticleGeneratorFlags)s.Serialize<ushort>((ushort)Flags, name: nameof(Flags));
                }
                GenType = s.Serialize<byte>(GenType, name: nameof(GenType));
                SpeedType = s.Serialize<byte>(SpeedType, name: nameof(SpeedType));

                GenOffset = s.Serialize<float>(GenOffset, name: nameof(GenOffset));
                GenParam0 = s.Serialize<float>(GenParam0, name: nameof(GenParam0));
                GenParam1 = s.Serialize<float>(GenParam1, name: nameof(GenParam1));
                GenParam2 = s.Serialize<float>(GenParam2, name: nameof(GenParam2));

                GenCountPerSecondInit = s.Serialize<float>(GenCountPerSecondInit, name: nameof(GenCountPerSecondInit));
                GenCountPerSecond = GenCountPerSecondInit; // NbPerSecondWantedByUser in NCIS

                Speed0 = s.Serialize<float>(Speed0, name: nameof(Speed0));
                Speed1 = s.Serialize<float>(Speed1, name: nameof(Speed1));
                Angle1 = s.Serialize<float>(Angle1, name: nameof(Angle1));
                Angle2 = s.Serialize<float>(Angle2, name: nameof(Angle2));
                SizeXMin = s.Serialize<float>(SizeXMin, name: nameof(SizeXMin));
                SizeXMax = s.Serialize<float>(SizeXMax, name: nameof(SizeXMax));
                SizeYMin = s.Serialize<float>(SizeYMin, name: nameof(SizeYMin));
                SizeYMax = s.Serialize<float>(SizeYMax, name: nameof(SizeYMax));
                TimeMin = s.Serialize<float>(TimeMin, name: nameof(TimeMin)); // TimeOriginalMin
                TimeMax = s.Serialize<float>(TimeMax, name: nameof(TimeMax)); // TimeOriginalMax
                TimeDeathMin = s.Serialize<float>(TimeDeathMin, name: nameof(TimeDeathMin));
                TimeDeathMax = s.Serialize<float>(TimeDeathMax, name: nameof(TimeDeathMax));
                TimeBirthMin = s.Serialize<float>(TimeBirthMin, name: nameof(TimeBirthMin));
                TimeBirthMax = s.Serialize<float>(TimeBirthMax, name: nameof(TimeBirthMax));

                Acceleration = s.SerializeObject(Acceleration, name: nameof(Acceleration));

                Friction = s.Serialize<float>(Friction, name: nameof(Friction)); // FrictionTranslationOriginal
                SizeDeathFactor = s.Serialize<float>(SizeDeathFactor, name: nameof(SizeDeathFactor));

                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Version >= 11) {
					BirthColor = s.SerializeObject<Jade_Color>(BirthColor, name: nameof(BirthColor));
                    Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
                    ColorEndLife = s.SerializeObject<Jade_Color>(ColorEndLife, name: nameof(ColorEndLife));
                    DeathColor = s.SerializeObject<Jade_Color>(DeathColor, name: nameof(DeathColor));
                } else {
                    Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
                }

                ZMin = s.Serialize<float>(ZMin, name: nameof(ZMin));
                ZMinStrength = s.Serialize<float>(ZMinStrength, name: nameof(ZMinStrength));
                ZMax = s.Serialize<float>(ZMax, name: nameof(ZMax));
                ZMaxStrength = s.Serialize<float>(ZMaxStrength, name: nameof(ZMaxStrength));
                RotationMin = s.Serialize<float>(RotationMin, name: nameof(RotationMin));
                RotationMax = s.Serialize<float>(RotationMax, name: nameof(RotationMax));
                RotationSpeedMin = s.Serialize<float>(RotationSpeedMin, name: nameof(RotationSpeedMin));
                RotationSpeedMax = s.Serialize<float>(RotationSpeedMax, name: nameof(RotationSpeedMax));

                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
                    SinXFactor = s.Serialize<float>(SinXFactor, name: nameof(SinXFactor));
                    SinYFactor = s.Serialize<float>(SinYFactor, name: nameof(SinYFactor));

                    if (Version >= 10) {
                        DistConstraint = s.Serialize<float>(DistConstraint, name: nameof(DistConstraint));
                    }

                    if (Version >= 12) {
                        Byte_270 = s.Serialize<byte>(Byte_270, name: nameof(Byte_270));

                        if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Phoenix)) {
                            Phoenix_Float_0 = s.Serialize<float>(Phoenix_Float_0, name: nameof(Phoenix_Float_0));
                            Phoenix_Float_1 = s.Serialize<float>(Phoenix_Float_1, name: nameof(Phoenix_Float_1));
                            Phoenix_Int_0 = s.Serialize<int>(Phoenix_Int_0, name: nameof(Phoenix_Int_0));
                            Phoenix_Int_1 = s.Serialize<int>(Phoenix_Int_1, name: nameof(Phoenix_Int_1));
                        }
                    }
                } else {
                    GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
                }
            }
            else if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Version > 11) {
                Flags = s.Serialize<PAG_ParticleGeneratorFlags>(Flags, name: nameof(Flags));
                if (Version >= 20) MoreFlags = s.Serialize<uint>(MoreFlags, name: nameof(MoreFlags));

                GenType = s.Serialize<byte>(GenType, name: nameof(GenType));
                SpeedType = s.Serialize<byte>(SpeedType, name: nameof(SpeedType));
                if (Version >= 29) {
					GenOffsetX = s.Serialize<float>(GenOffsetX, name: nameof(GenOffsetX));
					GenOffsetY = s.Serialize<float>(GenOffsetY, name: nameof(GenOffsetY));
                }
                GenOffset = s.Serialize<float>(GenOffset, name: nameof(GenOffset));

                if (Version >= 30) {
					ZMinRotationStrength = s.Serialize<float>(ZMinRotationStrength, name: nameof(ZMinRotationStrength));
					ZMaxRotationStrength = s.Serialize<float>(ZMaxRotationStrength, name: nameof(ZMaxRotationStrength));
					FrictionRotation = s.Serialize<float>(FrictionRotation, name: nameof(FrictionRotation));

                    if (Version >= 32 && s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_T2T)) {
                        ZMinRotationStrengthY = s.Serialize<float>(ZMinRotationStrengthY, name: nameof(ZMinRotationStrengthY));
                        ZMaxRotationStrengthY = s.Serialize<float>(ZMaxRotationStrengthY, name: nameof(ZMaxRotationStrengthY));
                        FrictionRotationY = s.Serialize<float>(FrictionRotationY, name: nameof(FrictionRotationY));
                        ZMinRotationStrengthZ = s.Serialize<float>(ZMinRotationStrengthZ, name: nameof(ZMinRotationStrengthZ));
                        ZMaxRotationStrengthZ = s.Serialize<float>(ZMaxRotationStrengthZ, name: nameof(ZMaxRotationStrengthZ));
                        FrictionRotationZ = s.Serialize<float>(FrictionRotationZ, name: nameof(FrictionRotationZ));
                    } else {
                        ZMinRotationStrengthY = ZMinRotationStrength;
                        ZMaxRotationStrengthY = ZMaxRotationStrength;
                        FrictionRotationY = FrictionRotation;
                        ZMinRotationStrengthZ = ZMinRotationStrength;
                        ZMaxRotationStrengthZ = ZMaxRotationStrength;
                        FrictionRotationZ = FrictionRotation;
                    }
                }
                GenParam0 = s.Serialize<float>(GenParam0, name: nameof(GenParam0));
                GenParam1 = s.Serialize<float>(GenParam1, name: nameof(GenParam1));
                GenParam2 = s.Serialize<float>(GenParam2, name: nameof(GenParam2));

                GenCountPerSecondInit = s.Serialize<float>(GenCountPerSecondInit, name: nameof(GenCountPerSecondInit));
                GenCountPerSecond = GenCountPerSecondInit; // NbPerSecondWantedByUser in NCIS

                Speed0 = s.Serialize<float>(Speed0, name: nameof(Speed0));
                Speed1 = s.Serialize<float>(Speed1, name: nameof(Speed1));

				if (Version >= 26) SpeedOfPAGScale = s.Serialize<float>(SpeedOfPAGScale, name: nameof(SpeedOfPAGScale));
				if (Version >= 27) WindForce = s.Serialize<float>(WindForce, name: nameof(WindForce));

                Angle1 = s.Serialize<float>(Angle1, name: nameof(Angle1));
                Angle2 = s.Serialize<float>(Angle2, name: nameof(Angle2));
                SizeXMin = s.Serialize<float>(SizeXMin, name: nameof(SizeXMin));
                SizeXMax = s.Serialize<float>(SizeXMax, name: nameof(SizeXMax));
                SizeYMin = s.Serialize<float>(SizeYMin, name: nameof(SizeYMin));
                SizeYMax = s.Serialize<float>(SizeYMax, name: nameof(SizeYMax));

                if (Version >= 31) {
					SizeSpeedScaleX = s.Serialize<float>(SizeSpeedScaleX, name: nameof(SizeSpeedScaleX));
					SizeSpeedScaleY = s.Serialize<float>(SizeSpeedScaleY, name: nameof(SizeSpeedScaleY));
                }
                if (Version >= 32)
                    FeatherMovementSpeedMin = s.Serialize<float>(FeatherMovementSpeedMin, name: nameof(FeatherMovementSpeedMin));

                TimeMin = s.Serialize<float>(TimeMin, name: nameof(TimeMin)); // TimeOriginalMin
                TimeMax = s.Serialize<float>(TimeMax, name: nameof(TimeMax)); // TimeOriginalMax
                TimeDeathMin = s.Serialize<float>(TimeDeathMin, name: nameof(TimeDeathMin));
                TimeDeathMax = s.Serialize<float>(TimeDeathMax, name: nameof(TimeDeathMax));
                TimeBirthMin = s.Serialize<float>(TimeBirthMin, name: nameof(TimeBirthMin));
                TimeBirthMax = s.Serialize<float>(TimeBirthMax, name: nameof(TimeBirthMax));

                Acceleration = s.SerializeObject(Acceleration, name: nameof(Acceleration));

                Friction = s.Serialize<float>(Friction, name: nameof(Friction)); // FrictionTranslationOriginal
                SizeDeathFactor = s.Serialize<float>(SizeDeathFactor, name: nameof(SizeDeathFactor));

                BirthColor = s.SerializeObject<Jade_Color>(BirthColor, name: nameof(BirthColor));
                Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
                ColorEndLife = s.SerializeObject<Jade_Color>(ColorEndLife, name: nameof(ColorEndLife));
                DeathColor = s.SerializeObject<Jade_Color>(DeathColor, name: nameof(DeathColor));

                if ((MoreFlags & 0x200000) != 0) {
                    BirthColor2 = s.SerializeObject<Jade_Color>(BirthColor2, name: nameof(BirthColor2));
                    Color2 = s.SerializeObject<Jade_Color>(Color2, name: nameof(Color2));
                    ColorEndLife2 = s.SerializeObject<Jade_Color>(ColorEndLife2, name: nameof(ColorEndLife2));
                    DeathColor2 = s.SerializeObject<Jade_Color>(DeathColor2, name: nameof(DeathColor2));
                }

                ZMin = s.Serialize<float>(ZMin, name: nameof(ZMin));
                ZMinStrength = s.Serialize<float>(ZMinStrength, name: nameof(ZMinStrength));
                ZMax = s.Serialize<float>(ZMax, name: nameof(ZMax));
                ZMaxStrength = s.Serialize<float>(ZMaxStrength, name: nameof(ZMaxStrength));
                RotationMin = s.Serialize<float>(RotationMin, name: nameof(RotationMin));
                RotationMax = s.Serialize<float>(RotationMax, name: nameof(RotationMax));
                RotationSpeedMin = s.Serialize<float>(RotationSpeedMin, name: nameof(RotationSpeedMin));
                RotationSpeedMax = s.Serialize<float>(RotationSpeedMax, name: nameof(RotationSpeedMax));


				FramesCountX = s.Serialize<byte>(FramesCountX, name: nameof(FramesCountX));
				FramesCountY = s.Serialize<byte>(FramesCountY, name: nameof(FramesCountY));
                AnimTicksCountPerAnimFrame = s.Serialize<byte>(AnimTicksCountPerAnimFrame, name: nameof(AnimTicksCountPerAnimFrame));

                GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();

                EndLifeSpawnGenerator = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(EndLifeSpawnGenerator, name: nameof(EndLifeSpawnGenerator))?.Resolve();
                AmountToSpawnOnEndLife = s.Serialize<ushort>(AmountToSpawnOnEndLife, name: nameof(AmountToSpawnOnEndLife));
                DeathSpawnGeneratorA = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(DeathSpawnGeneratorA, name: nameof(DeathSpawnGeneratorA))?.Resolve();
                AmountToSpawnOnDeathA = s.Serialize<ushort>(AmountToSpawnOnDeathA, name: nameof(AmountToSpawnOnDeathA));
                if (Version >= 16) {
                    DeathSpawnGeneratorB = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(DeathSpawnGeneratorB, name: nameof(DeathSpawnGeneratorB))?.Resolve();
                    AmountToSpawnOnDeathA = s.Serialize<ushort>(AmountToSpawnOnDeathB, name: nameof(AmountToSpawnOnDeathB));
                }
                if (Version >= 15) {
                    Particle3DObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Particle3DObject, name: nameof(Particle3DObject))?.Resolve();

                    RotationMinY = s.Serialize<float>(RotationMinY, name: nameof(RotationMinY));
                    RotationMaxY = s.Serialize<float>(RotationMaxY, name: nameof(RotationMaxY));
                    RotationSpeedMinY = s.Serialize<float>(RotationSpeedMinY, name: nameof(RotationSpeedMinY));
                    RotationSpeedMaxY = s.Serialize<float>(RotationSpeedMaxY, name: nameof(RotationSpeedMaxY));
                    RotationMinZ = s.Serialize<float>(RotationMinZ, name: nameof(RotationMinZ));
                    RotationMaxZ = s.Serialize<float>(RotationMaxZ, name: nameof(RotationMaxZ));
                    RotationSpeedMinZ = s.Serialize<float>(RotationSpeedMinZ, name: nameof(RotationSpeedMinZ));
                    RotationSpeedMaxZ = s.Serialize<float>(RotationSpeedMaxZ, name: nameof(RotationSpeedMaxZ));
                }
                if (Version >= 17) {
					SpecialLookAtX = s.Serialize<byte>(SpecialLookAtX, name: nameof(SpecialLookAtX));
					SpecialLookAtY = s.Serialize<byte>(SpecialLookAtY, name: nameof(SpecialLookAtY));
                }
            }


            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                if (Version >= 18) SoundID = s.Serialize<uint>(SoundID, name: nameof(SoundID));
				if (Version >= 19) PoolsCount = s.Serialize<int>(PoolsCount, name: nameof(PoolsCount));
                if (Version >= 22) {
                    AttractorType = s.Serialize<byte>(AttractorType, name: nameof(AttractorType));
					if (Version >= 28) Attractor = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Attractor, name: nameof(Attractor));
                    if (Attractor != null && !s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PhoenixRayman4)) {
                        Attractor?.Resolve();
                    }
					AttractorParam1 = s.Serialize<float>(AttractorParam1, name: nameof(AttractorParam1));
                    AttractorParam2 = s.Serialize<float>(AttractorParam2, name: nameof(AttractorParam2));
                    AttractorParam3 = s.Serialize<float>(AttractorParam3, name: nameof(AttractorParam3));
                    AttractorParam4 = s.Serialize<float>(AttractorParam4, name: nameof(AttractorParam4));
                    AttractorParam5 = s.Serialize<float>(AttractorParam5, name: nameof(AttractorParam5));
                    if (Version >= 23) AttractorParam6 = s.Serialize<float>(AttractorParam6, name: nameof(AttractorParam6));
					AttractorVector1 = s.SerializeObject<Jade_Vector>(AttractorVector1, name: nameof(AttractorVector1));
				}
                if (Version >= 24) {
					DispersionTransForceDamping = s.Serialize<float>(DispersionTransForceDamping, name: nameof(DispersionTransForceDamping));
					DispersionRotForceDamping = s.Serialize<float>(DispersionRotForceDamping, name: nameof(DispersionRotForceDamping));
					FPSCalibration = s.Serialize<float>(FPSCalibration, name: nameof(FPSCalibration));
                    if (Version >= 25) {
						FrontForceMax = s.Serialize<float>(FrontForceMax, name: nameof(FrontForceMax));
						RearForceMax = s.Serialize<float>(RearForceMax, name: nameof(RearForceMax));
					}
				}
                if ((MoreFlags & 0x800) != 0) {
					LODMinPercentageOfParticles = s.Serialize<float>(LODMinPercentageOfParticles, name: nameof(LODMinPercentageOfParticles));
				} else {
                    LODMinPercentageOfParticles = 0.5f;
                }
			}
        }
    }
}