using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	[Flags]
	public enum PAG_ParticleGeneratorFlags : int {
		None = 0,
		Transparant				= 0b0000000000000001,
		RGBEquAlpha				= 0b0000000000000010,
		SquareParticules		= 0b0000000000000100,
		DeathDecreaseAlpha		= 0b0000000000001000,
		DeathDecreaseSize		= 0b0000000000010000,
		DeathIncreaseSize		= 0b0000000000100000,
		Active					= 0b0000000001000000,
		Freeze					= 0b0000000010000000,
		BirthIncreaseAlpha		= 0b0000000100000000,
		BirthIncreaseSize		= 0b0000001000000000,
		UseZMin					= 0b0000010000000000,
		UseZMax					= 0b0000100000000000,
		UseRotation				= 0b0001000000000000,
		UseFriction				= 0b0010000000000000,
	}
}
