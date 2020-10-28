using R1Engine.Serialize;
using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_LocIndex : R1Serializable
    {
        public int LocIndex { get; set; }

        public string GetString(int lang = 0) => Context.GetStoredObject<string[]>("Loc")?.ElementAtOrDefault(LocIndex);

        public override void SerializeImpl(SerializerObject s)
        {
            LocIndex = s.Serialize<int>(LocIndex, name: nameof(LocIndex));

            var locString = GetString();
            
            if (locString != null)
                s.Log($"String: {locString}");
        }
    }
}