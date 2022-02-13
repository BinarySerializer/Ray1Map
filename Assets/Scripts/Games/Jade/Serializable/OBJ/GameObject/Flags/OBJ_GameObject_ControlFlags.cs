using System;

namespace Ray1Map.Jade {
	[Flags]
	public enum OBJ_GameObject_ControlFlags : ushort {
		None = 0,
		AutoActivableObject			 = 0x0001, // Indicates if the object shopuld be used by the auto activation system
		SecInvisible				 = 0x0002, // 
		ForceDetectionList			 = 0x0010, // Forces the object to have a detecion list 
		ForceInvisible				 = 0x0020, // Set to true to force an object to be invisible
		ForceInactive				 = 0x0040, // Set to true to force an object to be inactive
		LookAt						 = 0x0080, // Set to true to force object to look at camera
		RayInsensitive				 = 0x0100, // Set to true to force object to look at camera
		EditableBV					 = 0x0200, // Indicates if the BV of the object can be edit
		InPause						 = 0x0400, // Actor is in pause
		AlwaysActive				 = 0x0800, // Actor always active
		AlwaysVisible				 = 0x1000, // Actor always visible
		DesableAnimPlayerOnMe		 = 0x8000, // Anim

		//SectoInvisible				 = 0x0001,
		//SectoInactive				 = 0x0002,

	}
}
