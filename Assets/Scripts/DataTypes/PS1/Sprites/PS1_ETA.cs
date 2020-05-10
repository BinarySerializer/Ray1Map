using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// ETA data for PS1
    /// </summary>
    public class PS1_ETA : R1Serializable
    {
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
        /// Collection of states and substates for the event
        /// </summary>
        public Common_EventState[][] EventStates { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get number of ETAs, hack
            if (s is BinaryDeserializer)
            {
                // Save the current pointer
                var pointer = s.CurrentPointer;

                // Get the number of etats
                s.DoAt(pointer, () => {
                    Pointer p = s.SerializePointer(null, name: "FirstEtat");
                    
                    if (p.file != pointer.file || p.AbsoluteOffset < pointer.AbsoluteOffset + 4 || (p.AbsoluteOffset - pointer.AbsoluteOffset) % 4 != 0)
                        Debug.LogWarning("Number of ETAs wasn't correctly determined");

                    NumEtats = (p.AbsoluteOffset - pointer.AbsoluteOffset) / 4;
                });
            }

            // Serialize the Etat pointers
            EtatPointers = s.SerializePointerArray(EtatPointers, NumEtats, name: nameof(EtatPointers));
            
            // Get number of SubEtats for each Etat
            if (NumSubEtats == null)
            {
                // Get the state size
                uint stateSize;

                if (s.GameSettings.EngineVersion == EngineVersion.Ray2PS1)
                    stateSize = 16u;
                else if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3)
                    stateSize = 14u;
                else if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol6)
                    stateSize = 12u;
                else
                    stateSize = 8;

                NumSubEtats = new uint[NumEtats];
                
                // Enumerate every Etat, except last one
                for (int i = 0; i < EtatPointers.Length - 1; i++)
                {
                    // Make sure we have a valid pointer
                    if (EtatPointers[i] == null) 
                        continue;
                    
                    // Get size and make sure the next one is not null
                    if (EtatPointers[i + 1] != null)
                        NumSubEtats[i] = (EtatPointers[i + 1].AbsoluteOffset - EtatPointers[i].AbsoluteOffset) / stateSize;
                    else
                        Debug.LogWarning("An Etat Pointer was null - Number of SubEtats couldn't be determined");
                }

                // Get the size of the last Etat
                if (EtatPointers[NumEtats - 1] != null)
                {
                    // TODO: Find better way to parse this
                    
                    s.DoAt(s.CurrentPointer, () => {

                        uint count = 0;
                        const int maxCount = 20;

                        while (true)
                        {
                            // Make sure we can read more
                            if (s.CurrentLength - stateSize < s.CurrentPointer.FileOffset)
                                break;

                            // Read the next bytes
                            var bytes = s.SerializeArray<byte>(null, stateSize, name: $"Dummy state {count}");

                            // Check if it's invalid (is a pointer)
                            if (bytes[3] == 0x80)
                                break;

                            // Make sure we haven't reached the max
                            if (count > maxCount)
                                break;

                            count++;
                        }

                        NumSubEtats[NumEtats - 1] = count;
                    });
                    
                    /*
                    // Temp fix so we don't read past the end of the stream - this however causes certain events to get the wrong state!
                    uint maxEtats = (s.CurrentLength - EtatPointers[NumEtats - 1].FileOffset) / stateSize;
                    NumSubEtats[NumEtats - 1] = System.Math.Min(20u, maxEtats);
                    */
                }
            }

            // Create state array
            if (EventStates == null)
                EventStates = new Common_EventState[NumEtats][];

            // Serialize the states
            for (int i = 0; i < EtatPointers.Length; i++)
                s.DoAt(EtatPointers[i], () => EventStates[i] = s.SerializeObjectArray<Common_EventState>(EventStates[i], NumSubEtats[i], name: nameof(EventStates) + "[" + i + "]"));
        }
    }
}