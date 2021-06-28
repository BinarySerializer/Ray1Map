using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public abstract class GRO_GraphicRenderObject : BinarySerializable {
		public GRO_Struct GRO { get; set; }
		public uint ObjectVersion => GRO.ObjectVersion;
	}
}
