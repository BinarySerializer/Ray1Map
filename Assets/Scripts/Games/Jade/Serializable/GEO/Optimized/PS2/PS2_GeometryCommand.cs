using BinarySerializer;
using BinarySerializer.PS2;

namespace Ray1Map.Jade {
    public class PS2_GeometryCommand : BinarySerializable {
        public VIFcode VIFCode { get; set; }
        public CommandType Type { get; set; }

        public PS2_Vector16[] Vertices { get; set; }
        public PS2_UV16[] UVs { get; set; }
        public RGB888Color[] VertexColorsRGB { get; set; }
        public PS2_RGBA8888Color[] VertexColorsRGBA { get; set; }
        public PS2_Vector4_32[] V4_VL32 { get; set; }
        public PS2_Vector3_32[] V3_VL32 { get; set; }


        public GSReg_TEX0_1 TEX0 { get; set; }
        public GIFtag GIFTag { get; set; }
        public GSReg_CLAMP_1 CLAMP { get; set; }
        public uint[] ROW { get; set; }
        public uint[] COL { get; set; }
        public uint MASK { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            VIFCode = s.SerializeObject<VIFcode>(VIFCode, name: nameof(VIFCode));

            if (VIFCode.CMD >= 0x60 && VIFCode.CMD <= 0x7F) {
                // UNPACK command
                VIFcode_Unpack unpack = new VIFcode_Unpack(VIFCode);
                s.Log($"VIF Command: [UNPACK] {unpack}");
                if (unpack.VN == VIFcode_Unpack.UnpackVN.V3 && unpack.VL == VIFcode_Unpack.UnpackVL.VL_32) {
                    V3_VL32 = s.SerializeObjectArray<PS2_Vector3_32>(V3_VL32, unpack.SIZE, name: nameof(V3_VL32));
                } else if (unpack.VN == VIFcode_Unpack.UnpackVN.V4 && unpack.VL == VIFcode_Unpack.UnpackVL.VL_32) {
                    // Addr = 0x53: Full vector4
                    // Addr = 3: w = int
                    // Addr = 0: Mixed?

                    V4_VL32 = s.SerializeObjectArray<PS2_Vector4_32>(V4_VL32, unpack.SIZE, name: nameof(V4_VL32));
                } else if (unpack.VN == VIFcode_Unpack.UnpackVN.V3 && unpack.VL == VIFcode_Unpack.UnpackVL.VL_16) {
                    Vertices = s.SerializeObjectArray<PS2_Vector16>(Vertices, unpack.SIZE, name: nameof(Vertices));
                    s.Align(4, baseOffset: Offset);
                    Type = CommandType.Vertices;
                } else if (unpack.VN == VIFcode_Unpack.UnpackVN.V2 && unpack.VL == VIFcode_Unpack.UnpackVL.VL_16) {
                    UVs = s.SerializeObjectArray<PS2_UV16>(UVs, unpack.SIZE, name: nameof(UVs));
                    Type = CommandType.UVs;
                } else if (unpack.VN == VIFcode_Unpack.UnpackVN.V3 && unpack.VL == VIFcode_Unpack.UnpackVL.VL_8) {
                    VertexColorsRGB = s.SerializeObjectArray<RGB888Color>(VertexColorsRGB, unpack.SIZE, name: nameof(VertexColorsRGB));
                    s.Align(4, baseOffset: Offset);
                    Type = CommandType.VertexColors;
                } else if (unpack.VN == VIFcode_Unpack.UnpackVN.V4 && unpack.VL == VIFcode_Unpack.UnpackVL.VL_8) {
                    // Addr = 0x83 for these
                    VertexColorsRGBA = s.SerializeObjectArray<PS2_RGBA8888Color>(VertexColorsRGBA, unpack.SIZE, name: nameof(VertexColorsRGBA));
                    Type = CommandType.VertexColors;
                } else if (unpack.VN == VIFcode_Unpack.UnpackVN.S && unpack.VL == VIFcode_Unpack.UnpackVL.VL_32) {
                    //
                } else {
                    throw new BinarySerializableException(this, $"Unknown VIF Unpack command for data type {unpack.VN}-{unpack.VL}");
                }
            } else {

                s.Log($"VIF Command: [{(VIF_Command)VIFCode.CMD}]");
                switch ((VIF_Command)VIFCode.CMD) {
                    case VIF_Command.STCYCL:
                    case VIF_Command.MSCNT:
                    case VIF_Command.NOP:
                        break;
                    case VIF_Command.STROW:
						ROW = s.SerializeArray<uint>(ROW, 4, name: nameof(ROW));
                        break;
                    case VIF_Command.STCOL:
						COL = s.SerializeArray<uint>(COL, 4, name: nameof(COL));
                        break;
                    case VIF_Command.STMASK:
						MASK = s.Serialize<uint>(MASK, name: nameof(MASK));
						break;
                    case VIF_Command.BASE:
                    case VIF_Command.OFFSET:
                        break;
					default:
                        throw new BinarySerializableException(this, $"Unknown VIF command: {VIFCode.CMD:X2} or {(VIF_Command)VIFCode.CMD}");
                }
            }
        }

        public enum CommandType {
            Vertices,
            SectionPosition,
            TriangleStrips,
            UVs,
            VertexColors,
            Tex0,
            TransferData,
            NOP
        }

        // https://psi-rockin.github.io/ps2tek/#vifcommands
        public enum VIF_Command {
            NOP    = 0x00,
            STCYCL = 0x01,
            OFFSET = 0x02,
            BASE   = 0x03,
            ITOP   = 0x04,
            STMOD  = 0x05,
            MSKPATH3 = 0x06,
            MARK     = 0x07,
            FLUSHE   = 0x10,
            FLUSH    = 0x11,
            FLUSHA   = 0x13,
            MSCAL    = 0x14,
            MSCALF   = 0x15,
            MSCNT    = 0x17,
            STMASK   = 0x20,
            STROW    = 0x30,
            STCOL    = 0x31,
            MPG      = 0x4A,
            DIRECT   = 0x50,
            DIRECTHL = 0x51,
            UNPACK   = 0x60,
        }
    }
}