using System;

namespace R1Engine
{
    public class LUDI_AppInfo : R1Serializable {
        public LUDI_Header Header { get; set; }
        public LUDI_OffsetTable OffsetTable { get; set; }
        public LUDI_DataInfo DataInfo { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Header = s.SerializeObject<LUDI_Header>(Header, name: nameof(Header));
            switch (Header.Type) {
                case LUDI_Header.FileType.DataFile:
                    OffsetTable = s.SerializeObject<LUDI_OffsetTable>(OffsetTable, name: nameof(OffsetTable));
                    break;
                case LUDI_Header.FileType.SpecialFile:
                    DataInfo = s.SerializeObject<LUDI_DataInfo>(DataInfo, name: nameof(DataInfo));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}