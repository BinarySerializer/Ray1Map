using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GRID_WorldGrid : Jade_File {
		public uint UInt_Editor_00 { get; set; }
		public uint UInt_Editor_04 { get; set; }
		public Jade_Reference<GRID_CompressedGrid> CompressedGrid { get; set; }
		public uint UInt_Editor_08 { get; set; }
		public uint UInt_Editor_0C { get; set; }
		public uint UInt_Editor_10 { get; set; }
		public float Float_04 { get; set; }
		public float Float_08 { get; set; }
		public float Float_0C { get; set; }
		public float Float_10 { get; set; }
		public ushort Width { get; set; }
		public ushort Height { get; set; }
		public ushort UShort_Editor_14 { get; set; }
		public ushort UShort_Editor_16 { get; set; }
		public ushort UShort_14 { get; set; }
		public ushort UShort_16 { get; set; }
		public byte Byte_18 { get; set; }
		public uint UInt_Editor_19 { get; set; }
		public byte Byte_Editor_1D { get; set; }
		public byte Byte_Editor_1E { get; set; }
		public byte Byte_Editor_1F { get; set; }
		public int[] Grid { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (!Loader.IsBinaryData) {
				UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
				UInt_Editor_04 = s.Serialize<uint>(UInt_Editor_04, name: nameof(UInt_Editor_04));
			}
			CompressedGrid = s.SerializeObject<Jade_Reference<GRID_CompressedGrid>>(CompressedGrid, name: nameof(CompressedGrid))?.Resolve();
			if (!Loader.IsBinaryData) {
				UInt_Editor_08 = s.Serialize<uint>(UInt_Editor_08, name: nameof(UInt_Editor_08));
				UInt_Editor_0C = s.Serialize<uint>(UInt_Editor_0C, name: nameof(UInt_Editor_0C));
				UInt_Editor_10 = s.Serialize<uint>(UInt_Editor_10, name: nameof(UInt_Editor_10));
			}
			Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
			Width = s.Serialize<ushort>(Width, name: nameof(Width));
			Height = s.Serialize<ushort>(Height, name: nameof(Height));
			if (!Loader.IsBinaryData) {
				UShort_Editor_14 = s.Serialize<ushort>(UShort_Editor_14, name: nameof(UShort_Editor_14));
				UShort_Editor_16 = s.Serialize<ushort>(UShort_Editor_16, name: nameof(UShort_Editor_16));
			}
			UShort_14 = s.Serialize<ushort>(UShort_14, name: nameof(UShort_14));
			UShort_16 = s.Serialize<ushort>(UShort_16, name: nameof(UShort_16));
			Byte_18 = s.Serialize<byte>(Byte_18, name: nameof(Byte_18));
			if (!Loader.IsBinaryData) {
				UInt_Editor_19 = s.Serialize<uint>(UInt_Editor_19, name: nameof(UInt_Editor_19));
				Byte_Editor_1D = s.Serialize<byte>(Byte_Editor_1D, name: nameof(Byte_Editor_1D));
				Byte_Editor_1E = s.Serialize<byte>(Byte_Editor_1E, name: nameof(Byte_Editor_1E));
				Byte_Editor_1F = s.Serialize<byte>(Byte_Editor_1F, name: nameof(Byte_Editor_1F));
			}
			Grid = s.SerializeArray<int>(Grid, Width * Height, name: nameof(Grid));
		}

		public void Unload() {
			if (CompressedGrid?.Value != null) Loader.RemoveCacheReference(CompressedGrid.Key, all: true);
			Loader.RemoveCacheReference(Key, all: true);
		}
	}
}
