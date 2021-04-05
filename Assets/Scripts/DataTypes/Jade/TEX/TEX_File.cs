using System;
using BinarySerializer;

namespace R1Engine.Jade {
	// See: GDI_l_AttachWorld & TEX_l_File_Read
	// TEX_l_File_Read reads the texture header, after that GDI_l_AttachWorld resolves some references (for animated textures) and adds palette references
	public class TEX_File : Jade_File {
		public override void SerializeImpl(SerializerObject s) {
			throw new NotImplementedException($"TODO: Implement {GetType()}");
		}
	}
}
