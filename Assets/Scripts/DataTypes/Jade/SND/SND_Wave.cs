using BinarySerializer;

namespace R1Engine.Jade
{
    public class SND_Wave : Jade_File 
    {
		public byte[] WaveData { get; set; }

		public override void SerializeImpl(SerializerObject s) 
        {
			WaveData = s.SerializeArray<byte>(WaveData, FileSize, name: nameof(WaveData));
		}
	}
}