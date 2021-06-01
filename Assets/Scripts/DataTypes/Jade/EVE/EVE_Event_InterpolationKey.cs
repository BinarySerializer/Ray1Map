using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class EVE_Event_InterpolationKey : BinarySerializable
    {
        public EVE_Event Event { get; set; } // Set in onPreSerialize

        public ushort StructSize { get; set; }
        public TypeFlags Type { get; set; }

        public ushort Stored_StructSize { get; set; }
        public TypeFlags Stored_Type { get; set; }
        public ushort Stored_Buffer_StructSize { get; set; }
        public TypeFlags Stored_Buffer_Type { get; set; }

        public Jade_CompressedQuaternion CompressedQuaternion { get; set; }

        public Jade_Quaternion Quaternion { get; set; }
        public Jade_CompressedQuaternion16 CompressedQuaternion16 { get; set; }
        public Jade_Matrix Matrix { get; set; }
        public Jade_Vector Vector0 { get; set; }
        public Jade_Vector Vector1 { get; set; }
        public Jade_Vector Vector2 { get; set; }

        public uint Flag3_Count { get; set; }
        public float[] Flag3_Floats { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13)) {
                StructSize = s.Serialize<ushort>(StructSize, name: nameof(StructSize));
            } else {
                var firstEvent = Event.ListEvents.Events[0];
                if ((firstEvent.NumFrames_InGame & 0x8000) == 0) {
                    if (firstEvent.InterpolationKey_T.HasValue) {
                        throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                        //UShort_00 = (ushort)BitHelpers.ExtractBits((int)firstEvent.InterpolationKey_UInt, 15, 1);
                    } else if(firstEvent.InterpolationKey != null) {
                        StructSize = (ushort)(firstEvent.InterpolationKey.Stored_StructSize >> 1);
                    }
                }
            }
            if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12)) {
                Type = s.Serialize<TypeFlags>(Type, name: nameof(Type));
            } else if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13)) {
                var firstEvent = Event.ListEvents.Events[0];
                if ((firstEvent.NumFrames_InGame & 0x8000) != 0) {
                    Type = TypeFlags.Flag_4 | TypeFlags.Flag_7;
                } else if (firstEvent.InterpolationKey_T.HasValue) {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                    //Type = (ushort)BitHelpers.ExtractBits((int)firstEvent.InterpolationKey_UInt, 16, 16);
                } else if (firstEvent.InterpolationKey != null) {
                    Type = firstEvent.InterpolationKey.Stored_Type;
                } else {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                }
            } else {
                var firstEvent = Event.ListEvents.Events[0];
                if ((firstEvent.NumFrames & 0x8000) != 0) {
                    Type = TypeFlags.Flag_4 | TypeFlags.Flag_7;
                } else if (firstEvent.InterpolationKey_T.HasValue) {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                    //Type = (ushort)BitHelpers.ExtractBits((int)firstEvent.InterpolationKey_UInt, 16, 16);
                } else if (firstEvent.InterpolationKey != null) {
                    if ((firstEvent.InterpolationKey.Stored_StructSize & 1) == 1) {
                        Type = firstEvent.InterpolationKey.Stored_Type;
                    } else {
                        Type = firstEvent.InterpolationKey.Stored_Buffer_Type;
                    }
                } else {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                }
            }
            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
                var currentStructSize = StructSize;
                if (Type.HasFlag(TypeFlags.Flag_4) && !Type.HasFlag(TypeFlags.Flag_2) && !Loader.IsBinaryData) {
                    if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13)) {
                        currentStructSize = 12;
                    }
                    if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12)) {
                        // TODO
                    }
                }

                bool Is780 = Type.HasFlag(TypeFlags.Flag_7) || Type.HasFlag(TypeFlags.Flag_8) || Type.HasFlag(TypeFlags.Flag_9) || Type.HasFlag(TypeFlags.Flag_10); // 0x780
                bool Is14 = Type.HasFlag(TypeFlags.Flag_2) || Type.HasFlag(TypeFlags.Flag_4);
                bool Is3 = Type.HasFlag(TypeFlags.Flag_0) || Type.HasFlag(TypeFlags.Flag_1);
                bool IsF800 = Type.HasFlag(TypeFlags.Flag_11) || Type.HasFlag(TypeFlags.Flag_12)
                    || Type.HasFlag(TypeFlags.Flag_13) || Type.HasFlag(TypeFlags.Flag_14) || Type.HasFlag(TypeFlags.Flag_15);
                bool Is17 = Type.HasFlag(TypeFlags.Flag_4) || Type.HasFlag(TypeFlags.Flag_0)
                    || Type.HasFlag(TypeFlags.Flag_1) || Type.HasFlag(TypeFlags.Flag_2);
                if (Is14 && Is780) {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                } else {
                    if (Is3 && IsF800) {
                        throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                    }
                    if (!Is17) return;

                    if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13) || Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_8)) {
                        Stored_Type = Type;
                        Stored_StructSize = (ushort)((currentStructSize << 1) | 1);
                    } else {
                        Stored_Buffer_StructSize = currentStructSize;
                        Stored_Buffer_Type = Type;
                    }

                    if (Event.Index != 0 && Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13))
                        currentStructSize -= 2;
                    if (Event.Index != 0 && Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12))
                        currentStructSize -= 2;

                    switch ((ushort)Type & 3) {
                        case 0:
                            if (Type.HasFlag(TypeFlags.Flag_4)) {
                                if (Loader.IsBinaryData) {
									CompressedQuaternion16 = s.SerializeObject<Jade_CompressedQuaternion16>(CompressedQuaternion16, name: nameof(CompressedQuaternion16));
								} else {
                                    Quaternion = s.SerializeObject<Jade_Quaternion>(Quaternion, name: nameof(Quaternion));
                                }
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
                        if (curSize < currentStructSize) {
                            Flag3_Count = s.Serialize<uint>(Flag3_Count, name: nameof(Flag3_Count));
                            var count = Math.Max(Flag3_Count * 2 - 1, 0);
                            Flag3_Floats = s.SerializeArray<float>(Flag3_Floats, count, name: nameof(Flag3_Floats));
                        }
                    }
                }
            } else {
                var currentStructSize = StructSize;
                if (Type.HasFlag(TypeFlags.Flag_4) && !Type.HasFlag(TypeFlags.Flag_7)) currentStructSize = 20;

                if (Type.HasFlag(TypeFlags.Flag_7)) {
                    CompressedQuaternion = s.SerializeObject<Jade_CompressedQuaternion>(CompressedQuaternion, name: nameof(CompressedQuaternion));
                    currentStructSize = 20;
                    if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12))
                        currentStructSize = 10;
                    if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13)
                        && Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12)
                        && Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_11)) {
                        Event.NumFrames_InGame |= 0x8000;
                        // If there's an error here later, good chance that it's here.
                        // It moves CompressedQuaternion into the event struct?
                    } else if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13)) {
                        Stored_Type = Type;
                        Stored_StructSize = (ushort)((currentStructSize << 1) | 1);
                        if (!Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12)) {
                            Stored_Type = Stored_Type & ~TypeFlags.Flag_7;
                        }
                    } else {
                        Stored_Buffer_StructSize = currentStructSize;
                        Stored_Buffer_Type = Type;
                        if (!Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12)) {
                            Stored_Buffer_Type = Stored_Buffer_Type & ~TypeFlags.Flag_7;
                        }
                    }
                } else if (Type.HasFlag(TypeFlags.Flag_4) || Type.HasFlag(TypeFlags.Flag_0) || Type.HasFlag(TypeFlags.Flag_1) || Type.HasFlag(TypeFlags.Flag_2)) {
                    if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13)) {
                        Stored_Type = Type;
                        Stored_StructSize = (ushort)((currentStructSize << 1) | 1);
                    } else {
                        Stored_Buffer_StructSize = currentStructSize;
                        Stored_Buffer_Type = Type;
                    }
                    if (!Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_8)) {
                        if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_13))
                            currentStructSize -= 2;
                        if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.Flag_12))
                            currentStructSize -= 2;
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
                        if (curSize < currentStructSize) {
                            Flag3_Count = s.Serialize<uint>(Flag3_Count, name: nameof(Flag3_Count));
                            var count = Math.Max(Flag3_Count * 2 - 1, 0);
                            Flag3_Floats = s.SerializeArray<float>(Flag3_Floats, count, name: nameof(Flag3_Floats));
                        }
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