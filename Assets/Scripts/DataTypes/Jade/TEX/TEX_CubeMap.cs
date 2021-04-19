using BinarySerializer;

namespace R1Engine.Jade
{
    public class TEX_CubeMap : Jade_File 
    {
        public DDS DDSData { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
            DDSData = s.SerializeObject<DDS>(DDSData, name: nameof(DDSData));
        }
    }
}