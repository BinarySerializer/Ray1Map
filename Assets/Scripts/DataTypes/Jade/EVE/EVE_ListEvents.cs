using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class EVE_ListEvents : BinarySerializable
    {
        public EVE_Track Track { get; set; } // Set before serializing
        public uint EventsCount { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            EventsCount = s.Serialize<uint>(EventsCount, name: nameof(EventsCount));

            if (Track.Uint_04 > 0)
            {
                // TODO: Read data
            }

            // TODO: Read event array

            throw new NotImplementedException($"TODO: Implement {GetType()}");
        }
    }
}