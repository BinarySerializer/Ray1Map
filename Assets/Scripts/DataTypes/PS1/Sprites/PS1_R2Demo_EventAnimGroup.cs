using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Animation group data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_EventAnimGroup : R1Serializable
    {
        /// <summary>
        /// The ETA pointer
        /// </summary>
        public Pointer ETAPointer { get; set; }

        /// <summary>
        /// The animation descriptors pointer
        /// </summary>
        public Pointer AnimationDescriptorsPointer { get; set; }

        /// <summary>
        /// The animation descriptor count
        /// </summary>
        public ushort AnimationDescriptorCount { get; set; }

        // Usually 0
        public ushort Unknown { get; set; }


        /// <summary>
        /// The animation descriptors
        /// </summary>
        public PS1_R2Demo_AnimationDecriptor[] AnimationDecriptors { get; set; }

        /// <summary>
        /// The number of Etats
        /// </summary>
        public uint NumEtats { get; set; }

        /// <summary>
        /// The numbers of SubEtats
        /// </summary>
        public uint[] NumSubEtats { get; set; }

        /// <summary>
        /// Pointers to the ETA descriptors
        /// </summary>
        public Pointer[] EtatPointers { get; set; }

        /// <summary>
        /// Collection of states and substates
        /// </summary>
        public Common_EventState[][] EventStates { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the pointers
            ETAPointer = s.SerializePointer(ETAPointer, name: nameof(ETAPointer));
            AnimationDescriptorsPointer = s.SerializePointer(AnimationDescriptorsPointer, name: nameof(AnimationDescriptorsPointer));

            // Serialize the values
            AnimationDescriptorCount = s.Serialize<ushort>(AnimationDescriptorCount, name: nameof(AnimationDescriptorCount));
            Unknown = s.Serialize<ushort>(Unknown, name: nameof(Unknown));

            // Serialize the animation descriptors
            if (AnimationDescriptorsPointer != null)
                s.DoAt(AnimationDescriptorsPointer, () => AnimationDecriptors = s.SerializeObjectArray<PS1_R2Demo_AnimationDecriptor>(AnimationDecriptors, AnimationDescriptorCount, name: nameof(AnimationDecriptors)));

            // Serialize the event states (hack)
            // Get number of ETAs, hack
            if (s is BinaryDeserializer)
            {
                s.DoAt(ETAPointer, () => {
                    Pointer p = s.SerializePointer(null, name: "FirstEtat");
                    if (p.file != ETAPointer.file
                        || p.AbsoluteOffset < ETAPointer.AbsoluteOffset + 4
                        || (p.AbsoluteOffset - ETAPointer.AbsoluteOffset) % 4 != 0)
                    {
                        Debug.LogWarning("Number of ETAs wasn't correctly determined");
                    }
                    NumEtats = (p.AbsoluteOffset - ETAPointer.AbsoluteOffset) / 4;
                });
            }
            s.DoAt(ETAPointer, () => {
                EtatPointers = s.SerializePointerArray(EtatPointers, NumEtats, name: nameof(EtatPointers));
                if (NumSubEtats == null)
                {
                    // Get number of subetats, hack
                    NumSubEtats = new uint[NumEtats];
                    for (int i = 0; i < EtatPointers.Length - 1; i++)
                    {
                        if (EtatPointers[i] != null)
                        {
                            if (EtatPointers[i + 1] != null)
                            {
                                NumSubEtats[i] = (EtatPointers[i + 1].AbsoluteOffset - EtatPointers[i].AbsoluteOffset) / (16);
                            }
                            else
                            {
                                Debug.LogWarning("An Etat Pointer was null - Number of SubEtats couldn't be determined");
                            }
                        }
                    }
                    if (EtatPointers[NumEtats - 1] != null)
                    {
                        // TODO: Parse this last array
                        NumSubEtats[NumEtats - 1] = 10;
                    }
                }
                if (EventStates == null)
                {
                    EventStates = new Common_EventState[NumEtats][];
                }
                for (int i = 0; i < EtatPointers.Length; i++)
                {
                    s.DoAt(EtatPointers[i], () => {
                        EventStates[i] = s.SerializeObjectArray<Common_EventState>(EventStates[i], NumSubEtats[i], name: nameof(EventStates) + "[" + i + "]");
                    });
                }
            });
        }
    }
}