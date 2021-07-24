using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GRID_WorldGrid : Jade_File {
		public override bool HasHeaderBFFile => true;

		public uint PointerGroups { get; set; }
		public uint PointerRealGroups { get; set; }
		public Jade_Reference<GRID_CompressedGrid> CompressedGrid { get; set; }
		public uint PointerRealArray { get; set; }
		public uint PointerEvalArray { get; set; }
		public float MinZTotal { get; set; }
		public float MinXTotal { get; set; }
		public float MinYTotal { get; set; }
		public float MinXReal { get; set; }
		public float MinYReal { get; set; }
		public ushort GroupsCountX { get; set; }
		public ushort GroupsCountY { get; set; }
		public ushort RealGroupsCountX { get; set; }
		public ushort RealGroupsCountY { get; set; }
		public ushort XRealGroup { get; set; }
		public ushort YRealGroup { get; set; }
		public byte SizeGroup { get; set; }
		public uint UInt_Editor_19 { get; set; }
		public byte Byte_Editor_1D { get; set; }
		public byte Byte_Editor_1E { get; set; }
		public byte Byte_Editor_1F { get; set; }
		public int[] Grid { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (!Loader.IsBinaryData) {
				PointerGroups = s.Serialize<uint>(PointerGroups, name: nameof(PointerGroups));
				PointerRealGroups = s.Serialize<uint>(PointerRealGroups, name: nameof(PointerRealGroups));
			}
			CompressedGrid = s.SerializeObject<Jade_Reference<GRID_CompressedGrid>>(CompressedGrid, name: nameof(CompressedGrid))?.Resolve();
			if (!Loader.IsBinaryData) {
				PointerRealArray = s.Serialize<uint>(PointerRealArray, name: nameof(PointerRealArray));
				PointerEvalArray = s.Serialize<uint>(PointerEvalArray, name: nameof(PointerEvalArray));
			}
			if (!Loader.IsBinaryData || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				MinZTotal = s.Serialize<float>(MinZTotal, name: nameof(MinZTotal));
			}
			MinXTotal = s.Serialize<float>(MinXTotal, name: nameof(MinXTotal));
			MinYTotal = s.Serialize<float>(MinYTotal, name: nameof(MinYTotal));
			MinXReal = s.Serialize<float>(MinXReal, name: nameof(MinXReal));
			MinYReal = s.Serialize<float>(MinYReal, name: nameof(MinYReal));
			GroupsCountX = s.Serialize<ushort>(GroupsCountX, name: nameof(GroupsCountX));
			GroupsCountY = s.Serialize<ushort>(GroupsCountY, name: nameof(GroupsCountY));
			if (!Loader.IsBinaryData || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				RealGroupsCountX = s.Serialize<ushort>(RealGroupsCountX, name: nameof(RealGroupsCountX));
				RealGroupsCountY = s.Serialize<ushort>(RealGroupsCountY, name: nameof(RealGroupsCountY));
			}
			XRealGroup = s.Serialize<ushort>(XRealGroup, name: nameof(XRealGroup));
			YRealGroup = s.Serialize<ushort>(YRealGroup, name: nameof(YRealGroup));
			SizeGroup = s.Serialize<byte>(SizeGroup, name: nameof(SizeGroup));
			if (!Loader.IsBinaryData) {
				UInt_Editor_19 = s.Serialize<uint>(UInt_Editor_19, name: nameof(UInt_Editor_19));
				Byte_Editor_1D = s.Serialize<byte>(Byte_Editor_1D, name: nameof(Byte_Editor_1D));
				Byte_Editor_1E = s.Serialize<byte>(Byte_Editor_1E, name: nameof(Byte_Editor_1E));
				Byte_Editor_1F = s.Serialize<byte>(Byte_Editor_1F, name: nameof(Byte_Editor_1F));
			}
			Grid = s.SerializeArray<int>(Grid, GroupsCountX * GroupsCountY, name: nameof(Grid));
		}

		public void Unload() {
			if (CompressedGrid?.Value != null) Loader.RemoveCacheReference(CompressedGrid.Key, all: true);
			Loader.RemoveCacheReference(Key, all: true);
		}
	}
}
