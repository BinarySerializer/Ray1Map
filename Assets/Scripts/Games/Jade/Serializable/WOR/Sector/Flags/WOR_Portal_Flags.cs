using System;

namespace Ray1Map.Jade {
	[Flags]
	public enum WOR_Portal_Flags : ushort {
		None          = 0,
		Valid         = 1 << 0,
		Share         = 1 << 1,
		Flag2         = 1 << 2,
		Flag3         = 1 << 3,
		Flag4         = 1 << 4,
		Flag5         = 1 << 5,
		Flag6         = 1 << 6,
		Flag7         = 1 << 7,
		Render        = 1 << 8,
		Pickable      = 1 << 9,
		Picked        = 1 << 10,
		Flag11        = 1 << 11,
		Flag12        = 1 << 12,
		Flag13        = 1 << 13,
		Flag14        = 1 << 14,
		Flag15        = 1 << 15,
	}
}
