using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;

namespace R1Engine {
	/// <summary>
	/// Array of simple non-R1Serializable types. Mainly for files that are just simple arrays.
	/// Use s.SerializeArray where possible
	/// </summary>
	/// <typeparam name="T">Generic parameter, should not be R1Serializable</typeparam>
	public class Array<T> : R1Serializable {
		public uint Length = 0;
		public T[] Value;
		public override void SerializeImpl(SerializerObject s) {
			Value = s.SerializeArray<T>(Value, Length, name: "Value");
		}
	}
}
