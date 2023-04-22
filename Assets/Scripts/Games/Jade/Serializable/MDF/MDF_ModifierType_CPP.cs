namespace Ray1Map.Jade {
	// TODO: Use similar system to AI_Links - this array is probably different for other games
	public enum MDF_ModifierType_CPP : int {
		None = -1,

		MDF_WaveYourBody = 1, // OnduleTonCorps
		MDF_Morphing = 2,
		MDF_Lazy = 3,
		MDF_BoneMeca = 4,
		MDF_SemiLookAt = 5,
		MDF_Shadow = 6,
		MDF_SpecialLookAt = 7,
		MDF_BoneRefine = 8,
		MDF_XMEN = 9,

		MDF_RotR = 13,
		MDF_Snake = 14,
		MDF_Audio = 15,
		MDF_AudioAttenuator = 16,

		MDF_SoftBody = 18,
		MDF_Spring = 19,

		MDF_Water = 21,
		MDF_RotC = 22,
		MDF_BeamGen = 23,
		MDF_Disturber = 24,
		MDF_Nop = 25,
		MDF_AnimatedScale = 26,
		MDF_Wind = 27,

		MDF_Halo = 32,
		
		MDF_SectoElement = 36,
		MDF_AnimatedMaterial = 37,
		MDF_RotationPaste = 38,
		MDF_Ambiance = 39,
		MDF_AmbianceLinker = 40,
		MDF_AmbiancePocket = 41,
		MDF_Pendula = 42,
		MDF_TranslationPaste = 43,
		MDF_AnimatedPAG = 44,
		MDF_AnimatedGAO = 45,

		MDF_CharacterFX = 47,

		MDF_SoftBodyColl = 49,

		MDF_AlphaFade = 52,
		MDF_AlphaOccluder = 53,
		MDF_InteractivePlant = 54,
		MDF_PreDepthPass = 56,
		MDF_VolumetricSound = 57,
		MDF_ProceduralBone = 58,
		MDF_AudioReverbZone = 59,

		MDF_CharacterFxRef = 63,

		// Next are different per game
		MDF_EngineSplit = 64,
		MDF_SoundBank = 64, // TV party

		MDF_AnimIK = 64, // PoP: TFS, Sean White
		MDF_Melt = 65, // PoP: TFS
		MDF_AudioAmbiancePortal = 66, // PoP: TFS
		MDF_EvilPlant = 67, // PoP: TFS


		MDF_ClothDistort = 65, // Sean White
		MDF_Skybox = 66, // Sean White
		MDF_3DText = 68, // Sean White
		MDF_AudioAmbiancePortal_SW = 69, // Sean White
		MDF_RadioTrigger = 70, // Sean White
		MDF_ConstColorBlend = 74, // Sean White

	}
}
