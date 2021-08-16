using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEXPRO_Mpeg_Content : Jade_File {
        public uint UInt_00 { get; set; }

		// Irregular Jade_File format. Fixed filesize, only 4 bytes
		protected override void OnPreSerialize(SerializerObject s) {
			base.OnPreSerialize(s);
			FileSize = 4;
			Offset.File.AddRegion(Offset.FileOffset, 4, $"{GetType().Name}_{Key:X8}");
		}

		protected override void SerializeFile(SerializerObject s) {
            UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
        }
	}
}