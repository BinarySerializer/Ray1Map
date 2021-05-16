using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	[Flags]
	public enum OBJ_GameObject_IdentityFlags : uint {
		None = 0,
		Bone			 = 0x00000001, // l'objet est un bone... 
		Anims			 = 0x00000002, // Objets qui ont une animation à jouer
									   // (cette liste inclut les objets de la liste LIS_AnimsEvent)
		Dyna			 = 0x00000004, // Objets en mouvement qui ne sont pas triés par priorités
									   // (à traiter par le module dynamique)
		SharedMatrix	 = 0x00000008, // Interne (matrix partagé avec autre gao) 
		Lights			 = 0x00000010, // Lumières (ou plutôt, des objets qui ont une lumière dynamique sur eux)
		AI				 = 0x00000020, // Objets qui ont une AI à exécuter
		DesignStruct	 = 0x00000040, // Structure design alloué
		Waypoints		 = 0x00000080, // Objets qui servent de repère pour les déplacements d'acteurs
		ColMap			 = 0x00000100, // Objets contre lesquels on peut collisionner physiquement
									   // (ZDR = Zone de recalage, les ZDMs collisionnent contre les ZDRs )
		ZDM				 = 0x00000200, // Objets qui collisionnent physiquement contre d'autres (ZDM=Zone de mouvements)
		ZDE				 = 0x00000400, // Objets qui ont des zones de collisions de type evenement
		DrawAtEnd		 = 0x00000800, // Objets affichés à la fin (type composants menus, HUDs...)
		BaseObject		 = 0x00001000, // Object with base data
		ExtendedObject	 = 0x00002000, // Object with extended data
		Visu			 = 0x00004000, // Object with visu data, must be used with base
		Msg				 = 0x00008000, // Object that can receive messages
		HasInitialPos	 = 0x00010000, // Object with two pos : current and init
		Generated		 = 0x00020000, // Object has been generated (must not be save)
		Links			 = 0x00040000, // Object has some links to other objects
		OBBox			 = 0x00080000, // 
		DesignHelper	 = 0x00100000, // Special designer flag
		AdditionalMatrix = 0x00200000, // L'objet a des gizmos 
		Hierarchy		 = 0x00400000, // L'objets est hierarchise (il a un pere)
		Group			 = 0x00800000, // L'objet continent un groupe moteur
		AddMatArePointer = 0x01000000, // Les gizmos sont des pointeurs sur matrices et pas des matrices
		Events			 = 0x02000000, // Structure Event alloué
		FlashMatrix		 = 0x04000000, // L'objet contient une matrice de flash
		Sound			 = 0x08000000, // Structure Sound alloué
		ODE				 = 0x10000000, // Objects "visible" for ODE solver

		// Not used in Phoenix branch (so not added at the time of RRR)
		Flag29 = 1 << 29,
		Flag30 = 1 << 30,
		Flag31 = (uint)1 << 31


		/*Flag0 = 1 << 0,
		HasActionData = 1 << 1,
		DynOn = 1 << 2,
		Flag3 = 1 << 3,
		HasLight = 1 << 4,
		HasAI = 1 << 5,
		HasDesign = 1 << 6,
		Flag7 = 1 << 7,
		HasCOL_ColMap = 1 << 8,
		Flag9 = 1 << 9,
		Flag10 = 1 << 10,
		Flag11 = 1 << 11,
		HasBase = 1 << 12,
		HasExtended = 1 << 13,
		HasGameObjectAnim = 1 << 14,
		MsgOn = 1 << 15,
		StoreInitialPosition = 1 << 16,
		Flag17 = 1 << 17,
		HasLinks = 1 << 18,
		HasOBBox = 1 << 19,
		Flag20 = 1 << 20,
		HasAddMatrix = 1 << 21,
		HasHierarchy = 1 << 22,
		Flag23 = 1 << 23,
		Flag24 = 1 << 24,
		HasEvents = 1 << 25,
		HasFlashMatrix = 1 << 26, // What is this?
		HasSound = 1 << 27,
		HasDynamics = 1 << 28,
		Flag29 = 1 << 29,
		Flag30 = 1 << 30,
		Flag31 = (uint)1 << 31*/
	}
}
