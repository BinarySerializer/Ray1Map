using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_GeometryCommands : BinarySerializable
    {
        public PS2_GeometryCommand[] Commands { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Commands = s.SerializeObjectArrayUntil<PS2_GeometryCommand>(Commands, x => s.CurrentFileOffset >= s.CurrentLength, name: nameof(Commands));
        }
    }
}