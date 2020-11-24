using System.Linq;
using System.Text;

namespace R1Engine
{
    public class GBC_PalmOS_Block : R1Serializable {
        public byte[] Header { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Header = s.SerializeArray<byte>(Header, 4, name: nameof(Header));
        }
    }
}