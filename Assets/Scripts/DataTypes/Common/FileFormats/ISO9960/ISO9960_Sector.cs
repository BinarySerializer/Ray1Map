using BinarySerializer;

namespace R1Engine
{
    public class ISO9960_Sector<T> : BinarySerializable where T : BinarySerializable, new() {

        public byte[] Sync { get; set; }
        public byte[] Header { get; set; }
        public byte[] SubHeader { get; set; }

        public T Object { get; set; } // sector data, 0x800 bytes reserved

        public byte[] EDC { get; set; }
        public byte[] ECC { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Sync = s.SerializeArray<byte>(Sync, 12, name: nameof(Sync));
            Header = s.SerializeArray<byte>(Header, 4,name: nameof(Header));
            SubHeader = s.SerializeArray<byte>(SubHeader, 8, name: nameof(SubHeader));

            Object = s.SerializeObject<T>(Object, name: nameof(Object));
            
            s.Goto(Offset + ISO9960_BinFile.SectorDataSize + ISO9960_BinFile.SectorHeaderSize);
            EDC = s.SerializeArray<byte>(EDC, 4, name: nameof(EDC));
            ECC = s.SerializeArray<byte>(ECC, 276, name: nameof(ECC));
        }
    }
}