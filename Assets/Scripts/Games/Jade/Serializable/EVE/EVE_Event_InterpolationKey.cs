using System;
using BinarySerializer;

namespace Ray1Map.Jade
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
        public Jade_CompressedQuaternion10 CompressedQuaternion10 { get; set; }
        public Jade_Matrix Matrix { get; set; }
        public Jade_Vector Vector0 { get; set; }
        public Jade_Vector Vector1 { get; set; }
        public Jade_Vector Vector2 { get; set; }

        public uint Flag3_Count { get; set; }
        public float[] Flag3_Floats { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameSize)) {
                StructSize = s.Serialize<ushort>(StructSize, name: nameof(StructSize));
            } else {
                var firstEvent = Event.ListEvents.Events[0];
                if (!firstEvent.HasUglyOptimizationFlag) {
                    if (firstEvent.InterpolationKey_T.HasValue) {
                        throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                        //UShort_00 = (ushort)BitHelpers.ExtractBits((int)firstEvent.InterpolationKey_UInt, 15, 1);
                    } else if(firstEvent.InterpolationKey != null) {
                        StructSize = (ushort)(firstEvent.InterpolationKey.Stored_StructSize >> 1);
                    }
                }
            }
            if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameType)) {
                Type = s.Serialize<TypeFlags>(Type, name: nameof(Type));
            } else if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameSize)) {
                var firstEvent = Event.ListEvents.Events[0];
                if (firstEvent.HasUglyOptimizationFlag) {
                    Type = TypeFlags.RotationQuaternion | TypeFlags.CompressedQuaternion;
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
                if (firstEvent.HasUglyOptimizationFlag) {
                    Type = TypeFlags.RotationQuaternion | TypeFlags.CompressedQuaternion;
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
                if (Type.HasFlag(TypeFlags.RotationQuaternion) && !Type.HasFlag(TypeFlags.RotationMatrix) && !Loader.IsBinaryData) {
                    if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameSize)) {
                        currentStructSize = 12;
                    }
                    if (Event.Index == 0 || !Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameType)) {
                        // TODO
                    }
                }

                bool CompressedRotation = Type.HasFlag(TypeFlags.CompressedQuaternion) || Type.HasFlag(TypeFlags.UltraCompressedQuaternionX) || Type.HasFlag(TypeFlags.UltraCompressedQuaternionY) || Type.HasFlag(TypeFlags.UltraCompressedQuaternionZ); // 0x780
                bool HasRotation = Type.HasFlag(TypeFlags.RotationMatrix) || Type.HasFlag(TypeFlags.RotationQuaternion);
                bool HasTranslation = Type.HasFlag(TypeFlags.Translation0) || Type.HasFlag(TypeFlags.Translation1);
                bool CompressedTranslation = Type.HasFlag(TypeFlags.CompressedAbsoluteTranslation) || Type.HasFlag(TypeFlags.CompressedRelativeTranslation)
                    || Type.HasFlag(TypeFlags.UltraCompressedTranslationX) || Type.HasFlag(TypeFlags.UltraCompressedTranslationY) || Type.HasFlag(TypeFlags.UltraCompressedTranslationZ);
                bool HasTranslationOrRotation = HasTranslation || HasRotation;
                if (HasRotation && CompressedRotation) {
                    throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                } else {
                    if (HasTranslation && CompressedTranslation) {
                        throw new NotImplementedException($"TODO: Implement {GetType()}: Figure out InterpolationKey");
                    }
                    if (!HasTranslationOrRotation) return;

                    if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameSize) || Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.FirstEvent)) {
                        Stored_Type = Type;
                        Stored_StructSize = (ushort)((currentStructSize << 1) | 1);
                    } else {
                        Stored_Buffer_StructSize = currentStructSize;
                        Stored_Buffer_Type = Type;
                    }

                    if (Event.Index != 0 && Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameSize))
                        currentStructSize -= 2;
                    if (Event.Index != 0 && Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameType))
                        currentStructSize -= 2;

                    switch ((ushort)Type & 3) {
                        case 0:
                            if (Type.HasFlag(TypeFlags.RotationQuaternion)) {
                                if (Loader.IsBinaryData) {
                                    if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW_20040920)) {
										CompressedQuaternion10 = s.SerializeObject<Jade_CompressedQuaternion10>(CompressedQuaternion10, name: nameof(CompressedQuaternion10));
									} else {
                                        CompressedQuaternion16 = s.SerializeObject<Jade_CompressedQuaternion16>(CompressedQuaternion16, name: nameof(CompressedQuaternion16));
                                    }
								} else {
                                    Quaternion = s.SerializeObject<Jade_Quaternion>(Quaternion, name: nameof(Quaternion));
                                }
                            } else if (Type.HasFlag(TypeFlags.RotationMatrix)) {
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
                    if (Type.HasFlag(TypeFlags.Time)) {
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
                if (Type.HasFlag(TypeFlags.RotationQuaternion) && !Type.HasFlag(TypeFlags.CompressedQuaternion)) currentStructSize = 20;

                if (Type.HasFlag(TypeFlags.CompressedQuaternion)) {
                    CompressedQuaternion = s.SerializeObject<Jade_CompressedQuaternion>(CompressedQuaternion, name: nameof(CompressedQuaternion));
                    currentStructSize = 20;
                    if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameType))
                        currentStructSize = 10;
                    if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameSize)
                        && Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameType)
                        && Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameFlags)) {
                        Event.HasUglyOptimizationFlag = true;
                        // If there's an error here later, good chance that it's here.
                        // It moves CompressedQuaternion into the event struct?
                    } else if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameSize)) {
                        Stored_Type = Type;
                        Stored_StructSize = (ushort)((currentStructSize << 1) | 1);
                        if (!Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameType)) {
                            Stored_Type = Stored_Type & ~TypeFlags.CompressedQuaternion;
                        }
                    } else {
                        Stored_Buffer_StructSize = currentStructSize;
                        Stored_Buffer_Type = Type;
                        if (!Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameType)) {
                            Stored_Buffer_Type = Stored_Buffer_Type & ~TypeFlags.CompressedQuaternion;
                        }
                    }
                } else if (Type.HasFlag(TypeFlags.RotationQuaternion) || Type.HasFlag(TypeFlags.Translation0) || Type.HasFlag(TypeFlags.Translation1) || Type.HasFlag(TypeFlags.RotationMatrix)) {
                    if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameSize)) {
                        Stored_Type = Type;
                        Stored_StructSize = (ushort)((currentStructSize << 1) | 1);
                    } else {
                        Stored_Buffer_StructSize = currentStructSize;
                        Stored_Buffer_Type = Type;
                    }
                    if (!Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.FirstEvent)) {
                        if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameSize))
                            currentStructSize -= 2;
                        if (Event.ListEvents.Track.Flags.HasFlag(EVE_Track.TrackFlags.SameType))
                            currentStructSize -= 2;
                    }
                    switch ((ushort)Type & 3) {
                        case 0:
                            if (Type.HasFlag(TypeFlags.RotationQuaternion)) {
                                Quaternion = s.SerializeObject<Jade_Quaternion>(Quaternion, name: nameof(Quaternion));
                            } else if (Type.HasFlag(TypeFlags.RotationMatrix)) {
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
                    if (Type.HasFlag(TypeFlags.Time)) {
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
            Translation0 = 1 << 0,
            Translation1 = 1 << 1,
            RotationMatrix = 1 << 2,
            Time = 1 << 3,
            RotationQuaternion = 1 << 4,
            BlockedForIK = 1 << 5,
            HasNextValue = 1 << 6,
            CompressedQuaternion = 1 << 7,
            UltraCompressedQuaternionX = 1 << 8,
            UltraCompressedQuaternionY = 1 << 9,
            UltraCompressedQuaternionZ = 1 << 10,
            CompressedAbsoluteTranslation = 1 << 11,
            CompressedRelativeTranslation = 1 << 12,
            UltraCompressedTranslationX = 1 << 13,
            UltraCompressedTranslationY = 1 << 14,
            UltraCompressedTranslationZ = 1 << 15,
        }
    }
}