using R1Engine.Serialize;
using System;
using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_LocIndex : R1Serializable
    {
        public Func<int, int> ParseIndexFunc { get; set; }
        public int LocIndex { get; set; }

        public string GetString(int lang = 0) => Context.GetStoredObject<string[][]>("Loc")?.ElementAtOrDefault(lang)?.ElementAtOrDefault(ParseIndexFunc?.Invoke(LocIndex) ?? LocIndex);

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_RHR)
                LocIndex = s.Serialize<int>(LocIndex, name: nameof(LocIndex));
            else
                LocIndex = s.Serialize<ushort>((ushort)LocIndex, name: nameof(LocIndex));

            var locString = GetString();
            
            if (locString != null)
                s.Log($"String: {locString}");
        }
    }
}