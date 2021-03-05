using System;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_FilePath : R1Serializable
    {
        // Set before serializing
        public bool IsRelativePath { get; set; }
        public long? StringLength { get; set; }

        public string FilePath { get; set; }

        public void DoAtFile(Action action)
        {
            if (IsRelativePath)
                throw new Exception($"{nameof(DoAtFile)} can't be called on a relative path!");

            var manager = (GBAVV_NitroKart_NGage_Manager)Context.Settings.GetGameManager;

            var absolutePath = FilePath;

            manager.DoAtBlock(Context, absolutePath, action);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            FilePath = s.SerializeString(FilePath, StringLength, name: nameof(FilePath));
        }
    }
}