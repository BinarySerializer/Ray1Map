using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class COL_Instance : Jade_File {
        public Jade_Reference<COL_ColSet> ColSet { get; set; }
        public byte ZDxCount { get; set; }
        public byte SharedCount { get; set; }
        public byte SpecificCount { get; set; }
        public byte Priority { get; set; }
        public byte[] SharedZDx { get; set; }
        public COL_ZDx[] SpecificZDx { get; set; }
        public short Activation { get; set; }
        public short Specific { get; set; }
        public short Short_14_Editor { get; set; }
        public short Crossable { get; set; }
        public short Flags { get; set; }
        public byte MaxLOD { get; set; }
        public byte MinLOD { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            ColSet = s.SerializeObject<Jade_Reference<COL_ColSet>>(ColSet, name: nameof(ColSet))?.Resolve();
            if (FileSize == 4) return;

            ZDxCount = s.Serialize<byte>(ZDxCount, name: nameof(ZDxCount));
            SharedCount = s.Serialize<byte>(SharedCount, name: nameof(SharedCount));
            SpecificCount = s.Serialize<byte>(SpecificCount, name: nameof(SpecificCount));
            if (ZDxCount == SharedCount + SpecificCount) {
                Priority = s.Serialize<byte>(Priority, name: nameof(Priority));
                SharedZDx = s.SerializeArray<byte>(SharedZDx, ZDxCount, name: nameof(SharedZDx)); // Indices of ZDx in ColSet?
                SpecificZDx = s.SerializeObjectArray<COL_ZDx>(SpecificZDx, SpecificCount, onPreSerialize: c => c.IsInstance = true, name: nameof(SpecificZDx));
                Activation = s.Serialize<short>(Activation, name: nameof(Activation));
                Specific = s.Serialize<short>(Specific, name: nameof(Specific));
                if (!Loader.IsBinaryData) Short_14_Editor = s.Serialize<short>(Short_14_Editor, name: nameof(Short_14_Editor));
                Crossable = s.Serialize<short>(Crossable, name: nameof(Crossable));
                Flags = s.Serialize<short>(Flags, name: nameof(Flags));
                MaxLOD = s.Serialize<byte>(MaxLOD, name: nameof(MaxLOD));
                MinLOD = s.Serialize<byte>(MinLOD, name: nameof(MinLOD));
            }
        }
	}
}