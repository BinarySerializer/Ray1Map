using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class EVE_Event_InterpolationKey : BinarySerializable
    {
        public EVE_Event Event { get; set; } // Set in onPreSerialize

        public ushort UShort_00 { get; set; }
        public TypeFlags Type { get; set; }

        public ushort Stored_UShort_00 { get; set; }
        public TypeFlags Stored_Type { get; set; }
        public ushort Stored_Buffer_UShort_00 { get; set; }
        public TypeFlags Stored_Buffer_Type { get; set; }

        public Jade_Quaternion Quaternion { get; set; }
        public Jade_Matrix Matrix { get; set; }
        public Jade_Vector Vector0 { get; set; }
        public Jade_Vector Vector1 { get; set; }
        public Jade_Vector Vector2 { get; set; }

        public uint Flag3_Count { get; set; }
        public float[] Flag3_Floats { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13)) {
                UShort_00 = s.Serialize<ushort>(UShort_00, name: nameof(UShort_00));
            } else {
                var firstEvent = Event.ListEvents.Events[0];
                if ((firstEvent.Ushort_00 & 0x8000) == 0) {
                    if (firstEvent.InterpolationKey_UInt.HasValue) {
                        throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                        //UShort_00 = (ushort)BitHelpers.ExtractBits((int)firstEvent.InterpolationKey_UInt, 15, 1);
                    } else if(firstEvent.InterpolationKey != null) {
                        UShort_00 = (ushort)(firstEvent.InterpolationKey.Stored_UShort_00 >> 1);
                    }
                }
            }
            if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_8)) {
                Type = s.Serialize<TypeFlags>(Type, name: nameof(Type));
            } else if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13)) {
                var firstEvent = Event.ListEvents.Events[0];
                if ((firstEvent.Ushort_00 & 0x8000) != 0) {
                    Type = TypeFlags.Flag_4 | TypeFlags.Flag_7;
                } else if (firstEvent.InterpolationKey_UInt.HasValue) {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                    //Type = (ushort)BitHelpers.ExtractBits((int)firstEvent.InterpolationKey_UInt, 16, 16);
                } else if (firstEvent.InterpolationKey != null) {
                    Type = firstEvent.InterpolationKey.Stored_Type;
                } else {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                }
            } else {
                var firstEvent = Event.ListEvents.Events[0];
                if ((firstEvent.Ushort_00 & 0x8000) != 0) {
                    Type = TypeFlags.Flag_4 | TypeFlags.Flag_7;
                } else if (firstEvent.InterpolationKey_UInt.HasValue) {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                    //Type = (ushort)BitHelpers.ExtractBits((int)firstEvent.InterpolationKey_UInt, 16, 16);
                } else if (firstEvent.InterpolationKey != null) {
                    if ((firstEvent.InterpolationKey.Stored_UShort_00 & 1) == 1) {
                        Type = firstEvent.InterpolationKey.Stored_Type;
                    } else {
                        Type = firstEvent.InterpolationKey.Stored_Buffer_Type;
                    }
                } else {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                }
            }
            var currentUShort_00 = UShort_00;
            if(Type.HasFlag(TypeFlags.Flag_4) && !Type.HasFlag(TypeFlags.Flag_7)) currentUShort_00 = 20;

            if (Type.HasFlag(TypeFlags.Flag_7)) {
                throw new NotImplementedException($"TODO: Implement {GetType()}: Type & 0x80");
            } else if(Type.HasFlag(TypeFlags.Flag_4) || Type.HasFlag(TypeFlags.Flag_0) || Type.HasFlag(TypeFlags.Flag_1) || Type.HasFlag(TypeFlags.Flag_2)) {
                if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13)) {
                    Stored_Type = Type;
                    Stored_UShort_00 = (ushort)((currentUShort_00 << 1) | 1);
                } else {
                    Stored_Buffer_UShort_00 = currentUShort_00;
                    Stored_Buffer_Type = Type;
                }
                if (!Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_8)) {
                    if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13))
                        currentUShort_00 -= 2;
                    if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12))
                        currentUShort_00 -= 2;
                }
                switch ((ushort)Type & 3) {
                    case 0:
                        if (Type.HasFlag(TypeFlags.Flag_4)) {
                            Quaternion = s.SerializeObject<Jade_Quaternion>(Quaternion, name: nameof(Quaternion));
                        } else if (Type.HasFlag(TypeFlags.Flag_2)) {
                            Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));
                        }
                        break;
                    case 1:
                        Vector0 = s.SerializeObject<Jade_Vector>(Vector0, name: nameof(Vector0));
                        break;
                    case 2:
                        Vector1 = s.SerializeObject<Jade_Vector>(Vector1, name: nameof(Vector1));
                        Vector0 = s.SerializeObject<Jade_Vector>(Vector0, name: nameof(Vector0));
                        break;
                    case 3:
                        Vector2 = s.SerializeObject<Jade_Vector>(Vector2, name: nameof(Vector2));
                        Vector1 = s.SerializeObject<Jade_Vector>(Vector1, name: nameof(Vector1));
                        Vector0 = s.SerializeObject<Jade_Vector>(Vector0, name: nameof(Vector0));
                        break;
                }
                if (Type.HasFlag(TypeFlags.Flag_3)) {
                    uint curSize = (uint)(s.CurrentPointer - Offset);
                    if (curSize < currentUShort_00) {
                        Flag3_Count = s.Serialize<uint>(Flag3_Count, name: nameof(Flag3_Count));
                        var count = Math.Max(Flag3_Count * 2 - 1, 0);
                        Flag3_Floats = s.SerializeArray<float>(Flag3_Floats, count, name: nameof(Flag3_Floats));
                    }
                }
            }
        }

        [Flags]
        public enum TypeFlags : ushort {
            None = 0,
            Flag_0 = 1 << 0,
            Flag_1 = 1 << 1,
            Flag_2 = 1 << 2,
            Flag_3 = 1 << 3,
            Flag_4 = 1 << 4,
            Flag_5 = 1 << 5,
            Flag_6 = 1 << 6,
            Flag_7 = 1 << 7,
            Flag_8 = 1 << 8,
            Flag_9 = 1 << 9,
            Flag_10 = 1 << 10,
            Flag_11 = 1 << 11,
            Flag_12 = 1 << 12,
            Flag_13 = 1 << 13,
            Flag_14 = 1 << 14,
            Flag_15 = 1 << 15,
        }
    }
}