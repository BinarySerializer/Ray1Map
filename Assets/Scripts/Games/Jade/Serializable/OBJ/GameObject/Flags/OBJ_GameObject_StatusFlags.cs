using System;

namespace Ray1Map.Jade {
	[Flags]
	public enum OBJ_GameObject_StatusFlags : byte {
		None = 0,
		Active		= 1 << 0,		// Flag that indicates if the object is active or not
		Visible		= 1 << 1,		// Indicates if object is visible by one or more world views
		Culled		= 1 << 2,		// Indicates if object has a father or not
		//HasFather	= 1 << 2,		// Indicates if object has a father or not <-- was commented out in Horsez, also in Spelling Spree
		RTL	        = 1 << 3,		// Object is real time lighted
		SectoReinit	= 1 << 4,		// Object is reinitialized when activated by secto
		Detection	= 1 << 5,		// Object has SnP detection list
		HasChild	= 1 << 6,		// Has one or more children
		EnableSnP	= 1 << 7,		// Object must be inserted in the SnP
	}
}
