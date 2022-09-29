using BinarySerializer;

namespace Ray1Map
{
	public class ParentContextSerializerLogger : ISerializerLogger {
		public ISerializerLogger ParentLogger { get; set; }

		public bool IsEnabled => ParentLogger.IsEnabled;

		public void Log(object obj) => ParentLogger.Log(obj);

		public void Dispose() { }

		public ParentContextSerializerLogger(ISerializerLogger parent) {
			ParentLogger = parent;
		}
	}
}