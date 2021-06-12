using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	[Flags]
	public enum OBJ_GameObject_StatusFlags : ushort {
		None = 0,
		Active		= 1 << 0,		// Flag that indicates if the object is active or not
		Visible		= 1 << 1,		// Indicates if object is visible by one or more world views
		Culled		= 1 << 2,		// Indicates if object has a father or not
		//HasFather	= 1 << 2,		// Indicates if object has a father or not <-- was commented out in Horsez
		Unknown3	= 1 << 3,
		Unknown4	= 1 << 4,
		Unknown5	= 1 << 5,
		Unknown6	= 1 << 6,
		Unknown7	= 1 << 7,
		Unknown8	= 1 << 8,
		Unknown9	= 1 << 9,
		Unknown10	= 1 << 10,
		Unknown11	= 1 << 11,
		Unknown12	= 1 << 12,
		Unknown13	= 1 << 13,
		Unknown14	= 1 << 14,
		Unknown15	= 1 << 15,
	}
}
