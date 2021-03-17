using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class BIG_File : R1Serializable {
		public uint FileSize { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
		}
	}
}
