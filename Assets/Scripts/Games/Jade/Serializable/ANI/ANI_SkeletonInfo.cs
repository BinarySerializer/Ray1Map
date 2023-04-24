using BinarySerializer;
using System;

namespace Ray1Map.Jade
{
    public class ANI_SkeletonInfo : Jade_File {
		public override string Export_Extension => "shp";
		public override bool HasHeaderBFFile => true;

        public ushort Version { get; set; }
        public byte Count { get; set; } // Max 64
        public byte[] Bytes0 { get; set; }
        public byte[] Bytes1 { get; set; }

        protected override void SerializeFile(SerializerObject s)
        {
            Version = s.Serialize<ushort>(Version, name: nameof(Version));
            Count = s.Serialize<byte>(Count, name: nameof(Count));
            if(Count > 64) Count = 64;
            Bytes0 = s.SerializeArray<byte>(Bytes0, Count, name: nameof(Bytes0));
            if (Version >= 2) Bytes1 = s.SerializeArray<byte>(Bytes1, Count, name: nameof(Bytes1));
        }
    }
}