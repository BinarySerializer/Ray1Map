using System.Collections.Generic;

namespace Ray1Map.Jade {
	public abstract class AI_Links {

		public AI_FunctionDef[] FunctionDefs { get; protected set; }

		public AI_Link[] Categories { get; protected set; }
		public AI_Link[] Types { get; protected set; }
		public AI_Link[] Keywords { get; protected set; }
		public AI_Link[] Functions { get; protected set; }
		public AI_Link[] Fields { get; protected set; }

		public Dictionary<uint, AI_Link> Links { get; protected set; }
		public Dictionary<uint, AI_FunctionDef> CompiledFunctions { get; protected set; }
		private void CreateDictionaries() {
			Links = new Dictionary<uint, AI_Link>();
			foreach (var l in Categories) Links[l.Key] = l;
			foreach (var l in Types) Links[l.Key] = l;
			foreach (var l in Keywords) Links[l.Key] = l;
			foreach (var l in Functions) Links[l.Key] = l;
			foreach (var l in Fields) Links[l.Key] = l;
			CompiledFunctions = new Dictionary<uint, AI_FunctionDef>();
			foreach(var l in FunctionDefs) CompiledFunctions[l.Key] = l;
		}

		protected void Init() {
			InitFunctionDefs();
			InitCategories();
			InitTypes();
			InitKeywords();
			InitFunctions();
			InitFields();
		}
		protected abstract void InitFunctionDefs();
		protected abstract void InitCategories();
		protected abstract void InitTypes();
		protected abstract void InitKeywords();
		protected abstract void InitFunctions();
		protected abstract void InitFields();


