using BinarySerializer;

namespace R1Engine.Jade
{
    public class TEX_CubeMap : Jade_File 
    {
        public DDS DDSData { get; set; }
        public TEX_File Header { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            DDSData = s.SerializeObject<DDS>(DDSData, name: nameof(DDSData));
            if (s.GetR1Settings().EngineVersion != EngineVersion.Jade_KingKong_PCGamersEdition) {
                Header = s.SerializeObject<TEX_File>(Header, onPreSerialize: h => {
                    h.FileSize = 0x20;
                    h.Loader = Loader;
                    h.Key = Key;
                }, name: nameof(Header));
            }
        }
    }
}