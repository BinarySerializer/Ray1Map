namespace Ray1Map.Jade {
	// TODO: Use similar system to AI_Links - this array is probably different for other games
	public enum MDF_ModifierType : int {
		None = -1,
		GEO_ModifierSnap = 0,
		GEO_ModifierOnduleTonCorps = 1,
		GAO_ModifierExplode = 2,
		GAO_ModifierLegLink = 3,
		GEO_ModifierMorphing = 4,
		GAO_ModifierSemiLookAt = 5,
		GAO_ModifierShadow = 6,
		GAO_ModifierSpecialLookAt = 7,
		GEN_ModifierSound = 8,
		GAO_ModifierXMEN = 9,
		GAO_ModifierXMEC = 10,
		SPG_Modifier = 11,
		GEO_ModifierSymetrie = 12,
		GAO_ModifierROTR = 13,
		GAO_ModifierSNAKE = 14,
		GEN_ModifierSoundFx = 15,
		PROTEX_Modifier = 16,
		_Unknown = 17,
		MPAG_Modifier = 18,
		MDF_LoadingSound = 19,
		GAO_ModifierPhoto = 20,
		GEO_ModifierSTP = 21,
		GEO_ModifierCrush = 22,
		GEO_ModifierRLICarte = 23,
		GAO_ModifierLazy = 24,

		// Introduced after BG&E
		GPG_Modifier = 25,
		FUR_Modifier = 26,
		GEO_ModifierPerturb = 27,
		SPG2_Modifier = 28,
		GAO_ModifierODE = 29,
		MatrixBore_Modifier = 30,
		Grid_Modifier = 31,
		SND_ModifierSoundVol = 32,

		// Xenon modifiers
		WATER3D_Modifier = 33, // Xenon only
		Disturber_Modifier = 34, // Xenon only
		GAO_ModifierSfx = 35, // Xenon only

		GAO_ModifierRotationPaste = 36,
		GAO_ModifierTranslationPaste = 37,
		GAO_ModifierAnimatedGAO = 38,
		
		MDF_ModifierWeather = 39, // Xenon only
		GAO_ModifierSoftBody = 40, // Xenon only
		GAO_ModifierWind = 41, // Xenon only
		FUR_ModifierDynFur = 42, // Xenon only
		SPG2_ModifierHolder = 43, // Xenon only

		VINE_Modifier = 49,
		GAO_ModifierFOGDY = 50,
		GAO_ModifierFOGDY_Emtr = 51,
		GAO_ModifierBoneRefine = 52,
		GAO_ModifierBoneMeca = 53,
		FCLONE_Modifier = 54,
		UVTexWave_Modifier = 55,

		Modifier56 = 56,
		Modifier57 = 57,

		GAO_ModifierCharacterFX = 58,
	}
}
