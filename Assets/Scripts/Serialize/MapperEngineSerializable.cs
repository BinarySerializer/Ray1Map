using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine {
	// TODO: Improve this system

	/// <summary>
	/// Base type for structs in R1
	/// </summary>
	public abstract class MapperEngineSerializable {
		protected bool isFirstLoad = true;
		public Context Context { get; protected set; }

		public abstract void Read(MapperEngineCommandParser parser);
		public void Serialize(string path, Context context, SerializerMode mode) {
			Context = context;
			if (mode == SerializerMode.Read) {
				Stream s = context.GetFileStream(path);
				if (s != null && s.Length > 0) {
					OnPreSerialize(path);
					using (MapperEngineCommandParser parser = new MapperEngineCommandParser(s)) {
						Read(parser);
					}
					OnPostSerialize(path);
					isFirstLoad = false;
				}
			}
		}
		protected virtual void OnPreSerialize(string path) { }
		protected virtual void OnPostSerialize(string path) { }
	}
}
