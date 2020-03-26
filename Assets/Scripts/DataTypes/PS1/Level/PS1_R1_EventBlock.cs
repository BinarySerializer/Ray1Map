using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Event block data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_EventBlock : R1Serializable
    {
        /// <summary>
        /// Pointer to the events
        /// </summary>
        public Pointer EventsPointer { get; set; }

        /// <summary>
        /// The amount of events in the file
        /// </summary>
        public uint EventCount { get; set; }

        /// <summary>
        /// Pointer to the event links
        /// </summary>
        public Pointer EventLinksPointer { get; set; }

        /// <summary>
        /// The amount of event links in the file
        /// </summary>
        public uint EventLinkCount { get; set; }

        /// <summary>
        /// The events
        /// </summary>
        public PS1_R1_Event[] Events { get; set; }

        /// <summary>
        /// Data table for event linking
        /// </summary>
        public byte[] EventLinkingTable { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            EventsPointer = s.SerializePointer(EventsPointer, name: nameof(EventsPointer));
            EventCount = s.Serialize<uint>(EventCount, name: nameof(EventCount));
            EventLinksPointer = s.SerializePointer(EventLinksPointer, name: nameof(EventLinksPointer));
            EventLinkCount = s.Serialize<uint>(EventLinkCount, name: nameof(EventLinkCount));

            if (EventCount != EventLinkCount)
                Debug.LogError("Event counts don't match");

            s.DoAt(EventsPointer, (() =>
            {
                // Serialize every event
                Events = s.SerializeObjectArray<PS1_R1_Event>(Events, EventCount, name: nameof(Events));
            }));

            s.DoAt(EventLinksPointer, (() =>
            {
                // Serialize the event linking table
                EventLinkingTable = s.SerializeArray<byte>(EventLinkingTable, EventLinkCount, name: nameof(EventLinkingTable));
            }));
        }
    }
}