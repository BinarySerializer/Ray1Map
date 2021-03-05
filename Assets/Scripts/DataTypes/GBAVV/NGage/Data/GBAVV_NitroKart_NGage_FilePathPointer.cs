using System;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_FilePathPointer : R1Serializable
    {
        // Set before serializing
        public bool IsRelativePath { get; set; }

        public Pointer FilePathPointer { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_NGage_FilePath FilePath { get; set; }

        // Helpers
        public void DoAtFile(Action action) => FilePath.DoAtFile(action);

        public override void SerializeImpl(SerializerObject s)
        {
            FilePathPointer = s.SerializePointer(FilePathPointer, name: nameof(FilePathPointer));

            FilePath = s.DoAt(FilePathPointer, () => s.SerializeObject<GBAVV_NitroKart_NGage_FilePath>(FilePath, x => x.IsRelativePath = IsRelativePath, name: nameof(FilePath)));
        }
    }
}