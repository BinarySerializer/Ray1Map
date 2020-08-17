using System.Collections.Generic;

namespace R1Engine.Serialize
{
    public class MemoryMap {
		public List<BinaryFile> Files { get; } = new List<BinaryFile>();

		/// <summary>
		/// Pointers that can be relocated later
		/// </summary>
		public List<Pointer> Pointers { get; } = new List<Pointer>();

		/// <summary>
		/// Add a pointer to possibly relocate later
		/// </summary>
		/// <param name="pointer">Pointer to add to list of relocated objects</param>
		public void AddPointer(Pointer pointer) {
			Pointers.Add(pointer);
		}

		public void ClearPointers() {
			Pointers.Clear();
		}
	}
}