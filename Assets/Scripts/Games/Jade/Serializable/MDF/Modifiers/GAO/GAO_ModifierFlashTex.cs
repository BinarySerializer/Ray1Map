using BinarySerializer;

namespace Ray1Map.Jade
{
    public class GAO_ModifierFlashTex : MDF_Modifier {
        public uint Version { get; set; }
        public uint UInt_04 { get; set; }
        public uint V1_UInt_00 { get; set; }
		public uint V1_UInt_04 { get; set; }
		public uint V1_UInt_08 { get; set; }
        public Jade_Vector Vector { get; set; }
        public uint V2_UInt { get; set; }

        public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
            Version = s.Serialize<uint>(Version, name: nameof(Version));
            UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
            if (Version == 1) {
                V1_UInt_00 = s.Serialize<uint>(V1_UInt_00, name: nameof(V1_UInt_00));
                V1_UInt_04 = s.Serialize<uint>(V1_UInt_04, name: nameof(V1_UInt_04));
                V1_UInt_08 = s.Serialize<uint>(V1_UInt_08, name: nameof(V1_UInt_08));
            }
            Vector = s.SerializeObject<Jade_Vector>(Vector, name: nameof(Vector));
            if (Version >= 2) V2_UInt = s.Serialize<uint>(V2_UInt, name: nameof(V2_UInt));
        }
    }
}
