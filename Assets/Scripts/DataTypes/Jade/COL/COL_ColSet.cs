using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class COL_ColSet : Jade_File {
		public override string Export_Extension => "cmd";

		public byte ZDxCount { get; set; }
        public byte Flag { get; set; }
        public short InstancesCount { get; set; }
        public byte[] AI_Indices { get; set; } // Indices for COL_Instance.Design
        public COL_ZDx[] ZDx { get; set; }

        protected override void SerializeFile(SerializerObject s) {
            ZDxCount = s.Serialize<byte>(ZDxCount, name: nameof(ZDxCount));
            Flag = s.Serialize<byte>(Flag, name: nameof(Flag));
            if(!Loader.IsBinaryData) InstancesCount = s.Serialize<short>(InstancesCount, name: nameof(InstancesCount));
            AI_Indices = s.SerializeArray<byte>(AI_Indices, 16, name: nameof(AI_Indices));
            ZDx = s.SerializeObjectArray<COL_ZDx>(ZDx, ZDxCount, onPreSerialize: c => c.IsInstance = false, name: nameof(ZDx));

        }
    }
}