using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray1Map.Jade {
	[Flags]
	public enum OBJ_GameObject_TypeFlags : byte {
		None = 0,
		Friend		= 0x01,		// Friend of the main character
		Enemy		= 0x02,		// Enemy of the main character
		Projectile	= 0x04,		// Projectile
		Custom1		= 0x08,		// "Camera" in Spelling Spree
		Custom2		= 0x10,		//
		DodgeCamera	= 0x20,		// L'objet gêne la camera
		Space		= 0x40,		// Objet espace
		Pushable	= 0x80,		// Pushable
	}
}
