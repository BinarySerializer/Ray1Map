namespace R1Engine
{
    // All names starting with "TYPE_" come from the GBA version. All names starting with "MS_" come from Designer. The rest are manually added.

    /// <summary>
    /// The available event types
    /// </summary>
    public enum EventType : ushort
    {
        TYPE_BADGUY1,
        TYPE_PLATFORM,
        TYPE_POWERUP,

        [EventTypeInfo(EventFlag.Always)]
        TYPE_LIDOLPINK,

        TYPE_NEUTRAL,
        TYPE_WIZARD1,
        TYPE_FALLING_YING_OUYE,
        TYPE_MORNINGSTAR,
        TYPE_FALLING_OBJ,
        TYPE_BADGUY2,
        TYPE_FISH,

        [EventTypeInfo(EventFlag.Always)]
        TYPE_BOUM,

        TYPE_CHASSEUR1,

        [EventTypeInfo(EventFlag.Always)]
        TYPE_BALLE1,

        TYPE_CHASSEUR2,
        TYPE_BALLE2,
        TYPE_FALLPLAT,
        TYPE_LIFTPLAT,
        TYPE_BTBPLAT,

        [EventTypeInfo(EventFlag.Always)]
        TYPE_SPLASH,
        
        TYPE_GENEBADGUY,
        TYPE_PHOTOGRAPHE,
        TYPE_MOVE_PLAT,
        TYPE_RAYMAN,
        TYPE_INTERACTPLT,
        TYPE_INST_PLAT,
        TYPE_CRUMBLE_PLAT,
        TYPE_BOING_PLAT,
        TYPE_ONOFF_PLAT,
        TYPE_AUTOJUMP_PLAT,

        [EventTypeInfo(EventFlag.Editor)]
        TYPE_AUDIOSTART,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_ONEUP_ALWAYS,

        TYPE_DARK_PHASE2,

        [EventTypeInfo(EventFlag.Always)]
        TYPE_DARK2_SORT,
        
        TYPE_MOVE_AUTOJUMP_PLAT,
        TYPE_STONEMAN1,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_STONEBOMB,
        
        TYPE_TARZAN,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_GRAINE,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_NEN_GRAINE,
        
        TYPE_STONEDOG,
        TYPE_OUYE,
        TYPE_SIGNPOST,
        TYPE_STONEMAN2,

        [EventTypeInfo(EventFlag.Always)]
        TYPE_CLASH,
        
        TYPE_MOVE_OUYE,
        TYPE_BB1,
        TYPE_STONEBOMB2,
        TYPE_FLAMME2,
        TYPE_MOVE_START_PLAT,
        TYPE_MOSKITO,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_MST_FRUIT1,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_MST_FRUIT2,
        
        TYPE_MST_SHAKY_FRUIT,
        TYPE_MEDAILLON,
        TYPE_MUS_WAIT,
        TYPE_STONEWOMAN2,
        TYPE_STALAG,
        TYPE_CAGE,

        [EventTypeInfo(EventFlag.Always)]
        TYPE_CAGE2,
        
        TYPE_BIG_CLOWN,
        TYPE_WAT_CLOWN,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_DROP,
        
        TYPE_MOVE_START_NUA,
        
        [EventTypeInfo(EventFlag.Editor)]
        TYPE_SCROLL,
        
        TYPE_SPIDER,

        [EventTypeInfo(EventFlag.Always)]
        TYPE_DARD,

        TYPE_SWING_PLAT,
        TYPE_BIG_BOING_PLAT,
        TYPE_STONEBOMB3,
        TYPE_TROMPETTE,
        TYPE_NOTE,
        TYPE_PIRATE_NGAWE,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_RING,
        
        TYPE_SAXO,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_PAILLETTE,
        
        TYPE_DESTROYING_DOOR,
        TYPE_PIRATE_GUETTEUR,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_PIRATE_BOMB,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_STONECHIP,
        
        TYPE_BIGSTONE,
        TYPE_CYMBALE,
        TYPE_JAUGEUP,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_EXPLOSION,
        
        TYPE_TIBETAIN,
        TYPE_ROLLING_EYES,
        TYPE_MARACAS,
        TYPE_TAMBOUR1,
        TYPE_TAMBOUR2,
        TYPE_JOE,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_NOTE0,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_NOTE1,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_NOTE2,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_BONNE_NOTE,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_POING,
        
        TYPE_POING_POWERUP,
        TYPE_TOTEM,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_BBL,
        
        TYPE_SPACE_MAMA,
        TYPE_RAY_POS,
        TYPE_MITE,
        TYPE_MORNINGSTAR_MOUNTAI,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_BNOTE,
        
        TYPE_POI1,
        TYPE_POI2,
        TYPE_MARTEAU,
        TYPE_MOVE_MARTEAU,
        TYPE_GROSPIC,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_DARK2_PINK_FLY,
        
        TYPE_PI,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_PI_BOUM,
        
        TYPE_PI_MUS,
        TYPE_WASHING_MACHINE,
        TYPE_BAG1,
        TYPE_UNUSED_114,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_BB1_PLAT,
        
        TYPE_CLOWN_TNT,
        TYPE_CLOWN_TNT2,
        TYPE_CLOWN_TNT3,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_TNT_BOMB,
        
        TYPE_BATTEUR_FOU,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_ECLAIR,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_STONEDOG2,
        
        TYPE_BLACKTOON1,
        TYPE_PANCARTE,
        TYPE_BON3,
        TYPE_FOURCHETTE,
        TYPE_COUTEAU_SUISSE,
        TYPE_TIRE_BOUCHON,
        TYPE_PETIT_COUTEAU,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_BLACKTOON_EYES,
        
        TYPE_BAG3,
        TYPE_POI3,
        TYPE_SUPERHELICO,
        TYPE_FALLING_OBJ2,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_ETINC,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_DEMI_RAYMAN,
        
        TYPE_REDUCTEUR,
        TYPE_ROULETTE,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_MARACAS_BAS,
        
        TYPE_PT_GRAPPIN,
        
        [EventTypeInfo(EventFlag.Editor)]
        TYPE_NEIGE,
        
        TYPE_ONEUP,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_NOVA2,
        
        TYPE_LIDOLPINK2,
        TYPE_KILLING_EYES,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_FLASH,
        
        [EventTypeInfo(EventFlag.Editor)]
        TYPE_MST_SCROLL,
        
        TYPE_GRAP_BONUS,
        TYPE_CLE_SOL,
        TYPE_SCORPION,
        TYPE_BULLET,
        TYPE_CAISSE_CLAIRE,
        TYPE_FEE,
        TYPE_ROULETTE2,
        TYPE_ROULETTE3,
        TYPE_WALK_NOTE_1,

        [EventTypeInfo(EventFlag.Editor)]
        TYPE_EAU,

        [EventTypeInfo(EventFlag.Editor)]
        TYPE_PALETTE_SWAPPER,
        
        TYPE_TIBETAIN_6,
        TYPE_TIBETAIN_2,
        TYPE_WIZ,
        TYPE_UFO_IDC,
        TYPE_INDICATOR,
        
        [EventTypeInfo(EventFlag.Editor)]
        TYPE_GENERATING_DOOR,
        
        TYPE_BADGUY3,
        TYPE_LEVIER,
        TYPE_FALLING_OBJ3,
        TYPE_CYMBAL1,
        TYPE_CYMBAL2,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_RAYON,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_MST_COPAIN,
        
        TYPE_STONEWOMAN,
        TYPE_BATEAU,
        TYPE_PIRATE_POELLE,
        TYPE_PUNAISE1,
        TYPE_CRAYON_BAS,
        TYPE_FALLING_YING,
        TYPE_HERSE_BAS,
        TYPE_HERSE_BAS_NEXT,
        TYPE_SAXO2,
        
        [EventTypeInfo(EventFlag.Editor)]
        TYPE_SCROLL_SAX,
        
        TYPE_NOTE3,
        TYPE_SAXO3,
        TYPE_PIRATE_POELLE_D,
        TYPE_WALK_NOTE_2,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_POELLE_ALWAYS,
        
        TYPE_MAMA_PIRATE,
        TYPE_RUBIS,
        TYPE_MOVE_RUBIS,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_COUTEAU,
        
        TYPE_FALLING_CRAYON,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_SMA_GRAND_LASER,
        
        TYPE_SMA_BOMB,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_SMA_BOMB_CHIP,
        
        TYPE_SPIDER_PLAFOND,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_DARD_PLAFOND,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_MEDAILLON_TOON,
        
        TYPE_BB12,

        [EventTypeInfo(EventFlag.Editor)]
        TYPE_BB1_VIT,
        
        TYPE_BB13,
        TYPE_BB14,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_RAY_ETOILES,
        
        TYPE_SMA_WEAPON,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_BLACK_RAY,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_BLACK_FIST,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_PIEDS_RAYMAN,
        
        TYPE_POELLE,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_LANDING_SMOKE,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_FIRE_LEFT,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_FIRE_RIGHT,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_BOUT_TOTEM,
        
        TYPE_DARK,
        TYPE_SPACE_MAMA2,
        TYPE_BOUEE_JOE,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_DARK_SORT,
        
        TYPE_ENS,
        TYPE_MITE2,
        TYPE_HYBRIDE_MOSAMS,
        TYPE_CORDE,
        TYPE_PIERREACORDE,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_CFUMEE,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_CORDEBAS,
        
        TYPE_HYBRIDE_STOSKO,

        [EventTypeInfo(EventFlag.Always)]
        TYPE_STOSKO_PINCE,
        
        TYPE_PIRATE_P_45,
        TYPE_PIRATE_P_D_45,
        TYPE_MOSKITO2,
        TYPE_PRI,
        TYPE_PUNAISE2,
        TYPE_PUNAISE3,
        TYPE_HYB_BBF2_D,
        TYPE_HYB_BBF2_G,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_HYB_BBF2_LAS,
        
        TYPE_LAVE,
        TYPE_PUNAISE4,

        [EventTypeInfo(EventFlag.Editor)]
        TYPE_ANNULE_SORT_DARK,
        
        TYPE_GOMME,
        TYPE_POING_FEE,
        TYPE_PIRATE_GUETTEUR2,
        TYPE_CRAYON_HAUT,
        TYPE_HERSE_HAUT,
        TYPE_HERSE_HAUT_NEXT,
        TYPE_MARK_AUTOJUMP_PLAT,

        [EventTypeInfo(EventFlag.Always)]
        TYPE_SMA_PETIT_LASER,
        
        TYPE_DUNE,
        TYPE_CORDE_DARK,
        TYPE_VITRAIL,
        
        [EventTypeInfo(EventFlag.Always)]
        TYPE_SKO_PINCE,
        
        TYPE_RIDEAU,
        TYPE_PUNAISE5,
        TYPE_VAGUE_DEVANT,
        TYPE_VAGUE_DERRIERE,

        TYPE_PLANCHES,
        TYPE_SLOPEY_PLAT,

        // This type appears between TYPE_VAGUE_DERRIERE and TYPE_CB_BRIK
        UnknownType,

        TYPE_CB_BRIK,
        TYPE_CB_BALL,
        TYPE_BONBON_PLAT,

        // EDU

        TYPE_EDU_LETTRE,
        TYPE_EDU_CHIFFRE,
        TYPE_EDU_DIRECTION,

        EDU_ArtworkObject,

        [EventTypeInfo(EventFlag.Editor)]
        EDU_VoiceLine,

        EDU_Glow,

        [EventTypeInfo(EventFlag.Editor)]
        EDU_MOT,

        [EventTypeInfo(EventFlag.Editor)]
        MS_compteur = 266,

        // TODO: This is different in the EDU games!
        MS_wiz_comptage = 267,

        [EventTypeInfo(EventFlag.Editor)]
        EDU_LogicOperator,

        EDU_Magician,

        EDU_Betilla,

        MS_champ_fixe = 271,
        MS_pap = 272,
        MS_nougat = 273,

        Unk10,

        MS_scintillement = 275,
        MS_porte = 276,
        MS_poing_plate_forme = 277,

        [EventTypeInfo(EventFlag.Editor)]
        MS_super_gendoor = 278,

        [EventTypeInfo(EventFlag.Editor)]
        MS_super_kildoor = 279
    }
}