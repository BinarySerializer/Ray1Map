using BinarySerializer;
using System;

namespace Ray1Map.Jade
{
    public class AG_AnimGraphFile : Jade_File {
		//public override string Export_Extension => "aig"; // Unknown
		public override bool HasHeaderBFFile => true;
        public AG_AnimGraph AnimGraph { get; set; }

        protected override void SerializeFile(SerializerObject s)
        {
            AnimGraph = s.SerializeObject<AG_AnimGraph>(AnimGraph, name: nameof(AnimGraph));
            AnimGraph?.FightMatrix?.Resolve();
            s.Goto(Offset + FileSize);
        }

    }
}