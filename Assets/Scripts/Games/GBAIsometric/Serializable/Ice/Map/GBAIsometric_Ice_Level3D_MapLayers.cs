using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Ice_Level3D_MapLayers : BinarySerializable
    {
        public bool Pre_Resolve { get; set; } = true;

        public Pointer<GBAIsometric_Ice_Level3D_MapLayer>[] Layers { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Layers = s.SerializePointerArray(Layers, 4, resolve: Pre_Resolve, name: nameof(Layers));
        }
    }
}