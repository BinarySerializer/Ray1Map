using System;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public abstract class GBAIsometric_Spyro_DataRef : BinarySerializable
    {
        public abstract void DoAt(Action<long> action);
    }
}