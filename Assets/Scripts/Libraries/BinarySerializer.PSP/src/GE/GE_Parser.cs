using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinarySerializer.PSP
{
    public class GE_Parser
    {
        public List<GE_Command> SerializedCommands { get; set; } = new List<GE_Command>();

        public Pointer StartPointer { get; set; }
        public bool AlignVertices { get; set; } = true;
        public bool UseLineSizeLimitForTextures { get; set; } = false;

        public void Parse(SerializerObject s, Pointer startPointer = null) {
            if(startPointer != null)
                StartPointer = startPointer;

            uint BASE = 0;
            bool Condition = false;
            Stack<Pointer> CallStack = new Stack<Pointer>();

            GE_Command Command = null;
            Pointer newAddress = null;
            Pointer vertexListAddress = null;
            Pointer indexListAddress = null;
            Pointer textureAddress = null;
            GE_Command_VertexType CurrentVertexType = null;
            bool TextureMappingEnable = false;
            bool TextureSwizzleEnable = false;
            GE_PixelStorageMode? PixelStorageMode = null;
            GE_VertexLine[] Vertices = null;
            GE_Texture Texture = null;
            bool parse = true;

            s.Goto(StartPointer);

            Pointer GetPointer(uint address, uint msb) {
				return StartPointer + BitHelpers.SetBits64(address, msb, 4, 24);
			}
			Pointer GetPointerBase(uint address) => GetPointer(address, BASE);
			GE_Command_TextureBufferWidth GetTBW(int i) {
				return (GE_Command_TextureBufferWidth)SerializedCommands.Last(cmd => cmd.Command == (GE_CommandType)((int)GE_CommandType.TBW0 + i)).Data;
			}
			GE_Command_Address GetTBP(int i) {
				return (GE_Command_Address)SerializedCommands.Last(cmd => cmd.Command == (GE_CommandType)((int)GE_CommandType.TBP0 + i)).Data;
			}


			while (parse) {
                Command = s.SerializeObject<GE_Command>(Command, name: nameof(Command));
                SerializedCommands.Add(Command);
                switch (Command.Command) {
                    case GE_CommandType.END:
                        CallStack.Clear();
                        parse = false;
                        break;
                    case GE_CommandType.RET:
                        if (CallStack.TryPop(out newAddress)) {
                            s.Goto(newAddress);
                        } else parse = false;
                        break;
                    case GE_CommandType.CALL:
                        CallStack.Push(StartPointer);
                        s.Goto(GetPointerBase(((GE_Command_Address)Command.Data).Address));
                        break;
                    case GE_CommandType.JUMP:
                        s.Goto(GetPointerBase(((GE_Command_Address)Command.Data).Address));
                        break;
                    case GE_CommandType.BJUMP:
                        if (Condition) s.Goto(GetPointerBase(((GE_Command_Address)Command.Data).Address));
                        break;
                    case GE_CommandType.VTYPE:
                        CurrentVertexType = (GE_Command_VertexType)Command.Data;
                        break;
                    case GE_CommandType.BASE:
                        BASE = ((GE_Command_Base)Command.Data).Base;
                        break;
                    case GE_CommandType.VADDR:
                        vertexListAddress = GetPointerBase(((GE_Command_Address)Command.Data).Address);
                        break;
                    case GE_CommandType.IADDR:
                        indexListAddress = GetPointerBase(((GE_Command_Address)Command.Data).Address);
                        break;
                    case GE_CommandType.PRIM:
                        var primData = (GE_Command_PrimitiveKick)Command.Data;
                        s.DoAt(vertexListAddress, () => {
                            Vertices = s.SerializeObjectArray<GE_VertexLine>(default, primData.VerticesCount,
                                onPreSerialize: v => v.Pre_VertexType = CurrentVertexType,
                                name: nameof(Vertices));
                            Command.LinkedVertices = Vertices;
                            vertexListAddress = s.CurrentPointer;
                        });
                        break;
                    case GE_CommandType.TFUNC:
						var tfuncData = (GE_Command_TextureFunction)Command.Data;
                        // TODO
						break;
					case GE_CommandType.TMODE:
						var tmodeData = (GE_Command_TextureMode)Command.Data;
                        TextureSwizzleEnable = tmodeData.SwizzleEnable;
						break;
                    case GE_CommandType.TPSM:
						var tpsmData = (GE_Command_TexturePixelStorageMode)Command.Data;
                        PixelStorageMode = tpsmData.PixelStorageMode;
						break;
					case GE_CommandType.TME:
                        TextureMappingEnable = ((GE_Command_Enable)Command.Data).Enable;
                        break;
                    case GE_CommandType.TFLUSH:
                        // TODO
                        break;
                    case GE_CommandType.TBP0:
                    case GE_CommandType.TBP1:
					case GE_CommandType.TBP2:
					case GE_CommandType.TBP3:
					case GE_CommandType.TBP4:
					case GE_CommandType.TBP5:
					case GE_CommandType.TBP6:
					case GE_CommandType.TBP7:
						// TODO
						break;
					case GE_CommandType.TBW0:
					case GE_CommandType.TBW1:
					case GE_CommandType.TBW2:
					case GE_CommandType.TBW3:
					case GE_CommandType.TBW4:
					case GE_CommandType.TBW5:
					case GE_CommandType.TBW6:
					case GE_CommandType.TBW7:
						// TODO
						break;
					case GE_CommandType.TSIZE0:
					case GE_CommandType.TSIZE1:
					case GE_CommandType.TSIZE2:
					case GE_CommandType.TSIZE3:
					case GE_CommandType.TSIZE4:
					case GE_CommandType.TSIZE5:
					case GE_CommandType.TSIZE6:
					case GE_CommandType.TSIZE7:
                        // TODO: move somewhere else?
						var tsizeData = (GE_Command_TextureSize)Command.Data;
						int levelIndex = ((byte)Command.Command - (byte)GE_CommandType.TSIZE0);
                        var tbw = GetTBW(levelIndex);
                        var tbp = GetTBP(levelIndex);

						Pointer texturePtr = GetPointer(tbp.Address, tbw.AddressMSB);
						s.DoAt(texturePtr, () => {
                            Texture = s.SerializeObject<GE_Texture>(default,
                                onPreSerialize: t => {
                                    t.Pre_TSIZE = tsizeData;
                                    t.Pre_TBW = tbw;
                                    t.Pre_Format = PixelStorageMode ?? GE_PixelStorageMode.RGBA8888;
									t.Pre_IsSwizzled = TextureSwizzleEnable;
                                    t.Pre_UseLineSizeLimit = UseLineSizeLimitForTextures;
                                },
                                name: nameof(Texture));
                            Command.LinkedTextureData = Texture;
							textureAddress = s.CurrentPointer;
						});
						break;
					case GE_CommandType.NOP:
                        break;
                    default:
                        throw new BinarySerializableException(Command, $"Unhandled command {Command.Command}");
                }
            }
        }
    }
}