using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	[Flags]
	public enum OBJ_GameObject_IdentityFlags : uint {
		None = 0,
		Flag0 = 1 << 0,
		HasActionData = 1 << 1,
		DynOn = 1 << 2,
		Flag3 = 1 << 3,
		HasGEO_Object = 1 << 4,
		HasAI = 1 << 5,
		Flag6 = 1 << 6,
		Flag7 = 1 << 7,
		HasCOL_ColMap = 1 << 8,
		Flag9 = 1 << 9,
		Flag10 = 1 << 10,
		Flag11 = 1 << 11,
		HasVisual = 1 << 12,
		HasExtended = 1 << 13,
		HasGeometricData = 1 << 14,
		MsgOn = 1 << 15,
		StoreInitialPosition = 1 << 16,
		Flag17 = 1 << 17,
		HasWAY_AllLinkLists = 1 << 18,
		HasOBBox = 1 << 19,
		Flag20 = 1 << 20,
		Flag21 = 1 << 21,
		HasHierarchyData = 1 << 22,
		Flag23 = 1 << 23,
		Flag24 = 1 << 24,
		HasEVE_ListTracks = 1 << 25,
		HasFlashMatrix = 1 << 26, // What is this?
		HasSND_UnknownBank = 1 << 27,
		Flag28 = 1 << 28,
		Flag29 = 1 << 29,
		Flag30 = 1 << 30,
		Flag31 = (uint)1 << 31
	}
}
