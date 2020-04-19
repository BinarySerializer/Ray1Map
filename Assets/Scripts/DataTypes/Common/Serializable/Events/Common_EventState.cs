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

        public byte UnkDemo1 { get; set; }
        
        /// <summary>
        /// The left speed
        /// </summary>
        public sbyte LeftSpeed { get; set; }

        public byte UnkDemo2 { get; set; }

        /// <summary>
        /// The animation index
        /// </summary>
        public byte AnimationIndex { get; set; }

        public byte UnkDemo3 { get; set; }

        /// <summary>
        /// The etat value of the linked state
        /// </summary>
        public byte LinkedEtat { get; set; }
        
        /// <summary>
        /// The sub-etat value of the linked state
        /// </summary>
        public byte LinkedSubEtat { get; set; }

        public byte UnkDemo4 { get; set; }

        /// <summary>
        /// The amount of frames to skip in the animation each second, or 0 for it to not animate
        /// </summary>
        public byte AnimationSpeed { get; set; }

        public byte UnkDemo5 { get; set; }
        public byte UnkDemo6 { get; set; }
        public byte UnkDemo7 { get; set; }
        public byte UnkDemo8 { get; set; }

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

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3)
                UnkDemo1 = s.Serialize<byte>(UnkDemo1, name: nameof(UnkDemo1));

            LeftSpeed = s.Serialize<sbyte>(LeftSpeed, name: nameof(LeftSpeed));

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3)
                UnkDemo2 = s.Serialize<byte>(UnkDemo2, name: nameof(UnkDemo2));

            AnimationIndex = s.Serialize<byte>(AnimationIndex, name: nameof(AnimationIndex));

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3)
                UnkDemo3 = s.Serialize<byte>(UnkDemo3, name: nameof(UnkDemo3));

            LinkedEtat = s.Serialize<byte>(LinkedEtat, name: nameof(LinkedEtat));
            LinkedSubEtat = s.Serialize<byte>(LinkedSubEtat, name: nameof(LinkedSubEtat));

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3)
                UnkDemo4 = s.Serialize<byte>(UnkDemo4, name: nameof(UnkDemo4));

            AnimationSpeed = s.Serialize<byte>(AnimationSpeed, name: nameof(AnimationSpeed));

            if (s.GameSettings.EngineVersion == EngineVersion.RayPS1JPDemoVol3)
            {
                UnkDemo5 = s.Serialize<byte>(UnkDemo5, name: nameof(UnkDemo5));
                UnkDemo6 = s.Serialize<byte>(UnkDemo6, name: nameof(UnkDemo6));
                UnkDemo7 = s.Serialize<byte>(UnkDemo7, name: nameof(UnkDemo7));
                UnkDemo8 = s.Serialize<byte>(UnkDemo8, name: nameof(UnkDemo8));
            }
            else
            {
                SoundIndex = s.Serialize<byte>(SoundIndex, name: nameof(SoundIndex));
                InteractionType = s.Serialize<byte>(InteractionType, name: nameof(InteractionType));
            }
        }
    }
}