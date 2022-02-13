using System;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_LocIndex : BinarySerializable
    {
        public bool? Pre_Is32Bit { get; set; }
        public Func<int, int> Pre_ParseIndexFunc { get; set; }

        public int LocIndex { get; set; }

        public string GetString(int lang = 0) => Context.GetStoredObject<string[][]>("Loc")?.ElementAtOrDefault(lang)?.ElementAtOrDefault(Pre_ParseIndexFunc?.Invoke(LocIndex) ?? LocIndex);

        public override void SerializeImpl(SerializerObject s)
        {
            if (Pre_Is32Bit ?? s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_RHR)
                LocIndex = s.Serialize<int>(LocIndex, name: nameof(LocIndex));
            else
                LocIndex = s.Serialize<ushort>((ushort)LocIndex, name: nameof(LocIndex));

            var locString = GetString();
            
            if (locString != null)
                s.Log("String: {0}", locString);
        }
    }
}