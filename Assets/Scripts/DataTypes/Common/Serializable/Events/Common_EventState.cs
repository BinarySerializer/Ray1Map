namespace R1Engine
{
    /// <summary>
    /// Common event state (etat) data
    /// </summary>
    public class Common_EventState : R1Serializable
    {
        /// <summary>
        /// The right speed
        /// </summary>
        public sbyte RightSpeed { get; set; }
        
        /// <summary>
        /// The left speed
        /// </summary>
        public sbyte LeftSpeed { get; set; }
        
        /// <summary>
        /// The animation index
        /// </summary>
        public byte AnimationIndex { get; set; }
        
        /// <summary>
        /// The etat value
        /// </summary>
        public byte Etat { get; set; }
        
        /// <summary>
        /// The sub-etat value
        /// </summary>
        public byte SubEtat { get; set; }
        
        /// <summary>
        /// The amount of frames to skip in the animation each second, or 0 for it to not animate
        /// </summary>
        public byte AnimationSpeed { get; set; }
        
        /// <summary>
        /// The sound index
        /// </summary>
        public byte SoundIndex { get; set; }
        
        /// <summary>
        /// The interaction type
        /// </summary>
        public byte InteractionType { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            RightSpeed = s.Serialize<sbyte>(RightSpeed, name: nameof(RightSpeed));
            LeftSpeed = s.Serialize<sbyte>(LeftSpeed, name: nameof(LeftSpeed));
            AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));
            Etat = s.Serialize<byte>(Etat, name: nameof(Etat));
            SubEtat = s.Serialize<byte>(SubEtat, name: nameof(SubEtat));
            AnimationSpeed = s.Serialize<byte>(AnimationSpeed, name: nameof(AnimationSpeed));
            SoundIndex = s.Serialize<byte>(SoundIndex, name: nameof(SoundIndex));
            InteractionType = s.Serialize<byte>(InteractionType, name: nameof(InteractionType));
        }
    }
}