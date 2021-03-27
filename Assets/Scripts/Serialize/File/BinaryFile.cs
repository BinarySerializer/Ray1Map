using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R1Engine.Serialize
{
    public abstract class BinaryFile : IDisposable {
		public Context Context { get; }
		public long baseAddress;
		private Dictionary<uint, Pointer> predefinedPointers = new Dictionary<uint, Pointer>();
		public abstract Pointer StartPointer { get; }
		private SortedList<long, Region> regions;

		/// <summary>
		/// The file path relative to the main directory in the context
		/// </summary>
		public string filePath;

		/// <summary>
		/// Files can also be identified with an alias.
		/// </summary>
		public string alias;
		public string AbsolutePath => Context.BasePath + filePath;

		public abstract Reader CreateReader();
		public abstract Writer CreateWriter();

		public virtual Pointer GetPointer(uint serializedValue, Pointer anchor = null) {
			Pointer ptr = new Pointer(serializedValue, this, anchor: anchor);
			return ptr;
		}
		public virtual bool AllowInvalidPointer(uint serializedValue, Pointer anchor = null) {
			return false;
		}
		public virtual Pointer GetPreDefinedPointer(uint offset) {
			if (predefinedPointers.ContainsKey(offset)) {
				return predefinedPointers[offset];
			}
			return null;
		}

		public virtual void EndRead(Reader reader) {
			((IDisposable)reader).Dispose();
		}
		public virtual void EndWrite(Writer writer) {
			writer?.Flush();
			((IDisposable)writer).Dispose();
		}

		public BinaryFile(Context context) {
			this.Context = context;
		}

		protected void CreateBackupFile() {
			if (Settings.BackupFiles && !FileSystem.FileExists(AbsolutePath + ".BAK") && FileSystem.FileExists(AbsolutePath)) {
                using (Stream s = FileSystem.GetFileReadStream(AbsolutePath)) {
                    using (Stream sb = FileSystem.GetFileWriteStream(AbsolutePath + ".BAK")) {
                        s.CopyTo(sb);
                    }
                }
            }
		}

		public virtual void Dispose() { }

		public Endian Endianness { get; set; } = Endian.Little;
		public enum Endian {
			Little,
			Big
		}

		/// <summary>
		/// Indicates if the file should be recreated when writing to it
		/// </summary>
		public bool RecreateOnWrite { get; set; } = true;

		public virtual bool[] FileReadMap { get; protected set; }

		protected bool ShouldInitFileReadMap { get; set; }
		public void InitFileReadMap() => ShouldInitFileReadMap = true;
		public void InitFileReadMap(long length, bool forceInit = false)
        {
			if (forceInit || ShouldInitFileReadMap)
            {
				ShouldInitFileReadMap = false;
                FileReadMap = new bool[length];
			}
        }

        public void UpdateReadMap(long offset, long length)
        {
            if (FileReadMap == null) 
                return;

            for (int i = 0; i < length; i++)
                FileReadMap[offset + i] = true;
        }
		public void ExportFileReadMap(string outputFilePath)
        {
			File.WriteAllBytes(outputFilePath, FileReadMap.Select(x => (byte)(x ? 0xFF : 0x00)).ToArray());
        }

		public void AddRegion(long offset, long length, string name) {
			var newRegion = new Region(offset, length, name);
			if (regions == null) {
				regions = new SortedList<long, Region>();
			}
			regions.Add(offset, new Region(offset, length, name));
		}

		public Region GetRegion(long offset) {
			if(regions == null) return null;

			// Binary search
			int lower = 0;
			int upper = regions.Count - 1;
			var keys = regions.Keys;

			while (lower <= upper) {
				int middle = lower + (upper - lower) / 2;
				var val = regions[keys[middle]];
				if (offset < val.Offset) {
					upper = middle - 1;
				} else if (offset >= val.Offset && offset < val.Offset + val.Length) {
					return val;
				} else {
					lower = middle + 1;
				}
			}
			return null;
		}

		public class Region {
			public long Offset { get; private set; }
			public long Length { get; private set; }
			public string Name { get; private set; }

			public Region(long offset, long length, string name) {
				Offset = offset;
				Length = length;
				Name = name;
			}
		}
		private static RegionComparer regionComparer = new RegionComparer();
		public class RegionComparer : IComparer<Region> {
			public int Compare(Region x, Region y) {
				return x.Offset.CompareTo(y.Offset);
			}
		}
	}
}
