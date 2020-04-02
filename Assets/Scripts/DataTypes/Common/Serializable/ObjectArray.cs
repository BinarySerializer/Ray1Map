using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine {
	/// <summary>
	/// Array of serializable objects. Mainly for files that are just simple object arrays.
	/// Use s.SerializeObjectArray when possible
	/// </summary>
	/// <typeparam name="T">Generic parameter, should be a R1Serializable</typeparam>
	public class ObjectArray<T> : R1Serializable where T : R1Serializable, new() {
		public uint Length = 0;
		public T[] Value;
		public override void SerializeImpl(SerializerObject s) {
			Value = s.SerializeObjectArray<T>(Value, Length, name: "Value");
		}
	}
}
