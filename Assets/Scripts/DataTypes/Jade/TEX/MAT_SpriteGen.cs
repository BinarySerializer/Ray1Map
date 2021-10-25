using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class MAT_SpriteGen : BinarySerializable {
        public uint Identifier1 { get; set; }
        public uint Identifier2 { get; set; }
        public Jade_TextureReference BumpMap { get; set; }
        public ushort Flags { get; set; }
        public ushort TextureIndex { get; set; }
        public float SizeFloat { get; set; }
        public float ZExtraction { get; set; }
        public uint EnableTexture { get; set; }
        public float Noise { get; set; }
        public float SizeNoise { get; set; }
        public float MipMapCoef { get; set; }
        public float DistortionMax { get; set; }
        public uint XYZSMap_BIGKEY { get; set; }
        public uint BumpMapPointer { get; set; }
        public float BumpFactor { get; set; }
        public uint[] Undefined { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Identifier1 = s.Serialize<uint>(Identifier1, name: nameof(Identifier1));
            Identifier2 = s.Serialize<uint>(Identifier2, name: nameof(Identifier2));
            BumpMap = s.SerializeObject<Jade_TextureReference>(BumpMap, name: nameof(BumpMap));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            TextureIndex = s.Serialize<ushort>(TextureIndex, name: nameof(TextureIndex));
            SizeFloat = s.Serialize<float>(SizeFloat, name: nameof(SizeFloat));
            ZExtraction = s.Serialize<float>(ZExtraction, name: nameof(ZExtraction));
            EnableTexture = s.Serialize<uint>(EnableTexture, name: nameof(EnableTexture));
            Noise = s.Serialize<float>(Noise, name: nameof(Noise));
            SizeNoise = s.Serialize<float>(SizeNoise, name: nameof(SizeNoise));
            MipMapCoef = s.Serialize<float>(MipMapCoef, name: nameof(MipMapCoef));
            DistortionMax = s.Serialize<float>(DistortionMax, name: nameof(DistortionMax));
            XYZSMap_BIGKEY = s.Serialize<uint>(XYZSMap_BIGKEY, name: nameof(XYZSMap_BIGKEY));
            BumpMapPointer = s.Serialize<uint>(BumpMapPointer, name: nameof(BumpMapPointer)); // Leftover
            BumpFactor = s.Serialize<float>(BumpFactor, name: nameof(BumpFactor));
            Undefined = s.SerializeArray<uint>(Undefined, 2, name: nameof(Undefined));

            BumpMap?.Resolve(s, RRR2_readBool: true);
        }
	}
}