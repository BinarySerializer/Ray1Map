using System;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.PSP;

namespace Ray1Map.Jade
{
    public class TEX_Content_JTX_PSP : BinarySerializable {
        public TEX_Content_JTX JTX { get; set; }

		public byte[] Bytes { get; set; }
		public GE_Command[] SerializedCommands { get; set; }

        public GE_Texture Texture { get; set; }

        private bool UseLineSizeLimit => Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_T2T_20051002);

        public override void SerializeImpl(SerializerObject s) {
            uint FileSize = JTX.PS2_Size;
            if (FileSize == 0) return;

            if (JTX.Format == TEX_Content_JTX.JTX_Format.Raw32) {
                // Somehow not a GE program?
                Texture = s.SerializeObject<GE_Texture>(Texture,
                    onPreSerialize: t => {
                        t.Pre_Format = GE_PixelStorageMode.RGBA8888;
                        t.Pre_TBW = new GE_Command_TextureBufferWidth() {
                            BufferWidth = (ushort)JTX.Width
						};
                        t.Pre_TSIZE = new GE_Command_TextureSize() {
                            TW = (byte)NextPow2(JTX.Width),
                            TH = (byte)NextPow2(JTX.Height),
						};
                        t.Pre_IsSwizzled = true;
                        t.Pre_UseLineSizeLimit = UseLineSizeLimit;
				    },
                    name: nameof(Texture));
            } else {
                // GE Program
                Bytes = s.SerializeArray<byte>(Bytes, FileSize, name: nameof(Bytes));

                if (SerializedCommands == null) {
                    Execute(Context);
                    Texture = SerializedCommands?.LastOrDefault(c => c.LinkedTextureData != null)?.LinkedTextureData;
                }
            }
            return;
		}



		public void Execute(Context context) {
			var progKey = $"GEData_{Offset}";
			using (Context c = new Context("", serializerLogger: new ParentContextSerializerLogger(context.SerializerLogger), systemLogger: context.SystemLogger)) {
				// Parse GE program
				var file = c.AddStreamFile(progKey, new MemoryStream(Bytes));
				try {
					var s = c.Deserializer;
					var parser = new GE_Parser() {
                        UseLineSizeLimitForTextures = UseLineSizeLimit
                    };
					parser.Parse(s, file.StartPointer);
					SerializedCommands = parser.SerializedCommands.ToArray();
				} finally {
					c.RemoveFile(file);
				}
			}
		}

        int NextPow2(uint x) {
            int power = 1;
            int mul = 0;
            while (power < x) {
                power *= 2;
                mul++;
            }
            return mul;
        }
    }
}