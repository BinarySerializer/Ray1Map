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

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			TotalParticleManagers = s.Serialize<int>(TotalParticleManagers, name: nameof(TotalParticleManagers));
			TotalAlphaManagers = s.Serialize<int>(TotalAlphaManagers, name: nameof(TotalAlphaManagers));
			if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PhoenixRayman4)) {
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
			if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PhoenixRayman4)) {
				if (Version >= 2) BeamManagers = s.SerializeObjectArray<CharacterFX_BeamManager>(BeamManagers, TotalBeamManagers, onPreSerialize: m => m.CharacterFX = this, name: nameof(BeamManagers));
				if (Version >= 3) SoundManagers = s.SerializeObjectArray<CharacterFX_SoundManager>(SoundManagers, TotalSoundManagers, onPreSerialize: m => m.CharacterFX = this, name: nameof(SoundManagers));
				throw new NotImplementedException($"TODO: Implement {GetType()}");
			}
			throw new NotImplementedException($"TODO: Implement {GetType()}");
		}

		public class CharacterFX_ParticleManager : BinarySerializable {
			public GAO_ModifierCharacterFX CharacterFX { get; set; }

			public int Id { get; set; }
			public int AnimationChannelID { get; set; }
			public ParticleFlags Flags { get; set; }
			public Jade_Vector ParticleManagerOffset { get; set; }
			public Jade_Vector OrientationOffset { get; set; }
			public string Name { get; set; }
			public Jade_Key PAG { get; set; }
			public uint TargetObject { get; set; }
			public int AddToCameraSpace { get; set; } // Boolean
			public int UseParentMatrix { get; set; } // Boolean
			public int OwnerIsGenerator { get; set; } // Boolean

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				Id = s.Serialize<int>(Id, name: nameof(Id));
				AnimationChannelID = s.Serialize<int>(AnimationChannelID, name: nameof(AnimationChannelID));
				Flags = s.Serialize<ParticleFlags>(Flags, name: nameof(Flags));
				ParticleManagerOffset = s.SerializeObject<Jade_Vector>(ParticleManagerOffset, name: nameof(ParticleManagerOffset));
				OrientationOffset = s.SerializeObject<Jade_Vector>(OrientationOffset, name: nameof(OrientationOffset));
				if(!Loader.IsBinaryData) Name = s.SerializeString(Name, 0x80, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				PAG = s.SerializeObject<Jade_Key>(PAG, name: nameof(PAG));
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
			public Jade_Vector BeamManagerOffset { get; set; }
			public Jade_Vector OrientationOffset { get; set; }
			public string Name { get; set; }
			public Jade_Key Beam { get; set; }
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
				BeamManagerOffset = s.SerializeObject<Jade_Vector>(BeamManagerOffset, name: nameof(BeamManagerOffset));
				OrientationOffset = s.SerializeObject<Jade_Vector>(OrientationOffset, name: nameof(OrientationOffset));
				if (!Loader.IsBinaryData) Name = s.SerializeString(Name, 0x80, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				Beam = s.SerializeObject<Jade_Key>(Beam, name: nameof(Beam));
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
	}
}
