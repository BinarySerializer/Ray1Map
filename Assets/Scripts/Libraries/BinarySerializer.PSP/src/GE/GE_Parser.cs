using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinarySerializer.PSP
{
    public class GE_Parser
    {
        public List<GE_Command> SerializedCommands { get; set; }

        public Pointer StartPointer { get; set; }
        public bool AlignVertices { get; set; } = true;

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
            GE_Command_VertexType CurrentVertexType = null;
            GE_VertexLine[] Vertices = null;
            bool parse = true;

            s.Goto(StartPointer);

            Pointer GetPointer(uint address) {
                return StartPointer + BitHelpers.SetBits64(address, BASE, 4, 24);
            }

            while (parse) {
                Command = s.SerializeObject<GE_Command>(Command, name: nameof(Command));
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
                        s.Goto(GetPointer(((GE_Command_Address)Command.Data).Address));
                        break;
                    case GE_CommandType.JUMP:
                        s.Goto(GetPointer(((GE_Command_Address)Command.Data).Address));
                        break;
                    case GE_CommandType.BJUMP:
                        if (Condition) s.Goto(GetPointer(((GE_Command_Address)Command.Data).Address));
                        break;
                    case GE_CommandType.VTYPE:
                        CurrentVertexType = (GE_Command_VertexType)Command.Data;
                        break;
                    case GE_CommandType.BASE:
                        BASE = ((GE_Command_Base)Command.Data).Base;
                        break;
                    case GE_CommandType.VADDR:
                        vertexListAddress = GetPointer(((GE_Command_Address)Command.Data).Address);
                        break;
                    case GE_CommandType.IADDR:
                        indexListAddress = GetPointer(((GE_Command_Address)Command.Data).Address);
                        break;
                    case GE_CommandType.PRIM:
                        var primData = (GE_Command_PrimitiveKick)Command.Data;
                        s.DoAt(vertexListAddress, () => {
                            Vertices = s.SerializeObjectArray<GE_VertexLine>(Vertices, primData.VerticesCount,
                                onPreSerialize: v => v.Pre_VertexType = CurrentVertexType,
                                name: nameof(Vertices));
                            vertexListAddress = s.CurrentPointer;
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