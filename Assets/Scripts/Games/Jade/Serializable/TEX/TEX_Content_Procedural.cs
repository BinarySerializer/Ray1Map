using BinarySerializer;

namespace Ray1Map.Jade
{
    public class TEX_Content_Procedural : BinarySerializable {
        public uint FileSize { get; set; } // Set in onPreSerialize

        public uint DataSize { get; set; }
        public ushort Flags { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public TEXPRO_Type Type { get; set; }

        public TEXPRO_Photo Photo { get; set; }
        public TEXPRO_Mpeg Mpeg { get; set; }

        // Custom
        public int EditorSizeDifference => Photo != null && FileSize > 12 ? 4 : 0;

        public override void SerializeImpl(SerializerObject s) {
            DataSize = s.Serialize<uint>(DataSize, name: nameof(DataSize));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Type = s.Serialize<TEXPRO_Type>(Type, name: nameof(Type));

            uint contentSize = FileSize - 12;
            switch (Type) {
                case TEXPRO_Type.Photo:
                    Photo = s.SerializeObject<TEXPRO_Photo>(Photo, onPreSerialize: p => p.FileSize = contentSize, name: nameof(Photo));
                    break;
                case TEXPRO_Type.Mpeg:
                    Mpeg = s.SerializeObject<TEXPRO_Mpeg>(Mpeg, onPreSerialize: m => m.FileSize = contentSize, name: nameof(Mpeg));
                    break;
            }
        }

        public enum TEXPRO_Type : ushort {
            Unknown = 0,
            Water = 1,
            Fire = 2,
            Mpeg = 3,
            Photo = 4,
            Plasma = 5
        }
	}
}