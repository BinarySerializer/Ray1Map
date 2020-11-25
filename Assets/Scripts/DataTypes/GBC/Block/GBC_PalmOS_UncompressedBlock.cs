using System.Linq;
using System.Text;

namespace R1Engine
{
    public class GBC_PalmOS_UncompressedBlock<T> : GBC_BaseBlock where T : R1Serializable, new()
    {
        public T Value { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            base.SerializeImpl(s);
            Value = s.SerializeObject<T>(Value, name: nameof(Value));
        }
    }
}