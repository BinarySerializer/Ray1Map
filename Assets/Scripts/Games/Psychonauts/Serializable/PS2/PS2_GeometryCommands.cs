using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.PS2;
using PsychoPortal;
using UnityEngine;
using RGBA8888Color = PsychoPortal.RGBA8888Color;

namespace Ray1Map.Psychonauts
{
    public class PS2_GeometryCommands : BinarySerializable
    {
        public PS2_GeometryCommand[] Commands { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Commands = s.SerializeObjectArrayUntil<PS2_GeometryCommand>(Commands, x => s.CurrentFileOffset >= s.CurrentLength, name: nameof(Commands));
        }

        public IEnumerable<Primitive> EnumeratePrimitives()
        {
            // Current data
            GIFtag tag = null;
            Vec3[] vertices = null;
            Vec3[] normals = null;
            RGBA8888Color[] vertexColors = null;
            UV[][] uvs = new UV[3][];

            // Registers
            uint[] row = new uint[4];
            uint[] col = new uint[4];
            uint mask = 0;

            foreach (PS2_GeometryCommand cmd in Commands)
            {
                if (cmd.VIFCode.IsUnpack)
                {
                    VIFcode_Unpack unpack = cmd.VIFCode.GetUnpack();
                    uint addr = unpack.ADDR;

                    // TODO: Handle masked unpacking in a more reliable way using the row/col values and checking the mask flags
                    switch (addr) // TODO: Is using the address like this reliable?
                    {
                        // GIFTag
                        case 0:
                            if (tag != null)
                                throw new Exception("Tag has already been set for this primitive");

                            tag = cmd.GIFTag;
                            break;

                        // Unused?
                        case 1:
                            throw new Exception("Data written to address 1");

                        // Vertices
                        case 2 when !unpack.M:
                            vertices = cmd.Vertices.Select(x => new Vec3()
                            {
                                // TODO: Fix the offset here. Hard-coding it to this only works in some levels. Game
                                //       seems to convert it to a float using the row data?
                                X = x.X - 0x8000,
                                Y = x.Y - 0x8000,
                                Z = x.Z - 0x8000,
                            }).ToArray();
                            break;
                        case 2 when unpack.M:
                            vertices = Enumerable.Range(0, tag.NLOOP).Select(x => new Vec3()).ToArray();
                            break;

                        case 3 when !unpack.M:
                            normals = cmd.Normals.Select(x => new Vec3()
                            {
                                // TODO: Correctly convert to float values
                                X = x.X,
                                Y = x.Y,
                                Z = x.Z,
                            }).ToArray();
                            break;
                        case 3 when unpack.M:
                            normals = Enumerable.Range(0, tag.NLOOP).Select(x => new Vec3()).ToArray();
                            break;

                        case 4 when !unpack.M:
                            vertexColors = cmd.VertexColors.Select(x => new RGBA8888Color
                            {
                                Red = x.R,
                                Green = x.G,
                                Blue = x.B,
                                Alpha = x.A
                            }).ToArray();
                            break;
                        case 4 when unpack.M:
                            vertexColors = Enumerable.Range(0, tag.NLOOP).Select(x => new RGBA8888Color()
                            {
                                Red = 128,
                                Green = 128,
                                Blue = 128,
                                Alpha = 255,
                            }).ToArray();
                            break;

                        // UV
                        case 5 when !unpack.M:
                        case 6 when !unpack.M:
                        case 7 when !unpack.M:
                            float toFloat(int value) => BitConverter.ToSingle(BitConverter.GetBytes(value));
                            int uvIndex = (int)(addr - 5);
                            uvs[uvIndex] = cmd.UVs.Select(x => new UV()
                            {
                                Pre_Version = 316, // TODO: This is a hack to use floats. Convert to integer values.

                                // TODO: Is this even correct? Value range should be 0-1, but it's usually above 250...
                                U_Float = toFloat(x.U | (int)row[0]),
                                V_Float = toFloat(x.V | (int)row[1])
                            }).ToArray();
                            break;

                        case 5 when unpack.M:
                        case 6 when unpack.M:
                        case 7 when unpack.M:
                            int mUvIndex = (int)(addr - 5);
                            uvs[mUvIndex] = Enumerable.Range(0, tag.NLOOP).Select(x => new UV()).ToArray();
                            break;

                    }
                }
                else
                {
                    switch (cmd.VIFCode.CMD)
                    {
                        case VIFcode.Command.MSCNT:
                            if (uvs[2] != null && (uvs[0] == null || uvs[1] == null))
                                throw new Exception("Invalid UV data");
                            if (uvs[1] != null && uvs[0] == null)
                                throw new Exception("Invalid UV data");

                            yield return new Primitive(tag, vertices, normals, vertexColors,
                                uvs.Where(x => x != null).ToArray());

                            // Reset
                            tag = null;
                            vertices = null;
                            normals = null;
                            vertexColors = null;
                            for (int i = 0; i < uvs.Length; i++)
                                uvs[i] = null;
                            
                            row = new uint[4];
                            col = new uint[4];
                            mask = 0;
                            break;

                        case VIFcode.Command.STROW:
                            row = cmd.ROW;
                            break;

                        case VIFcode.Command.STCOL:
                            col = cmd.COL;
                            Debug.LogWarning("Column command is used"); // Hopefully this is never used
                            break;

                        case VIFcode.Command.STMASK:
                            mask = cmd.MASK;
                            break;
                    }
                }

            }
        }

        public record Primitive(GIFtag GIFTag, Vec3[] Vertices, Vec3[] Normals, RGBA8888Color[] VertexColors, UV[][] UVs);
    }
}