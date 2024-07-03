using System;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Image;
using UnityEngine;

namespace Ray1Map.Jade
{
    public class TEX_Content_Cooked : BinarySerializable {
        public TEX_File Texture { get; set; }

        public ushort Version { get; set; }
        public NoriTextureSetType TextureSetType { get; set; }
        public uint TextureCount { get; set; }
        public VirtuosRemasterFormat CompressedFormat { get; set; }
        public uint CompressedSizeWithAlpha { get; set; }
        public uint CompressedSizeNoAlpha { get; set; }
        public bool HasAlpha { get; set; }
        public uint MipmapCount { get; set; }

        public uint CompressedSize => HasAlpha ? CompressedSizeWithAlpha : CompressedSizeNoAlpha;
        public uint CompressedSizeForMapsWithNoAlpha => TextureSetType == NoriTextureSetType.Custom ? CompressedSize : CompressedSizeNoAlpha;

        public NoriTexture Albedo { get; set; }
        public NoriTexture NormalMap { get; set; }
        public NoriTexture HeightEmissive { get; set; }
        public NoriTexture SpecGlossOccl { get; set; }
		//public NoriTexture BadData { get; set; }
		public float ShaderParam0 { get; set; }
        public float ShaderParam1 { get; set; }
        public string CustomShaderName { get; set; }


		public override void SerializeImpl(SerializerObject s) {
            uint FileSize = Texture.ContentSize;
            if (FileSize == 0) return;
            
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Version = s.Serialize<ushort>(Version, name: nameof(Version));
            if (Version != 14) return;
            TextureSetType = s.Serialize<NoriTextureSetType>(TextureSetType, name: nameof(TextureSetType));
            TextureCount = s.Serialize<uint>(TextureCount, name: nameof(TextureCount));
            CompressedFormat = s.Serialize<VirtuosRemasterFormat>(CompressedFormat, name: nameof(CompressedFormat));
            CompressedSizeWithAlpha = s.Serialize<uint>(CompressedSizeWithAlpha, name: nameof(CompressedSizeWithAlpha));
            CompressedSizeNoAlpha = s.Serialize<uint>(CompressedSizeNoAlpha, name: nameof(CompressedSizeNoAlpha));
            HasAlpha = s.Serialize<bool>(HasAlpha, name: nameof(HasAlpha));
            MipmapCount = s.Serialize<uint>(MipmapCount, name: nameof(MipmapCount));

            if(CompressedSize == 0) return;

            if (TextureCount == 0) {
				// BadData = SerializeTexture(BadData, 0, name: nameof(BadData));
				Albedo = SerializeTexture(Albedo, 0, name: nameof(Albedo));
			} else {
                Albedo = SerializeTexture(Albedo, 0, name: nameof(Albedo));
                NormalMap = SerializeTexture(NormalMap, 1, name: nameof(NormalMap));
                HeightEmissive = SerializeTexture(HeightEmissive, 2, name: nameof(HeightEmissive));
                SpecGlossOccl = SerializeTexture(SpecGlossOccl, 3, name: nameof(SpecGlossOccl));
            }
            if (TextureSetType <= NoriTextureSetType.Type6 && TextureSetType != NoriTextureSetType.Invalid) {
                ShaderParam0 = s.Serialize<float>(ShaderParam0, name: nameof(ShaderParam0));
            }
            if (TextureSetType == NoriTextureSetType.Type6) {
                ShaderParam1 = s.Serialize<float>(ShaderParam1, name: nameof(ShaderParam1));
            }

			if (TextureSetType == NoriTextureSetType.Type6 || TextureSetType == NoriTextureSetType.Custom) {
                CustomShaderName = s.SerializeString(CustomShaderName, length: 0x40, name: nameof(CustomShaderName));
            }

			NoriTexture SerializeTexture(NoriTexture t, int index, string name = null) {
                if(index >= TextureCount && index != 0) return t;
                uint size = index >= 1 ? CompressedSizeForMapsWithNoAlpha : CompressedSize;
                t = s.SerializeObject<NoriTexture>(t, onPreSerialize: t => t.Pre_Size = size, name: name);
                return t;
            }
        }


        public enum NoriTextureSetType : uint {
            Invalid = 0,
            Simple = 1,
            Standard = 2,
            Parallax = 3,
            Emissive = 4,
            Custom = 5,
            Type6 = 6
		}

		public enum VirtuosRemasterFormat : uint {
			Invalid = 0,
            RGBA8 = 1,
            DXT = 2,
            ASTC = 3,
		}

        public class NoriTexture : BinarySerializable {
            public uint Pre_Size { get; set; }

            public byte[] Data { get; set; }
			
            public override void SerializeImpl(SerializerObject s) {
                Data = s.SerializeArray<byte>(Data, Pre_Size, name: nameof(Data));
            }
		}

        public Texture2D ToTexture2D() {
            if(Albedo == null) return null;
            var w = Texture.Width;
            var h = Texture.Height;

            Texture2D texture = null;
            switch (CompressedFormat) {
                case VirtuosRemasterFormat.DXT:
                    if (!HasAlpha) {
                        ushort nextMultiple(ushort u) {
                            if (u % 4 != 0) {
                                //u = (ushort)(u + (4 - (u % 4)));
                                u = (ushort)((u / 4) * 4);
                            }
                            return u;
                        }
                        h = nextMultiple(h);
                        w = nextMultiple(w);
                    }

                    texture = new Texture2D(w, h, HasAlpha ? TextureFormat.DXT5 : TextureFormat.DXT1, (int)MipmapCount, false);
					texture.LoadRawTextureData(Albedo.Data);
					texture.Apply();
					break;
                case VirtuosRemasterFormat.ASTC:
					texture = new Texture2D(w, h, TextureFormat.ASTC_6x6, (int)MipmapCount, false);
					texture.LoadRawTextureData(Albedo.Data);
					texture.Apply();
                    break;
                default:
                    throw new NotImplementedException($"Unimplemented type {CompressedFormat}");
			}
            return texture;
        }
	}
}