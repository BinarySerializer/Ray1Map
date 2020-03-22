using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine {
	/// <summary>
	/// Base type for structs in R1
	/// </summary>
	public abstract class R1Serializable {
		protected bool isFirstLoad = true;
		public Context Context { get; protected set; }
		public Pointer Offset { get; protected set; }

		public void Init(Pointer offset) {
			this.Offset = offset;
			this.Context = offset.Context;
		}

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public abstract void SerializeImpl(SerializerObject s);

		public void Serialize(SerializerObject s) {
			OnPreSerialize(s);
			SerializeImpl(s);
			Size = s.CurrentPointer.AbsoluteOffset - Offset.AbsoluteOffset;
			OnPostSerialize(s);
			isFirstLoad = false;
		}
		protected virtual void OnPreSerialize(SerializerObject s) { }
		protected virtual void OnPostSerialize(SerializerObject s) { }

		public virtual uint Size { get; protected set; }

		/// <summary>
		/// Reimplement for objects with varying sizes
		/// </summary>
		public virtual void RecalculateSize() { }
	}
}
