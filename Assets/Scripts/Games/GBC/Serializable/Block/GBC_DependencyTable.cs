﻿using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBC
{
    public class GBC_DependencyTable : BinarySerializable {
        public uint DependenciesCount { get; set; }
        public GBC_Dependency[] Dependencies { get; set; }


        public static List<GBC_DependencyTable> DependencyTables { get; } = new List<GBC_DependencyTable>();
        public bool[] UsedDependencies { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            if (s.GetR1Settings().EngineVersion == EngineVersion.GBC_R1)
		        DependenciesCount = s.Serialize<byte>((byte)DependenciesCount, name: nameof(DependenciesCount));
            else
		        DependenciesCount = s.Serialize<uint>(DependenciesCount, name: nameof(DependenciesCount));
            Dependencies = s.SerializeObjectArray<GBC_Dependency>(Dependencies, DependenciesCount, name: nameof(Dependencies));

            // For export
            UsedDependencies = new bool[DependenciesCount];
            DependencyTables.Add(this);
        }

        public Pointer GetPointer(int index) {
            UsedDependencies[index] = true;

            if (Context.GetR1Settings().EngineVersion == EngineVersion.GBC_R1_Palm ||
                Context.GetR1Settings().EngineVersion == EngineVersion.GBC_R1_PocketPC) {
                var offTable = Context.GetStoredObject<LUDI_GlobalOffsetTable>(GBC_BaseManager.GlobalOffsetTableKey);
                return offTable?.Resolve(Dependencies[index]);
            } else {
                var ptr = Dependencies[index];
                return ptr.GBC_Pointer.GetPointer();
            }
        }
    }
}