using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine {
	public class Array<T> : R1Serializable {
		public uint Length = 0;
		public byte[] Value;
		public override void SerializeImpl(SerializerObject s) {
			Value = s.SerializeArray<byte>(Value, Length, name: "Value");
		}
	}
}
