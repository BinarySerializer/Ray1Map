using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class Ambiance_Modifier : MDF_Modifier {
		public uint Version { get; set; }
		public Ambiance_Filter.Flags Filters { get; set; }
		public uint UInt_00 { get; set; }
		public Ambiance_Filter.ColorBurn ColorBurn { get; set; }
		public Ambiance_Filter.Highlight Highlight { get; set; }
		public Ambiance_Filter.BlackWhite BlackWhite { get; set; }
		public Ambiance_Filter.Glow Glow { get; set; }
		public Ambiance_Filter.Monochromatic Monochromatic { get; set; }
		public Ambiance_Filter.Colorize Colorize { get; set; }
		public Ambiance_Filter.Overlay Overlay { get; set; }
		public Ambiance_Filter.Contrast Contrast { get; set; }
		public Ambiance_Filter.CBalance CBalance { get; set; }
		public Ambiance_Filter.Brightness Brightness { get; set; }
		public Ambiance_Filter.SpiralBlur SpiralBlur { get; set; }
		public Ambiance_Filter.Shadow Shadow { get; set; }
		public uint Ambient { get; set; }
		public uint LinkersCount { get; set; }
		public Jade_Reference<OBJ_GameObject>[] Linkers { get; set; }
		public Ambiance_Filter.Glow2 Glow2 { get; set; }
		public Ambiance_Filter.DOF DOF { get; set; }
		public Ambiance_Filter.Distortion Distortion { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version == 0) {
				Filters = (Ambiance_Filter.Flags)s.Serialize<byte>((byte)Filters, name: nameof(Filters));
			} else {
				Filters = s.Serialize<Ambiance_Filter.Flags>(Filters, name: nameof(Filters));
			}
			if (Filters.HasFlag(Ambiance_Filter.Flags.ColorBurn)) ColorBurn = s.SerializeObject<Ambiance_Filter.ColorBurn>(ColorBurn, name: nameof(ColorBurn));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Highlight)) Highlight = s.SerializeObject<Ambiance_Filter.Highlight>(Highlight, name: nameof(Highlight));
			if (Filters.HasFlag(Ambiance_Filter.Flags.BlackWhite)) BlackWhite = s.SerializeObject<Ambiance_Filter.BlackWhite>(BlackWhite, name: nameof(BlackWhite));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Glow)) Glow = s.SerializeObject<Ambiance_Filter.Glow>(Glow, name: nameof(Glow));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Monochromatic)) Monochromatic = s.SerializeObject<Ambiance_Filter.Monochromatic>(Monochromatic, name: nameof(Monochromatic));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Colorize)) Colorize = s.SerializeObject<Ambiance_Filter.Colorize>(Colorize, name: nameof(Colorize));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Overlay)) Overlay = s.SerializeObject<Ambiance_Filter.Overlay>(Overlay, onPreSerialize: f => f.Pre_HasColor2 = Version >= 5, name: nameof(Overlay));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Contrast)) Contrast = s.SerializeObject<Ambiance_Filter.Contrast>(Contrast, name: nameof(Contrast));
			if (Version >= 4 && Filters.HasFlag(Ambiance_Filter.Flags.CBalance)) CBalance = s.SerializeObject<Ambiance_Filter.CBalance>(CBalance, name: nameof(CBalance));
			if (Filters.HasFlag(Ambiance_Filter.Flags.Brightness)) Brightness = s.SerializeObject<Ambiance_Filter.Brightness>(Brightness, name: nameof(Brightness));
			if (Version >= 3 && Filters.HasFlag(Ambiance_Filter.Flags.SpiralBlur)) SpiralBlur = s.SerializeObject<Ambiance_Filter.SpiralBlur>(SpiralBlur, name: nameof(SpiralBlur));
			if (Version >= 6 && Filters.HasFlag(Ambiance_Filter.Flags.Shadow)) Shadow = s.SerializeObject<Ambiance_Filter.Shadow>(Shadow, name: nameof(Shadow));
			if (Version < 2) Ambient = s.Serialize<uint>(Ambient, name: nameof(Ambient));
			LinkersCount = s.Serialize<uint>(LinkersCount, name: nameof(LinkersCount));
			Linkers = s.SerializeObjectArray<Jade_Reference<OBJ_GameObject>>(Linkers, LinkersCount, name: nameof(Linkers))?.Resolve();
			if (Version >= 7 && Filters.HasFlag(Ambiance_Filter.Flags.Glow2)) Glow2 = s.SerializeObject<Ambiance_Filter.Glow2>(Glow2, name: nameof(Glow2));
			if (Version >= 8 && Filters.HasFlag(Ambiance_Filter.Flags.DOF)) DOF = s.SerializeObject<Ambiance_Filter.DOF>(DOF, name: nameof(DOF));
			if (Version >= 9 && Filters.HasFlag(Ambiance_Filter.Flags.Distortion)) Distortion = s.SerializeObject<Ambiance_Filter.Distortion>(Distortion, name: nameof(Distortion));
		}
	}
}
