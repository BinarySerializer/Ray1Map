using BinarySerializer;

namespace Ray1Map
{
	public class ParentContextSerializerLog : ISerializerLog {
		public ISerializerLog ParentLog { get; set; }

		public bool IsEnabled => ParentLog.IsEnabled;

		public void Log(object obj) => ParentLog.Log(obj);

		public void Dispose() { }

		public ParentContextSerializerLog(ISerializerLog parent) {
			ParentLog = parent;
		}
	}
}