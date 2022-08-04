using BinarySerializer;
using BinarySerializer.PS2;

namespace Ray1Map.Psychonauts
{
    public class PS2_GeometryCommand : BinarySerializable
    {
        public VIFcode VIFCode { get; set; }

        public GIFtag GIFTag { get; set; }

        public PS2_Vector3_Int16[] Vertices { get; set; }
        public PS2_Vector3_Int8[] Normals { get; set; }

        public RGBA8888Color[] VertexColors { get; set; }
        public PS2_UV16[] UVs { get; set; }

        public PS2_Vector4_Float32[] V4_VL32 { get; set; }
        public byte[] S_VL8 { get; set; }

        public uint[] ROW { get; set; }
        public uint[] COL { get; set; }
        public uint MASK { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            VIFCode = s.SerializeObject<VIFcode>(VIFCode, name: nameof(VIFCode));

            if (VIFCode.IsUnpack)
            {
                // UNPACK command
                VIFcode_Unpack unpack = VIFCode.GetUnpack();

                s.Log("VIF Command: [UNPACK] {0}", unpack);

                if (unpack.M)
                    return;

                switch (unpack.VN)
                {
                    case VIFcode_Unpack.UnpackVN.V4 when unpack.VL == VIFcode_Unpack.UnpackVL.VL_32:
                    {
                        if (unpack.ADDR == 0)
                        {
                            GIFTag = s.SerializeObject<GIFtag>(GIFTag, name: nameof(GIFTag));
                            V4_VL32 = s.SerializeObjectArray<PS2_Vector4_Float32>(V4_VL32, unpack.SIZE - 1, name: nameof(V4_VL32));
                        }
                        else
                        {
                            throw new BinarySerializableException(this);
                        }

                        break;
                    }

                    case VIFcode_Unpack.UnpackVN.V4 when unpack.VL == VIFcode_Unpack.UnpackVL.VL_8:
                        VertexColors = s.SerializeObjectArray<RGBA8888Color>(VertexColors, unpack.SIZE, name: nameof(VertexColors));
                        break;

                    case VIFcode_Unpack.UnpackVN.V3 when unpack.VL == VIFcode_Unpack.UnpackVL.VL_16:
                        Vertices = s.SerializeObjectArray<PS2_Vector3_Int16>(Vertices, unpack.SIZE, name: nameof(Vertices));
                        s.Align(4, baseOffset: Offset);
                        break;

                    case VIFcode_Unpack.UnpackVN.V3 when unpack.VL == VIFcode_Unpack.UnpackVL.VL_8:
                        Normals = s.SerializeObjectArray<PS2_Vector3_Int8>(Normals, unpack.SIZE, name: nameof(Normals));
                        s.Align(4, baseOffset: Offset);
                        break;

                    case VIFcode_Unpack.UnpackVN.V2 when unpack.VL == VIFcode_Unpack.UnpackVL.VL_16:
                        UVs = s.SerializeObjectArray<PS2_UV16>(UVs, unpack.SIZE, name: nameof(UVs));
                        break;

                    case VIFcode_Unpack.UnpackVN.S when unpack.VL == VIFcode_Unpack.UnpackVL.VL_8:
                        S_VL8 = s.SerializeArray<byte>(S_VL8, unpack.SIZE, name: nameof(S_VL8));
                        s.Align(4, baseOffset: Offset);
                        break;

                    default:
                        throw new BinarySerializableException(this, $"Unknown VIF Unpack command for data type {unpack.VN}-{unpack.VL}");
                }
            }
            else
            {
                s.Log("VIF Command: [{0}]", VIFCode.CMD);

                switch (VIFCode.CMD)
                {
                    case VIFcode.Command.NOP:
                    case VIFcode.Command.BASE:
                    case VIFcode.Command.OFFSET:
                    case VIFcode.Command.STMOD:
                    case VIFcode.Command.MSCAL:
                    case VIFcode.Command.MSCALF:
                    case VIFcode.Command.MSCNT: // Transfer data
                    case VIFcode.Command.STCYCL:
                        break;

                    case VIFcode.Command.STROW:
                        ROW = s.SerializeArray<uint>(ROW, 4, name: nameof(ROW));
                        break;

                    case VIFcode.Command.STCOL:
                        COL = s.SerializeArray<uint>(COL, 4, name: nameof(COL));
                        break;

                    case VIFcode.Command.STMASK:
                        MASK = s.Serialize<uint>(MASK, name: nameof(MASK));
                        break;

                    default:
                        throw new BinarySerializableException(this, $"Unexpected VIF command: {VIFCode.CMD} ({(int)VIFCode.CMD:X2})");
                }
            }
        }
    }
}