		public static AI_Links GetAILinks(GameSettings settings) {
			AI_Links links = null;
			switch (settings.GameModeSelection) {
				case GameModeSelection.RaymanRavingRabbidsPC:
				case GameModeSelection.RaymanRavingRabbidsPCDemo:
				case GameModeSelection.RaymanRavingRabbidsPS2:
				case GameModeSelection.RaymanRavingRabbidsWii:
				case GameModeSelection.RaymanRavingRabbidsWiiJP:
				case GameModeSelection.RaymanRavingRabbidsPCUnbinarized:
				case GameModeSelection.RaymanRavingRabbidsPCPrototype:
					links = new AI_Links_RRR_Wii();
					break;
				case GameModeSelection.RaymanRavingRabbidsXbox360:
				case GameModeSelection.RaymanRavingRabbidsXbox360_20070213:
					links = new AI_Links_RRR_Xbox360();
					break;
				case GameModeSelection.BeyondGoodAndEvilGC:
					links = new AI_Links_BGE_GC();
					break;
				case GameModeSelection.BeyondGoodAndEvilPC:
				case GameModeSelection.BeyondGoodAndEvilXbox:
				case GameModeSelection.BeyondGoodAndEvilPS2:
					links = new AI_Links_BGE_PC();
					break;
				case GameModeSelection.BeyondGoodAndEvilXbox360:
					links = new AI_Links_BGE_HD_Xbox360();
					break;
				case GameModeSelection.BeyondGoodAndEvilPS3:
					links = new AI_Links_BGE_HD_PS3();
					break;
				case GameModeSelection.BeyondGoodAndEvilPCDemo:
					links = new AI_Links_BGE_PCDemo();
					break;
				case GameModeSelection.BeyondGoodAndEvilPS2_20030814:
					links = new AI_Links_BGE_PS2_20030814();
					break;
				case GameModeSelection.BeyondGoodAndEvilPS2_20030805:
					links = new AI_Links_BGE_PS2_20030805();
					break;
				case GameModeSelection.KingKongPCGamersEdition:
					links = new AI_Links_KingKong_PCGamersEdition();
					break;
				case GameModeSelection.KingKongXbox360:
					links = new AI_Links_KingKong_Xbox360();
					break;
				case GameModeSelection.KingKongXbox360_20050926:
					links = new AI_Links_KingKong_Xbox360_20050926();
					break;
				case GameModeSelection.KingKongPC:
					links = new AI_Links_KingKong_PC();
					break;
				case GameModeSelection.KingKongGC:
				case GameModeSelection.KingKongPS2:
				case GameModeSelection.KingKongXbox:
					links = new AI_Links_KingKong_GC();
					break;
				case GameModeSelection.KingKongXbox_20050728:
					links = new AI_Links_KingKong_Xbox_20050728();
					break;
				case GameModeSelection.KingKongPSP:
					links = new AI_Links_KingKong_PSP();
					break;
				case GameModeSelection.RaymanRavingRabbids2Wii:
				case GameModeSelection.RaymanRavingRabbids2Wii_20070901:
				case GameModeSelection.RaymanRavingRabbids2PC:
					links = new AI_Links_RRR2_Wii();
					break;
				case GameModeSelection.PrinceOfPersiaTheSandsOfTimePS2_20030819:
				case GameModeSelection.PrinceOfPersiaTheSandsOfTimePS2_20030723:
				case GameModeSelection.PrinceOfPersiaTheSandsOfTimeXbox_20030723:
				case GameModeSelection.PrinceOfPersiaTheSandsOfTimePS2:
				case GameModeSelection.PrinceOfPersiaTheSandsOfTimeGC:
				case GameModeSelection.PrinceOfPersiaTheSandsOfTimePC:
				case GameModeSelection.PrinceOfPersiaTheSandsOfTimePCLimitedDemo:
				case GameModeSelection.PrinceOfPersiaTheSandsOfTimeXbox:
				case GameModeSelection.PrinceOfPersiaTheSandsOfTimePS3:
					links = new AI_Links_PoP_SoT_PS2_Proto();
					break;

				case GameModeSelection.PrinceOfPersiaWarriorWithinPC:
				case GameModeSelection.PrinceOfPersiaWarriorWithinPCDemo:
				case GameModeSelection.PrinceOfPersiaWarriorWithinPS2:
				case GameModeSelection.PrinceOfPersiaWarriorWithinPS2_20040920:
				case GameModeSelection.PrinceOfPersiaWarriorWithinPS2_20041024:
				case GameModeSelection.PrinceOfPersiaWarriorWithinPSP:
				case GameModeSelection.PrinceOfPersiaWarriorWithinXbox:
				case GameModeSelection.PrinceOfPersiaWarriorWithinIOS:
				case GameModeSelection.PrinceOfPersiaWarriorWithinIOSDemo:
				case GameModeSelection.PrinceOfPersiaWarriorWithinIOS3GOS4Demo:
				case GameModeSelection.PrinceOfPersiaWarriorWithinGC:
				case GameModeSelection.PrinceOfPersiaWarriorWithinPS3:

				case GameModeSelection.PrinceOfPersiaTheTwoThronesPC:
				case GameModeSelection.PrinceOfPersiaTheTwoThronesGC:
				case GameModeSelection.PrinceOfPersiaTheTwoThronesWii:
				case GameModeSelection.PrinceOfPersiaTheTwoThronesPS2:
				case GameModeSelection.PrinceOfPersiaTheTwoThronesPSP:
				case GameModeSelection.PrinceOfPersiaTheTwoThronesXbox:
				case GameModeSelection.PrinceOfPersiaTheTwoThronesXbox_20051002:
				case GameModeSelection.PrinceOfPersiaTheTwoThronesPS3:

				case GameModeSelection.BeowulfPSP:

				case GameModeSelection.MyWordCoachWii:

				case GameModeSelection.TMNTPC:
				case GameModeSelection.TMNTPS2:
				case GameModeSelection.TMNTGC:
				case GameModeSelection.TMNTWii:
					links = new AI_Links_TMNT_GC();
					break;

				case GameModeSelection.AvatarWii:
				case GameModeSelection.AvatarPSP:
				case GameModeSelection.PrinceOfPersiaTheForgottenSandsWii:
				case GameModeSelection.PrinceOfPersiaTheForgottenSandsPSP:
				case GameModeSelection.RaymanRavingRabbidsTVPartyWii:
				case GameModeSelection.JustDanceWii:
				case GameModeSelection.Naruto1RiseOfANinjaXbox360:
					links = new AI_Links_PoP_TFS_Wii();
					break;

				case GameModeSelection.HorsezPS2:
					links = new AI_Links_Horsez_PS2();
					break;
				case GameModeSelection.Horsez2Wii:
				case GameModeSelection.Horsez2PS2:
				case GameModeSelection.Horsez2PSP:
				case GameModeSelection.Horsez2PSPDemo:
					links = new AI_Links_Horsez2_Wii();
					break;
				case GameModeSelection.Horsez2PC:
				case GameModeSelection.Horsez2PCHD:
					links = new AI_Links_Horsez2_PC();
					break;
				case GameModeSelection.PetzHorseClubWii:
					links = new AI_Links_PetzHorseClub_Wii();
					break;
				case GameModeSelection.PetzHorseClubPC:
				case GameModeSelection.PetzHorseClubPCHD:
					links = new AI_Links_PetzHorseClub_PC();
					break;
				case GameModeSelection.MovieGamesWii:
					links = new AI_Links_MovieGames_Wii();
					break;
			}
			if (links != null) {
				links.Init();
				links.CreateDictionaries();
			}
			return links;
		}
	}
}
