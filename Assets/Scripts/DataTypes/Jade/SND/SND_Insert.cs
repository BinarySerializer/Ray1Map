using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class SND_Insert : Jade_File {
		public override string Export_Extension => "ins";
		public byte[] WaveData { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			// TODO: Maybe properly parse this later on
			WaveData = s.SerializeArray<byte>(WaveData, FileSize, name: nameof(WaveData));
		}
	}
}
