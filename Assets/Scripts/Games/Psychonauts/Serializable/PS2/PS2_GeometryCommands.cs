using System;
using System.Collections.Generic;
using System.IO;
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
        public VIF_Command[] Commands { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var parser = new VIF_Parser() {
                IsVIF1 = true
            };
            Commands = s.SerializeObjectArrayUntil<VIF_Command>(Commands, x => s.CurrentFileOffset >= s.CurrentLength, onPreSerialize: (c,_) => c.Pre_Parser = parser, name: nameof(Commands));
        }
    }
}