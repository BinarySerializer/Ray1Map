using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_LocIndex : R1Serializable
    {
        public int LocIndex { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LocIndex = s.Serialize<int>(LocIndex, name: nameof(LocIndex));

            var loc = s.Context.GetStoredObject<string[]>("Loc");
            var locString = loc?.ElementAtOrDefault(LocIndex);
            
            if (locString != null)
                s.Log($"String: {locString}");
        }
    }
}