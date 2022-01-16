﻿using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_DialogData : BinarySerializable
    {
        public Entry[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Entries == null)
            {
                var entries = new List<Entry>();

                while (entries.LastOrDefault()?.Values.First().IsLastEntry != true)
                    entries.Add(s.SerializeObject<Entry>(default, name: $"{nameof(Entries)}[{entries.Count}]"));

                Entries = entries.ToArray();
            }
            else
            {
                s.SerializeObjectArray<Entry>(Entries, Entries.Length, name: nameof(Entries));
            }
        }

        public class Entry : BinarySerializable
        {
            public Value[] Values { get; set; }

            // Parsed data
            public int PortraitIndex { get; set; }
            public GBAIsometric_LocIndex[] LocIndices { get; set; }
            public GBAIsometric_LocIndex[] MultiChoiceLocIndices { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                if (Values == null)
                {
                    var values = new List<Value>();

                    var initialValue = s.SerializeObject<Value>(default, x => x.IsInstruction = true, name: $"InitialValue");
                    values.Add(initialValue);

                    switch (initialValue.Instruction)
                    {
                        case Instruction.DrawPortrait:
                            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro3)
                                values.Add(s.SerializeObject<Value>(default, name: $"{nameof(PortraitIndex)}"));

                            PortraitIndex = values.Last().Param;

                            break;

                        case Instruction.DrawText:
                        case Instruction.DrawMultiChoiceText:
                            var locIndices = new List<GBAIsometric_LocIndex>();

                            bool read = true;

                            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2)
                            {
                                var l = new GBAIsometric_LocIndex()
                                {
                                    LocIndex = initialValue.Param
                                };
                                l.Init(s.CurrentPointer);
                                locIndices.Add(l);

                                read = !initialValue.IsLastParameter;
                            }

                            var index = 0;

                            while (read)
                            {
                                var v = s.SerializeObject<Value>(default, name: $"{nameof(LocIndices)}[{index++}]");

                                var l = new GBAIsometric_LocIndex()
                                {
                                    LocIndex = v.Param
                                };
                                l.Init(s.CurrentPointer);
                                locIndices.Add(l);

                                if (v.IsLastParameter)
                                    read = false;
                            }

                            LocIndices = locIndices.ToArray();
                            break;

                        case Instruction.MoveCamera:
                            // No data gets parsed here, instead it jumps to r3
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(initialValue.Instruction), initialValue.Instruction, null);
                    }

                    Values = values.ToArray();
                }
                else
                {
                    s.SerializeObjectArray<Value>(Values, Values.Length, name: nameof(Values));
                }

                if (Values.First().Instruction == Instruction.DrawMultiChoiceText)
                    MultiChoiceLocIndices = s.SerializeObjectArray<GBAIsometric_LocIndex>(MultiChoiceLocIndices, 4, x => x.ParseIndexFunc = i => i == 0 ? -1 : i, name: nameof(MultiChoiceLocIndices));
            }

        }

        public class Value : BinarySerializable
        {
            public Instruction Instruction { get; set; }
            public bool IsLastEntry { get; set; }
            public bool HasMultiChoiceIndices { get; set; }
            public bool IsInstruction { get; set; }
            public bool IsLastParameter { get; set; }
            public int Param { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                s.DoBits<ushort>(b =>
                {
                    if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro2)
                    {
                        Param = b.SerializeBits<int>(Param, 10, name: nameof(Param));
                        b.SerializeBits<int>(default, 1, name: "Padding");
                        IsLastEntry = b.SerializeBits<int>(IsLastEntry ? 1 : 0, 1, name: nameof(IsLastEntry)) == 1;
                        IsLastParameter = b.SerializeBits<int>(IsLastParameter ? 1 : 0, 1, name: nameof(IsLastParameter)) == 1;
                        HasMultiChoiceIndices = b.SerializeBits<int>(HasMultiChoiceIndices ? 1 : 0, 1, name: nameof(HasMultiChoiceIndices)) == 1;
                        Instruction = (Instruction)b.SerializeBits<int>((byte)Instruction, 2, name: nameof(Instruction));
                    }
                    else
                    {
                        if (IsInstruction)
                        {
                            Instruction = (Instruction)b.SerializeBits<int>((byte)Instruction, 4, name: nameof(Instruction));
                            b.SerializeBits<int>(default, 6, name: "Padding");
                            HasMultiChoiceIndices = b.SerializeBits<int>(HasMultiChoiceIndices ? 1 : 0, 1, name: nameof(HasMultiChoiceIndices)) == 1;
                            IsLastEntry = b.SerializeBits<int>(IsLastEntry ? 1 : 0, 1, name: nameof(IsLastEntry)) == 1;
                            b.SerializeBits<int>(default, 2, name: "Padding");
                            IsInstruction = b.SerializeBits<int>(IsInstruction ? 1 : 0, 1, name: nameof(IsInstruction)) == 1;
                            IsLastParameter = b.SerializeBits<int>(IsLastParameter ? 1 : 0, 1, name: nameof(IsLastParameter)) == 1;
                        }
                        else
                        {
                            Param = b.SerializeBits<int>(Param, 12, name: nameof(Param));
                            b.SerializeBits<int>(default, 2, name: "Padding");
                            IsInstruction = b.SerializeBits<int>(IsInstruction ? 1 : 0, 1, name: nameof(IsInstruction)) == 1;
                            IsLastParameter = b.SerializeBits<int>(IsLastParameter ? 1 : 0, 1, name: nameof(IsLastParameter)) == 1;
                        }
                    }
                });
            }
        }

        public enum Instruction : byte
        {
            DrawPortrait = 0,
            DrawText = 1,
            DrawMultiChoiceText = 2,
            MoveCamera = 3
        }
    }
}