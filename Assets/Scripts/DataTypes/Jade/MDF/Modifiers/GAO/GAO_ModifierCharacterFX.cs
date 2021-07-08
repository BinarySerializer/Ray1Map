using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierCharacterFX : MDF_Modifier {
		public uint Version { get; set; }
		public int TotalParticleManagers { get; set; }
		public int TotalAlphaManagers { get; set; }
		public int TotalBeamManagers { get; set; }
		public int TotalSoundManagers { get; set; }
		public int TotalAfterEffectManagers { get; set; }
		public int TotalLightManagers { get; set; }
		public int TotalGroupManagers { get; set; }
		public int TotalBeamInstances { get; set; }
		public int MaterialID { get; set; }

		public CharacterFX_ParticleManager[] ParticleManagers { get; set; }
		public CharacterFX_AlphaManager[] AlphaManagers { get; set; }
		public CharacterFX_BeamManager[] BeamManagers { get; set; }
		public CharacterFX_SoundManager[] SoundManagers { get; set; }
		public CharacterFX_AfterEffectManager[] AfterEffectManagers { get; set; }
		public CharacterFX_LightManager[] LightManagers { get; set; }
		public CharacterFX_GroupManager[] GroupManagers { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			TotalParticleManagers = s.Serialize<int>(TotalParticleManagers, name: nameof(TotalParticleManagers));
			TotalAlphaManagers = s.Serialize<int>(TotalAlphaManagers, name: nameof(TotalAlphaManagers));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_TMNT)) {
				if (Version >= 2) TotalBeamManagers = s.Serialize<int>(TotalBeamManagers, name: nameof(TotalBeamManagers));
				if (Version >= 3) TotalSoundManagers = s.Serialize<int>(TotalSoundManagers, name: nameof(TotalSoundManagers));
				if (Version >= 5) TotalAfterEffectManagers = s.Serialize<int>(TotalAfterEffectManagers, name: nameof(TotalAfterEffectManagers));
				if (Version >= 32) TotalLightManagers = s.Serialize<int>(TotalLightManagers, name: nameof(TotalLightManagers));
			}
			TotalGroupManagers = s.Serialize<int>(TotalGroupManagers, name: nameof(TotalGroupManagers));
			if (Version >= 16) {
				TotalBeamInstances = s.Serialize<int>(TotalBeamInstances, name: nameof(TotalBeamInstances));
			} else {
				TotalBeamInstances = 10;
			}
			if (Version >= 26) MaterialID = s.Serialize<int>(MaterialID, name: nameof(MaterialID));

			ParticleManagers = s.SerializeObjectArray<CharacterFX_ParticleManager>(ParticleManagers, TotalParticleManagers, onPreSerialize: m => m.CharacterFX = this, name: nameof(ParticleManagers));
			AlphaManagers = s.SerializeObjectArray<CharacterFX_AlphaManager>(AlphaManagers, TotalAlphaManagers, onPreSerialize: m => m.CharacterFX = this, name: nameof(AlphaManagers));
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_TMNT)) {
				if (Version >= 2) BeamManagers = s.SerializeObjectArray<CharacterFX_BeamManager>(BeamManagers, TotalBeamManagers, onPreSerialize: m => m.CharacterFX = this, name: nameof(BeamManagers));
				if (Version >= 3) SoundManagers = s.SerializeObjectArray<CharacterFX_SoundManager>(SoundManagers, TotalSoundManagers, onPreSerialize: m => m.CharacterFX = this, name: nameof(SoundManagers));
				if (Version >= 5) AfterEffectManagers = s.SerializeObjectArray<CharacterFX_AfterEffectManager>(AfterEffectManagers, TotalAfterEffectManagers, onPreSerialize: m => m.CharacterFX = this, name: nameof(AfterEffectManagers));
				if (Version >= 32) LightManagers = s.SerializeObjectArray<CharacterFX_LightManager>(LightManagers, TotalLightManagers, onPreSerialize: m => m.CharacterFX = this, name: nameof(LightManagers));
			}
			GroupManagers = s.SerializeObjectArray<CharacterFX_GroupManager>(GroupManagers, TotalGroupManagers, onPreSerialize: m => m.CharacterFX = this, name: nameof(GroupManagers));
			if (GroupManagers != null) {
				foreach(var g in GroupManagers) g?.SerializeElements(s);
			}
		}

		public class CharacterFX_ParticleManager : BinarySerializable {
			public GAO_ModifierCharacterFX CharacterFX { get; set; }

			public int Id { get; set; }
			public int AnimationChannelID { get; set; }
			public ParticleFlags Flags { get; set; }
			public Jade_Vector ParticleOffset { get; set; }
			public Jade_Vector OrientationOffset { get; set; }
			public string Name { get; set; }
			public Jade_Reference<OBJ_GameObject> PAG { get; set; }
			public uint TargetObject { get; set; }
			public int AddToCameraSpace { get; set; } // Boolean
			public int UseParentMatrix { get; set; } // Boolean
			public int OwnerIsGenerator { get; set; } // Boolean

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				Id = s.Serialize<int>(Id, name: nameof(Id));
				AnimationChannelID = s.Serialize<int>(AnimationChannelID, name: nameof(AnimationChannelID));
				Flags = s.Serialize<ParticleFlags>(Flags, name: nameof(Flags));
				ParticleOffset = s.SerializeObject<Jade_Vector>(ParticleOffset, name: nameof(ParticleOffset));
				OrientationOffset = s.SerializeObject<Jade_Vector>(OrientationOffset, name: nameof(OrientationOffset));
				if(!Loader.IsBinaryData) Name = s.SerializeString(Name, 0x80, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				PAG = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(PAG, name: nameof(PAG))?.Resolve();
				if (CharacterFX.Version >= 30) TargetObject = s.Serialize<uint>(TargetObject, name: nameof(TargetObject));
				if (CharacterFX.Version >= 23) AddToCameraSpace = s.Serialize<int>(AddToCameraSpace, name: nameof(AddToCameraSpace));
				if (CharacterFX.Version >= 25) UseParentMatrix = s.Serialize<int>(UseParentMatrix, name: nameof(UseParentMatrix));
				if (CharacterFX.Version >= 28) OwnerIsGenerator = s.Serialize<int>(OwnerIsGenerator, name: nameof(OwnerIsGenerator));
			}

			[Flags]
			public enum ParticleFlags : uint {
				None = 0,
				Frozen = 1 << 0,
				Trigger = 1 << 1,
				Active = 1 << 2,
				StopRequested = 1 << 3,
				// Rest is unused
			}
		}

		public class CharacterFX_AlphaManager : BinarySerializable {
			public GAO_ModifierCharacterFX CharacterFX { get; set; }

			public int Id { get; set; }
			public int MaterialId { get; set; }
			public int LayerId { get; set; }
			public AlphaFlags Flags { get; set; }
			public int MinAlpha { get; set; }
			public int MaxAlpha { get; set; }
			public float Duration { get; set; }
			public string Name { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				Id = s.Serialize<int>(Id, name: nameof(Id));
				MaterialId = s.Serialize<int>(MaterialId, name: nameof(MaterialId));
				LayerId = s.Serialize<int>(LayerId, name: nameof(LayerId));
				Flags = s.Serialize<AlphaFlags>(Flags, name: nameof(Flags));
				MinAlpha = s.Serialize<int>(MinAlpha, name: nameof(MinAlpha));
				MaxAlpha = s.Serialize<int>(MaxAlpha, name: nameof(MaxAlpha));
				Duration = s.Serialize<float>(Duration, name: nameof(Duration));
				if (!Loader.IsBinaryData) Name = s.SerializeString(Name, 0x80, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			}

			[Flags]
			public enum AlphaFlags : uint {
				None = 0,
				Frozen = 1 << 0,
				Trigger = 1 << 1,
				EnableLayerAtStart = 1 << 2,
				DisableLayerAtEnd = 1 << 3,
				Active = 1 << 4,
				ValidBackupInfo = 1 << 5,
				ApplyToAlphaTest = 1 << 6,
				ApplyToLocalAlpha = 1 << 7,
				// Rest is unused
			}
		}

		public class CharacterFX_BeamManager : BinarySerializable {
			public GAO_ModifierCharacterFX CharacterFX { get; set; }

			public int Id { get; set; }
			public int AnimationChannelID { get; set; }
			public BeamFlags Flags { get; set; }
			public Jade_Vector BeamOffset { get; set; }
			public Jade_Vector OrientationOffset { get; set; }
			public string Name { get; set; }
			public Jade_Reference<OBJ_GameObject> Beam { get; set; }
			public int GrowEffect { get; set; } // Boolean
			public float BirthTime { get; set; }
			public float LifeTime { get; set; }
			public float DeathTime { get; set; }
			public Jade_Vector Scale { get; set; }
			public BeamBools Bools { get; set; }
			public float NoiseAmplitude { get; set; }
			public float NoiseSpeed { get; set; }
			public int ActionToStart { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				Id = s.Serialize<int>(Id, name: nameof(Id));
				AnimationChannelID = s.Serialize<int>(AnimationChannelID, name: nameof(AnimationChannelID));
				Flags = s.Serialize<BeamFlags>(Flags, name: nameof(Flags));
				BeamOffset = s.SerializeObject<Jade_Vector>(BeamOffset, name: nameof(BeamOffset));
				OrientationOffset = s.SerializeObject<Jade_Vector>(OrientationOffset, name: nameof(OrientationOffset));
				if (!Loader.IsBinaryData) Name = s.SerializeString(Name, 0x80, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				Beam = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Beam, name: nameof(Beam))?.Resolve();
				if (CharacterFX.Version < 4) GrowEffect = s.Serialize<int>(GrowEffect, name: nameof(GrowEffect));
				BirthTime = s.Serialize<float>(BirthTime, name: nameof(BirthTime));
				LifeTime = s.Serialize<float>(LifeTime, name: nameof(LifeTime));
				DeathTime = s.Serialize<float>(DeathTime, name: nameof(DeathTime));
				if (CharacterFX.Version >= 4) {
					Scale = s.SerializeObject<Jade_Vector>(Scale, name: nameof(Scale));
					Bools = s.Serialize<BeamBools>(Bools, name: nameof(Bools));
				}
				if (CharacterFX.Version >= 13) {
					NoiseAmplitude = s.Serialize<float>(NoiseAmplitude, name: nameof(NoiseAmplitude));
					NoiseSpeed = s.Serialize<float>(NoiseSpeed, name: nameof(NoiseSpeed));
				}
				if (CharacterFX.Version >= 27) ActionToStart = s.Serialize<int>(ActionToStart, name: nameof(ActionToStart));
			}

			[Flags]
			public enum BeamFlags : uint {
				None = 0,
				Frozen = 1 << 0,
				Trigger = 1 << 1,
				Active = 1 << 2,
				// Rest is unused
			}

			[Flags]
			public enum BeamBools : uint {
				None = 0,
				Attach = 1 << 0,
				GrowEffect = 1 << 1,
				OrientationOffsetIsAbsolute = 1 << 2,
				EnableNoise = 1 << 3,
				TakeIntoAccountMirror = 1 << 4,
				// Rest is unused
			}
		}

		public class CharacterFX_SoundManager : BinarySerializable {
			public GAO_ModifierCharacterFX CharacterFX { get; set; }

			public int Id { get; set; }
			public SoundFlags Flags { get; set; }
			public AudioType Type { get; set; }
			public uint Event { get; set; }
			public int StopEnabled { get; set; } // Boolean
			public uint StopEvent { get; set; }
			public float Duration { get; set; }
			public string Name { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				Id = s.Serialize<int>(Id, name: nameof(Id));
				Flags = s.Serialize<SoundFlags>(Flags, name: nameof(Flags));
				Type = s.Serialize<AudioType>(Type, name: nameof(Type));
				Event = s.Serialize<uint>(Event, name: nameof(Event));
				if (CharacterFX.Version >= 18) StopEnabled = s.Serialize<int>(StopEnabled, name: nameof(StopEnabled));
				if (CharacterFX.Version >= 10) StopEvent = s.Serialize<uint>(StopEvent, name: nameof(StopEvent));
				Duration = s.Serialize<float>(Duration, name: nameof(Duration));
				if (!Loader.IsBinaryData) Name = s.SerializeString(Name, 0x80, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			}

			[Flags]
			public enum SoundFlags : uint {
				None = 0,
				Frozen = 1 << 0,
				Trigger = 1 << 1,
				Active = 1 << 2,
				// Rest is unused
			}

			// Taken from PoP:TFS Wii
			public enum AudioType : uint {
				None = 0x0,
				Music_Exploration = 0x1,
				Music_MiniGame = 0x2,
				Music_PLV_ScriptedEvent = 0x3,
				Music_Generic = 0x4,
				Music_Konoha = 0x5,
				Music_Character = 0x6,
				Music_Menu = 0x7,
				Music_Fight = 0x8,
				Music_RGS = 0x9,
				RTC = 0xa,
				SFX_Propagated = 0xb,
				SFX_PLV_ScriptedEvent = 0xc,
				SFX_Generic = 0xd,
				SFX_Naruto = 0xe,
				SFX_Menu = 0xf,
				SFX_Fight = 0x10,
				SFX_PLV = 0x11,
				SFX_HUD = 0x12,
				Voice_ScriptedEvent = 0x13,
				Voice_Generic = 0x14,
				Voice_Narrator = 0x15,
				Voice_ScriptedDialogs = 0x16,
				Voice_AI = 0x17,
				Voice_Onos = 0x18,
				Foley_Generic = 0x19,
				Foley_MainActor = 0x1a,
				Foley_Monster = 0x1b,
				Foley_Other = 0x1c,
				Ambiance_MultitrackQuad = 0x1d,
				Ambiance_3DElements = 0x1e,
				Ambiance_2D = 0x1f,
				Volumetric = 0x20,
				Count = 0x21,
			}
		}

		public class CharacterFX_AfterEffectManager : BinarySerializable {
			public GAO_ModifierCharacterFX CharacterFX { get; set; }

			public AfterEffectFlags Flags { get; set; }
			public int Id { get; set; }
			public AfterEffectType AEType { get; set; }
			public Jade_Vector Vector { get; set; }
			public float Duration { get; set; }

			public CharacterFX_ColorOperation_Params ColorOperationParams { get; set; }
			public CharacterFX_MotionBlur_Params MotionBlurParams { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				Flags = s.Serialize<AfterEffectFlags>(Flags, name: nameof(Flags));
				Id = s.Serialize<int>(Id, name: nameof(Id));
				AEType = s.Serialize<AfterEffectType>(AEType, name: nameof(AEType));
				if (CharacterFX.Version < 31) Vector = s.SerializeObject<Jade_Vector>(Vector, name: nameof(Vector));
				Duration = s.Serialize<float>(Duration, name: nameof(Duration));

				if (CharacterFX.Version >= 14) {
					switch (AEType) {
						case AfterEffectType.ColorOperation:
							ColorOperationParams = s.SerializeObject<CharacterFX_ColorOperation_Params>(ColorOperationParams, onPreSerialize: p => p.Manager = this, name: nameof(ColorOperationParams));
							break;
						case AfterEffectType.MotionBlur:
							MotionBlurParams = s.SerializeObject<CharacterFX_MotionBlur_Params>(MotionBlurParams, onPreSerialize: p => p.Manager = this, name: nameof(MotionBlurParams));
							break;
					}
				}
			}

			[Flags]
			public enum AfterEffectFlags : uint {
				None = 0,
				Frozen = 1 << 0,
				Trigger = 1 << 1,
				Active = 1 << 2,
				// Rest is unused
			}

			// Taken from PoP:TFS Wii
			public enum AfterEffectType : uint {
				None = 0,
				ColorOperation = 1,
				MotionBlur = 2,
				Count = 3
			}


			public class CharacterFX_ColorOperation_Params : BinarySerializable {
				public CharacterFX_AfterEffectManager Manager { get; set; }

				public ColorOperationFlags Flags { get; set; }
				public float ContrastFactor { get; set; }
				public float BrightnessFactor { get; set; }
				public Jade_Vector ColorVector { get; set; }
				public Jade_Color Color { get; set; }
				public float BirthTime { get; set; }
				public float LifeTime { get; set; }
				public float DeathTime { get; set; }

				public Jade_Vector MonochromaticColorVector { get; set; }
				public Jade_Color MonochromaticColor { get; set; }
				public float ColorBalance_Intensity { get; set; }
				public float ColorBalance_Spectre { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Flags = s.Serialize<ColorOperationFlags>(Flags, name: nameof(Flags));
					ContrastFactor = s.Serialize<float>(ContrastFactor, name: nameof(ContrastFactor));
					BrightnessFactor = s.Serialize<float>(BrightnessFactor, name: nameof(BrightnessFactor));
					if (Manager.CharacterFX.Version == 14) {
						ColorVector = s.SerializeObject<Jade_Vector>(ColorVector, name: nameof(ColorVector));
					} else if (Manager.CharacterFX.Version > 14) {
						Color = s.SerializeObject<Jade_Color>(Color, name: nameof(Color));
					}
					BirthTime = s.Serialize<float>(BirthTime, name: nameof(BirthTime));
					LifeTime = s.Serialize<float>(LifeTime, name: nameof(LifeTime));
					DeathTime = s.Serialize<float>(DeathTime, name: nameof(DeathTime));

					if (Manager.CharacterFX.Version >= 17) {
						if (Manager.CharacterFX.Version >= 29) {
							MonochromaticColor = s.SerializeObject<Jade_Color>(MonochromaticColor, name: nameof(MonochromaticColor));
						} else {
							MonochromaticColorVector = s.SerializeObject<Jade_Vector>(MonochromaticColorVector, name: nameof(MonochromaticColorVector));
						}
					}
					if (Manager.CharacterFX.Version >= 24) {
						ColorBalance_Intensity = s.Serialize<float>(ColorBalance_Intensity, name: nameof(ColorBalance_Intensity));
						ColorBalance_Spectre = s.Serialize<float>(ColorBalance_Spectre, name: nameof(ColorBalance_Spectre));
					}
				}

				[Flags]
				public enum ColorOperationFlags : uint {
					None = 0,
					Contrast = 1 << 0,
					Brightness = 1 << 1,
					Colorize = 1 << 2,
					Monochromatic = 1 << 3,
					ColorBalance = 1 << 4,
					// Rest is unused
				}

				// Taken from PoP:TFS Wii
				public enum AfterEffectType : uint {
					None = 0,
					ColorOperation = 1,
					MotionBlur = 2,
					Count = 3
				}
			}

			public class CharacterFX_MotionBlur_Params : BinarySerializable {
				public CharacterFX_AfterEffectManager Manager { get; set; }

				public float Value { get; set; }
				public float Duration { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Value = s.Serialize<float>(Value, name: nameof(Value));
					Duration = s.Serialize<float>(Duration, name: nameof(Duration));
				}
			}
		}

		public class CharacterFX_LightManager : BinarySerializable {
			public GAO_ModifierCharacterFX CharacterFX { get; set; }

			public int Id { get; set; }
			public int AnimationChannelID { get; set; }
			public LightFlags Flags { get; set; }
			public Jade_Vector LightOffset { get; set; }
			public string Name { get; set; }
			public Jade_Reference<OBJ_GameObject> Target { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				Id = s.Serialize<int>(Id, name: nameof(Id));
				AnimationChannelID = s.Serialize<int>(AnimationChannelID, name: nameof(AnimationChannelID));
				Flags = s.Serialize<LightFlags>(Flags, name: nameof(Flags));
				LightOffset = s.SerializeObject<Jade_Vector>(LightOffset, name: nameof(LightOffset));
				if (!Loader.IsBinaryData) Name = s.SerializeString(Name, 0x80, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				Target = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Target, name: nameof(Target))?.Resolve();
			}

			[Flags]
			public enum LightFlags : uint {
				None = 0,
				Frozen = 1 << 0,
				Trigger = 1 << 1,
				Active = 1 << 2,
				// Rest is unused
			}
		}

		public class CharacterFX_GroupManager : BinarySerializable {
			public GAO_ModifierCharacterFX CharacterFX { get; set; }

			public int Id { get; set; }
			public short ColMatIDsCount { get; set; }
			public uint[] ColMatIDs { get; set; }
			public int SpecialFilterValue { get; set; }
			public int ActionToStartOnOwner { get; set; }
			public int NumberOfAlphaEffectsInGroup { get; set; }
			public int NumberOfParticleEffectsInGroup { get; set; }
			public int NumberOfBeamEffectsInGroup { get; set; }
			public int NumberOfSoundEffectsInGroup { get; set; }
			public int NumberOfAfterEffectsInGroup { get; set; }
			public int NumberOfLightsInGroup { get; set; }
			public GroupFlags Flags { get; set; }
			public string Name { get; set; }
			public float V22_Float { get; set; }
			public uint V2_Phoenix_Dword { get; set; }

			public CharacterFX_GroupParticleElement[] ParticleEffectElements { get; set; }
			public CharacterFX_GroupAlphaElement[] AlphaEffectElements { get; set; }
			public CharacterFX_GroupBeamElement[] BeamEffectElements { get; set; }
			public CharacterFX_GroupSoundElement[] SoundEffectElements { get; set; }
			public CharacterFX_GroupAfterEffectElement[] AfterEffectElements { get; set; }
			public CharacterFX_GroupLightElement[] LightElements { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				Id = s.Serialize<int>(Id, name: nameof(Id));
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_TMNT)) {
					if (CharacterFX.Version >= 12) {
						ColMatIDsCount = s.Serialize<short>(ColMatIDsCount, name: nameof(ColMatIDsCount));
						ColMatIDs = s.SerializeArray<uint>(ColMatIDs, ColMatIDsCount, name: nameof(ColMatIDs));
						SpecialFilterValue = s.Serialize<int>(SpecialFilterValue, name: nameof(SpecialFilterValue));
					}
					if (CharacterFX.Version >= 27) ActionToStartOnOwner = s.Serialize<int>(ActionToStartOnOwner, name: nameof(ActionToStartOnOwner));
					NumberOfAlphaEffectsInGroup = s.Serialize<short>((short)NumberOfAlphaEffectsInGroup, name: nameof(NumberOfAlphaEffectsInGroup));
					NumberOfParticleEffectsInGroup = s.Serialize<short>((short)NumberOfParticleEffectsInGroup, name: nameof(NumberOfParticleEffectsInGroup));
					if (CharacterFX.Version >= 2) NumberOfBeamEffectsInGroup = s.Serialize<short>((short)NumberOfBeamEffectsInGroup, name: nameof(NumberOfBeamEffectsInGroup));
					if (CharacterFX.Version >= 3) NumberOfSoundEffectsInGroup = s.Serialize<short>((short)NumberOfSoundEffectsInGroup, name: nameof(NumberOfSoundEffectsInGroup));
					if (CharacterFX.Version >= 5) NumberOfAfterEffectsInGroup = s.Serialize<short>((short)NumberOfAfterEffectsInGroup, name: nameof(NumberOfAfterEffectsInGroup));
					if (CharacterFX.Version >= 32) NumberOfLightsInGroup = s.Serialize<short>((short)NumberOfLightsInGroup, name: nameof(NumberOfLightsInGroup));
				} else {
					NumberOfAlphaEffectsInGroup = s.Serialize<int>(NumberOfAlphaEffectsInGroup, name: nameof(NumberOfAlphaEffectsInGroup));
					NumberOfParticleEffectsInGroup = s.Serialize<int>(NumberOfParticleEffectsInGroup, name: nameof(NumberOfParticleEffectsInGroup));
					if (CharacterFX.Version >= 2) V2_Phoenix_Dword = s.Serialize<uint>(V2_Phoenix_Dword, name: nameof(V2_Phoenix_Dword));
				}
				Flags = s.Serialize<GroupFlags>(Flags, name: nameof(Flags));
				if (!Loader.IsBinaryData) Name = s.SerializeString(Name, 0x80, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				if (CharacterFX.Version >= 22 && !Loader.IsBinaryData) V22_Float = s.Serialize<float>(V22_Float, name: nameof(V22_Float));
			}

			public void SerializeElements(SerializerObject s) {
				ParticleEffectElements = s.SerializeObjectArray<CharacterFX_GroupParticleElement>(ParticleEffectElements, Math.Max(0, NumberOfParticleEffectsInGroup), onPreSerialize: e => e.Manager = this, name: nameof(ParticleEffectElements));
				AlphaEffectElements = s.SerializeObjectArray<CharacterFX_GroupAlphaElement>(AlphaEffectElements, Math.Max(0, NumberOfAlphaEffectsInGroup), onPreSerialize: e => e.Manager = this, name: nameof(AlphaEffectElements));
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_TMNT)) {
					if (CharacterFX.Version >= 2) BeamEffectElements = s.SerializeObjectArray<CharacterFX_GroupBeamElement>(BeamEffectElements, NumberOfBeamEffectsInGroup, onPreSerialize: e => e.Manager = this, name: nameof(BeamEffectElements));
					if (CharacterFX.Version >= 3) SoundEffectElements = s.SerializeObjectArray<CharacterFX_GroupSoundElement>(SoundEffectElements, NumberOfSoundEffectsInGroup, onPreSerialize: e => e.Manager = this, name: nameof(SoundEffectElements));
					if (CharacterFX.Version >= 5) AfterEffectElements = s.SerializeObjectArray<CharacterFX_GroupAfterEffectElement>(AfterEffectElements, NumberOfAfterEffectsInGroup, onPreSerialize: e => e.Manager = this, name: nameof(AfterEffectElements));
					if (CharacterFX.Version >= 32) LightElements = s.SerializeObjectArray<CharacterFX_GroupLightElement>(LightElements, NumberOfLightsInGroup, onPreSerialize: e => e.Manager = this, name: nameof(LightElements));
				}
			}

			[Flags]
			public enum GroupFlags : uint {
				None = 0,
				VisualActive = 1 << 0,
				Frozen = 1 << 1,
				Trigger = 1 << 2,
				Active = 1 << 3,
				// Rest is unused
			}

			public abstract class CharacterFX_BaseElement : BinarySerializable {
				public virtual bool HasDuration => true;
				public CharacterFX_GroupManager Manager { get; set; }

				public int Id { get; set; }
				public float TriggerDelay { get; set; }
				public float Duration { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					Id = s.Serialize<int>(Id, name: nameof(Id));
					TriggerDelay = s.Serialize<float>(TriggerDelay, name: nameof(TriggerDelay));
					if(HasDuration) Duration = s.Serialize<float>(Duration, name: nameof(Duration));
				}
			}
			public class CharacterFX_GroupParticleElement : CharacterFX_BaseElement {

				public override bool HasDuration => Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_TMNT) || Manager.CharacterFX.Version >= 1;

				public int ForceStop { get; set; } // Boolean
				public int StopGenerating { get; set; } // Boolean

				public override void SerializeImpl(SerializerObject s) {
					base.SerializeImpl(s);
					if (Manager.CharacterFX.Version >= 20) ForceStop = s.Serialize<int>(ForceStop, name: nameof(ForceStop));
					if (Manager.CharacterFX.Version >= 21) StopGenerating = s.Serialize<int>(StopGenerating, name: nameof(StopGenerating));
				}
			}
			public class CharacterFX_GroupAlphaElement : CharacterFX_BaseElement {
				public override bool HasDuration => false;
			}
			public class CharacterFX_GroupBeamElement : CharacterFX_BaseElement { }
			public class CharacterFX_GroupSoundElement : CharacterFX_BaseElement { }
			public class CharacterFX_GroupAfterEffectElement : CharacterFX_BaseElement { }
			public class CharacterFX_GroupLightElement : CharacterFX_BaseElement { }
		}
	}
}
