using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine {
	public class Array<T> : R1Serializable {
		public uint Length = 0;
		public T[] Value;
		public override void SerializeImpl(SerializerObject s) {
			Value = s.SerializeArray<T>(Value, Length, name: "Value");
		}
	}
}
