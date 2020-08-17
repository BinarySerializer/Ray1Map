namespace R1Engine
{
    // All names starting with "TYPE_" come from the GBA version. All names starting with "MS_" come from Designer. The rest are manually added.

    /// <summary>
    /// The available event types
    /// </summary>
    public enum R1_EventType : ushort
    {
        TYPE_BADGUY1 = 0,
        TYPE_PLATFORM = 1,
        TYPE_POWERUP = 2,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_LIDOLPINK = 3,

        TYPE_NEUTRAL = 4,
        TYPE_WIZARD1 = 5,
        TYPE_FALLING_YING_OUYE = 6,
        TYPE_MORNINGSTAR = 7,
        TYPE_FALLING_OBJ = 8,
        TYPE_BADGUY2 = 9,
        TYPE_FISH = 10,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_BOUM = 11,

        TYPE_CHASSEUR1 = 12,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_BALLE1 = 13,

        TYPE_CHASSEUR2 = 14,
        TYPE_BALLE2 = 15,
        TYPE_FALLPLAT = 16,
        TYPE_LIFTPLAT = 17,
        TYPE_BTBPLAT = 18,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_SPLASH = 19,
        
        TYPE_GENEBADGUY = 20,
        TYPE_PHOTOGRAPHE = 21,
        TYPE_MOVE_PLAT = 22,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_RAYMAN = 23,

        TYPE_INTERACTPLT = 24,
        TYPE_INST_PLAT = 25,
        TYPE_CRUMBLE_PLAT = 26,
        TYPE_BOING_PLAT = 27,
        TYPE_ONOFF_PLAT = 28,
        TYPE_AUTOJUMP_PLAT = 29,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_AUDIOSTART = 30,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_ONEUP_ALWAYS = 31,

        TYPE_DARK_PHASE2 = 32,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_DARK2_SORT = 33,
        
        TYPE_MOVE_AUTOJUMP_PLAT = 34,
        TYPE_STONEMAN1 = 35,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_STONEBOMB = 36,
        
        TYPE_TARZAN = 37,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_GRAINE = 38,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_NEN_GRAINE = 39,
        
        TYPE_STONEDOG = 40,
        TYPE_OUYE = 41,
        TYPE_SIGNPOST = 42,
        TYPE_STONEMAN2 = 43,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_CLASH = 44,
        
        TYPE_MOVE_OUYE = 45,
        TYPE_BB1 = 46,
        TYPE_STONEBOMB2 = 47,
        TYPE_FLAMME2 = 48,
        TYPE_MOVE_START_PLAT = 49,
        TYPE_MOSKITO = 50,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_MST_FRUIT1 = 51,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_MST_FRUIT2 = 52,
        
        TYPE_MST_SHAKY_FRUIT = 53,
        TYPE_MEDAILLON = 54,
        TYPE_MUS_WAIT = 55,
        TYPE_STONEWOMAN2 = 56,
        TYPE_STALAG = 57,
        TYPE_CAGE = 58,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_CAGE2 = 59,
        
        TYPE_BIG_CLOWN = 60,
        TYPE_WAT_CLOWN = 61,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_DROP = 62,
        
        TYPE_MOVE_START_NUA = 63,
        
        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_SCROLL = 64,
        
        TYPE_SPIDER = 65,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_DARD = 66,

        TYPE_SWING_PLAT = 67,
        TYPE_BIG_BOING_PLAT = 68,
        TYPE_STONEBOMB3 = 69,
        TYPE_TROMPETTE = 70,
        TYPE_NOTE = 71,
        TYPE_PIRATE_NGAWE = 72,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_RING = 73,
        
        TYPE_SAXO = 74,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_PAILLETTE = 75,
        
        TYPE_DESTROYING_DOOR = 76,
        TYPE_PIRATE_GUETTEUR = 77,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_PIRATE_BOMB = 78,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_STONECHIP = 79,
        
        TYPE_BIGSTONE = 80,
        TYPE_CYMBALE = 81,
        TYPE_JAUGEUP = 82,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_EXPLOSION = 83,
        
        TYPE_TIBETAIN = 84,
        TYPE_ROLLING_EYES = 85,
        TYPE_MARACAS = 86,
        TYPE_TAMBOUR1 = 87,
        TYPE_TAMBOUR2 = 88,
        TYPE_JOE = 89,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_NOTE0 = 90,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_NOTE1 = 91,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_NOTE2 = 92,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_BONNE_NOTE = 93,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_POING = 94,
        
        TYPE_POING_POWERUP = 95,
        TYPE_TOTEM = 96,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_BBL = 97,
        
        TYPE_SPACE_MAMA = 98,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_RAY_POS = 99,
        
        TYPE_MITE = 100,
        TYPE_MORNINGSTAR_MOUNTAI = 101,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_BNOTE = 102,
        
        TYPE_POI1 = 103,
        TYPE_POI2 = 104,
        TYPE_MARTEAU = 105,
        TYPE_MOVE_MARTEAU = 106,
        TYPE_GROSPIC = 107,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_DARK2_PINK_FLY = 108,
        
        TYPE_PI = 109,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_PI_BOUM = 110,
        
        TYPE_PI_MUS = 111,
        TYPE_WASHING_MACHINE = 112,
        TYPE_BAG1 = 113,
        TYPE_UNUSED_114 = 114,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_BB1_PLAT = 115,
        
        TYPE_CLOWN_TNT = 116,
        TYPE_CLOWN_TNT2 = 117,
        TYPE_CLOWN_TNT3 = 118,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_TNT_BOMB = 119,
        
        TYPE_BATTEUR_FOU = 120,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_ECLAIR = 121,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_STONEDOG2 = 122,
        
        TYPE_BLACKTOON1 = 123,
        TYPE_PANCARTE = 124,
        TYPE_BON3 = 125,
        TYPE_FOURCHETTE = 126,
        TYPE_COUTEAU_SUISSE = 127,
        TYPE_TIRE_BOUCHON = 128,
        TYPE_PETIT_COUTEAU = 129,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_BLACKTOON_EYES = 130,
        
        TYPE_BAG3 = 131,
        TYPE_POI3 = 132,
        TYPE_SUPERHELICO = 133,
        TYPE_FALLING_OBJ2 = 134,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_ETINC = 135,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_DEMI_RAYMAN = 136,
        
        TYPE_REDUCTEUR = 137,
        TYPE_ROULETTE = 138,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_MARACAS_BAS = 139,
        
        TYPE_PT_GRAPPIN = 140,
        
        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_NEIGE = 141,
        
        TYPE_ONEUP = 142,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_NOVA2 = 143,
        
        TYPE_LIDOLPINK2 = 144,
        TYPE_KILLING_EYES = 145,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_FLASH = 146,
        
        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_MST_SCROLL = 147,
        
        TYPE_GRAP_BONUS = 148,
        TYPE_CLE_SOL = 149,
        TYPE_SCORPION = 150,
        TYPE_BULLET = 151,
        TYPE_CAISSE_CLAIRE = 152,
        TYPE_FEE = 153,
        TYPE_ROULETTE2 = 154,
        TYPE_ROULETTE3 = 155,
        TYPE_WALK_NOTE_1 = 156,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_EAU = 157,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_PALETTE_SWAPPER = 158,
        
        TYPE_TIBETAIN_6 = 159,
        TYPE_TIBETAIN_2 = 160,
        TYPE_WIZ = 161,
        TYPE_UFO_IDC = 162,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_INDICATOR = 163,
        
        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_GENERATING_DOOR = 164,
        
        TYPE_BADGUY3 = 165,
        TYPE_LEVIER = 166,
        TYPE_FALLING_OBJ3 = 167,
        TYPE_CYMBAL1 = 168,
        TYPE_CYMBAL2 = 169,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_RAYON = 170,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_MST_COPAIN = 171,
        
        TYPE_STONEWOMAN = 172,
        TYPE_BATEAU = 173,
        TYPE_PIRATE_POELLE = 174,
        TYPE_PUNAISE1 = 175,
        TYPE_CRAYON_BAS = 176,
        TYPE_FALLING_YING = 177,
        TYPE_HERSE_BAS = 178,
        TYPE_HERSE_BAS_NEXT = 179,
        TYPE_SAXO2 = 180,
        
        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_SCROLL_SAX = 181,
        
        TYPE_NOTE3 = 182,
        TYPE_SAXO3 = 183,
        TYPE_PIRATE_POELLE_D = 184,
        TYPE_WALK_NOTE_2 = 185,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_POELLE_ALWAYS = 186,
        
        TYPE_MAMA_PIRATE = 187,
        TYPE_RUBIS = 188,
        TYPE_MOVE_RUBIS = 189,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_COUTEAU = 190,
        
        TYPE_FALLING_CRAYON = 191,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_SMA_GRAND_LASER = 192,
        
        TYPE_SMA_BOMB = 193,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_SMA_BOMB_CHIP = 194,
        
        TYPE_SPIDER_PLAFOND = 195,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_DARD_PLAFOND = 196,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_MEDAILLON_TOON = 197,
        
        TYPE_BB12 = 198,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_BB1_VIT = 199,
        
        TYPE_BB13 = 200,
        TYPE_BB14 = 201,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_RAY_ETOILES = 202,
        
        TYPE_SMA_WEAPON = 203,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_BLACK_RAY = 204,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_BLACK_FIST = 205,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_PIEDS_RAYMAN = 206,
        
        TYPE_POELLE = 207,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_LANDING_SMOKE = 208,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_FIRE_LEFT = 209,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_FIRE_RIGHT = 210,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_BOUT_TOTEM = 211,
        
        TYPE_DARK = 212,
        TYPE_SPACE_MAMA2 = 213,
        TYPE_BOUEE_JOE = 214,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_DARK_SORT = 215,
        
        TYPE_ENS = 216,
        TYPE_MITE2 = 217,
        TYPE_HYBRIDE_MOSAMS = 218,
        TYPE_CORDE = 219,
        TYPE_PIERREACORDE = 220,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_CFUMEE = 221,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_CORDEBAS = 222,
        
        TYPE_HYBRIDE_STOSKO = 223,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_STOSKO_PINCE = 224,
        
        TYPE_PIRATE_P_45 = 225,
        TYPE_PIRATE_P_D_45 = 226,
        TYPE_MOSKITO2 = 227,
        TYPE_PRI = 228,
        TYPE_PUNAISE2 = 229,
        TYPE_PUNAISE3 = 230,
        TYPE_HYB_BBF2_D = 231,
        TYPE_HYB_BBF2_G = 232,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_HYB_BBF2_LAS = 233,
        
        TYPE_LAVE = 234,
        TYPE_PUNAISE4 = 235,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        TYPE_ANNULE_SORT_DARK = 236,
        
        TYPE_GOMME = 237,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_POING_FEE = 238,

        TYPE_PIRATE_GUETTEUR2 = 239,
        TYPE_CRAYON_HAUT = 240,
        TYPE_HERSE_HAUT = 241,
        TYPE_HERSE_HAUT_NEXT = 242,
        TYPE_MARK_AUTOJUMP_PLAT = 243,

        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_SMA_PETIT_LASER = 244,
        
        TYPE_DUNE = 245,
        TYPE_CORDE_DARK = 246,
        TYPE_VITRAIL = 247,
        
        [ObjTypeInfo(ObjTypeFlag.Always)]
        TYPE_SKO_PINCE = 248,
        
        TYPE_RIDEAU = 249,
        TYPE_PUNAISE5 = 250,
        TYPE_VAGUE_DEVANT = 251,
        TYPE_VAGUE_DERRIERE = 252,

        TYPE_PLANCHES = 253,
        TYPE_SLOPEY_PLAT = 254,

        UnknownType = 255,

        TYPE_CB_BRIK = 256,
        TYPE_CB_BALL = 257,
        TYPE_BONBON_PLAT = 258,

        // EDU

        TYPE_EDU_LETTRE = 259,
        TYPE_EDU_CHIFFRE = 260,
        TYPE_EDU_DIRECTION = 261,

        EDU_ArtworkObject = 262,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        EDU_VoiceLine = 263,

        EDU_Glow = 264,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        EDU_MOT = 265,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        MS_compteur = 266,

        // TODO: This is different in the EDU games!
        MS_wiz_comptage = 267,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        EDU_LogicOperator = 268,

        EDU_Magician = 269,

        EDU_Betilla = 270,

        MS_champ_fixe = 271,
        MS_pap = 272,
        MS_nougat = 273,

        Unk10 = 274,

        MS_scintillement = 275,
        MS_porte = 276,
        MS_poing_plate_forme = 277,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        MS_super_gendoor = 278,

        [ObjTypeInfo(ObjTypeFlag.Editor)]
        MS_super_kildoor = 279
    }
}