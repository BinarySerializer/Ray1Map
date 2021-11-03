using System.Linq;
using System.Text;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GEN {
	public class GEN_RLXData : BinarySerializable {
		public byte RLXType { get; set; }
		public ushort Width { get; set; }
		public ushort Height { get; set; }
		public ushort X { get; set; }
		public ushort Y { get; set; }
		public uint? FileSize { get; set; }
		public byte LookupTableCount { get; set; }
		public byte[] LookupTable { get; set; }

		public uint DataLength { get; set; }
		public byte[] Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			RLXType = s.Serialize<byte>(RLXType, name: nameof(RLXType));
			Width = s.Serialize<ushort>(Width, name: nameof(Width));
			Height = s.Serialize<ushort>(Height, name: nameof(Height));
			X = s.Serialize<ushort>(X, name: nameof(X));
			Y = s.Serialize<ushort>(Y, name: nameof(Y));
			if (RLXType < 7) {
				if (RLXType > 1) {
					LookupTableCount = s.Serialize<byte>(LookupTableCount, name: nameof(LookupTableCount));
					LookupTable = s.SerializeArray<byte>(LookupTable, LookupTableCount, name: nameof(LookupTable));
				}
				DataLength = FileSize.Value - (uint)(s.CurrentPointer - Offset);
				Data = s.DoEncoded(new GEN_RLXEncoder(this), () => s.SerializeArray<byte>(Data, s.CurrentLength, name: nameof(Data)));
			} else {
				DataLength = FileSize.Value - (uint)(s.CurrentPointer - Offset);
				Data = s.SerializeArray<byte>(Data, DataLength, name: nameof(Data));
			}
		}

		public Texture2D ToTexture2D(Color[] palette, Texture2D texture = null, bool raw = false) {
			var tex = texture ?? TextureHelpers.CreateTexture2D(X + Width, Y + Height, clear: true);
			if (RLXType < 7) {
				if (raw) {
					tex.FillRegion(Data, 0, palette, Util.TileEncoding.Linear_8bpp,
						X, Y, Width, Height,
						flipTextureY: true);
				} else {
					tex.FillRegion(Data, 0, null, Util.TileEncoding.Linear_8bpp,
						X, Y, Width, Height,
						flipTextureY: true,
						paletteFunction: (palIndex, x, y) => {
							if (palIndex == 0) { // Transparent color
							return Color.clear;
							} else if (palIndex == 1) { // Red: Clear previous frame
							return Color.clear;
							} else if (palIndex == 2) { // Green: Unchanged
							return null;
							} else return palette[palIndex];
						});
				}
			}
			tex.Apply();
			return tex;
		}
	}
}