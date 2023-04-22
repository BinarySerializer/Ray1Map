using BinarySerializer;
using System;

namespace Ray1Map.Jade
{
    public class AG_AnimGraph : Jade_File {
		//public override string Export_Extension => "aig"; // Unknown
		public override bool HasHeaderBFFile => true;

        protected override void SerializeFile(SerializerObject s)
        {
            throw new BinarySerializableException(this, $"{GetType()} is not yet implemented");
        }

    }
}