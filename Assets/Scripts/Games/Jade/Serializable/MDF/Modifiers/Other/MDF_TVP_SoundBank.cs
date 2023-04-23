using BinarySerializer;

namespace Ray1Map.Jade
{
    public class MDF_TVP_SoundBank : MDF_Modifier {
        public uint Version { get; set; }
        public Jade_Reference<SND_TVP_Bank> Bank { get; set; }

        public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
            Version = s.Serialize<uint>(Version, name: nameof(Version));
            if (Version != 0) {
                Bank = s.SerializeObject<Jade_Reference<SND_TVP_Bank>>(Bank, name: nameof(Bank))?.Resolve();
            }
        }
    }
}
