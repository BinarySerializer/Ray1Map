using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Animation group data for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class R1_R2EventAnimGroup : BinarySerializable
    {
        #region Event Data

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

        #endregion

        #region Parsed from Pointers

        /// <summary>
        /// The animation descriptors
        /// </summary>
        public R1_R2AnimationDecriptor[] AnimationDecriptors { get; set; }

        /// <summary>
        /// The event ETA
        /// </summary>
        public R1_PS1_ETA ETA { get; set; }

        #endregion

        #region Methods

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
                s.DoAt(AnimationDescriptorsPointer, () => AnimationDecriptors = s.SerializeObjectArray<R1_R2AnimationDecriptor>(AnimationDecriptors, AnimationDescriptorCount, name: nameof(AnimationDecriptors)));

            // Serialize ETA
            if (ETAPointer != null)
                s.DoAt(ETAPointer, () => ETA = s.SerializeObject<R1_PS1_ETA>(ETA, name: nameof(ETA)));
        }

        #endregion
    }
}