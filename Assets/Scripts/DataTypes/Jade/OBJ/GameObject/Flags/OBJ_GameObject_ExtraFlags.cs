using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	[Flags]
	public enum OBJ_GameObject_ExtraFlags : ushort {
		None = 0,
		FreezePlayer				= 0x01,                 // Freeze le player meme la BMagique
		Montpellier_NoNBlendBone	= 0x1000,				// No Blend On Bone
		Montpellier_AlwaysPlay		= 0x2000,				// Always play animation, even if culled
		RecordHieHF					= 0x0001,
		RecordAnimask				= 0x0002,
		RecordRope					= 0x0020,
		RecordBridge    			= 0x0040,
		RecordAnimix	   			= 0x0080,
		RecordTranslation			= 0x0100,
		RecordRotation				= 0x0200,
		RecordAnimation				= 0x0400,
		PlayAIDuringRewind			= 0x0800,
		RecordAI					= 0x1000,
		RecordDyna					= 0x2000,
		RecordHie					= 0x4000,
		RecordEVE					= 0x8000,
	}
}
