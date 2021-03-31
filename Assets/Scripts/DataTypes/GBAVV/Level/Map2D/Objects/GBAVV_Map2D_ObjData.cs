using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class GBAVV_Map2D_ObjData : BinarySerializable
    {
        public ushort ObjectsCount { get; set; }
        public ushort ObjGroupsCount { get; set; }
        public Pointer ObjGroupsPointer { get; set; }
        public Pointer ObjParamsOffsetsPointer { get; set; }
        public Pointer ObjParamsPointer { get; set; }
        public Pointer Pointer_10 { get; set; } // Only referenced from function at 08023d8c in Crash 2?

        // Serialized from pointers
        public GBAVV_Map2D_ObjGroups[] ObjGroups { get; set; }
        public GBAVV_Map2D_Object[] Objects { get; set; }
        public IEnumerable<GBAVV_Map2D_Object> GetObjects => ObjGroupsCount == 0 ? Objects : ObjGroups.SelectMany(x => x.Objects);
        public ushort[] ObjParamsOffsets { get; set; }
        public byte[][] ObjParams { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ObjectsCount = s.Serialize<ushort>(ObjectsCount, name: nameof(ObjectsCount));
            ObjGroupsCount = s.Serialize<ushort>(ObjGroupsCount, name: nameof(ObjGroupsCount));
            ObjGroupsPointer = s.SerializePointer(ObjGroupsPointer, name: nameof(ObjGroupsPointer));
            ObjParamsOffsetsPointer = s.SerializePointer(ObjParamsOffsetsPointer, name: nameof(ObjParamsOffsetsPointer));
            ObjParamsPointer = s.SerializePointer(ObjParamsPointer, name: nameof(ObjParamsPointer));
            Pointer_10 = s.SerializePointer(Pointer_10, name: nameof(Pointer_10));

            if (ObjGroupsCount == 0)
                Objects = s.DoAt(ObjGroupsPointer, () => s.SerializeObjectArray<GBAVV_Map2D_Object>(Objects, ObjectsCount, name: nameof(Objects)));
            else
                ObjGroups = s.DoAt(ObjGroupsPointer, () => s.SerializeObjectArray<GBAVV_Map2D_ObjGroups>(ObjGroups, ObjGroupsCount, name: nameof(ObjGroups)));
            
            ObjParamsOffsets = s.DoAt(ObjParamsOffsetsPointer, () => s.SerializeArray<ushort>(ObjParamsOffsets, GetObjects.Max(x => x.ObjParamsIndex) + 1, name: nameof(ObjParamsOffsets)));

            s.DoAt(ObjParamsPointer, () =>
            {
                if (ObjParams == null)
                    ObjParams = new byte[ObjParamsOffsets.Length][];

                for (int i = 0; i < ObjParams.Length; i++)
                {
                    var length = i < ObjParams.Length - 1 ? ObjParamsOffsets[i + 1] - ObjParamsOffsets[i] : ObjParamsOffsetsPointer - (ObjParamsPointer + ObjParamsOffsets[i]);

                    // Make sure the length is reasonable (only really used for the last entry)
                    if (length < 0 || length > 64)
                    {
                        length = 8; // Default to 8

                        Debug.LogWarning($"Obj params length is invalid for entry {i}/{ObjParams.Length - 1}");
                    }

                    ObjParams[i] = s.SerializeArray<byte>(ObjParams[i], length, name: $"{nameof(ObjParams)}[{i}]");
                }
            });
        }
    }
}