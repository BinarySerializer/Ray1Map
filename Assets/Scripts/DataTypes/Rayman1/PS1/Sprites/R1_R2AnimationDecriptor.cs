using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Animation descriptor for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class R1_R2AnimationDecriptor : BinarySerializable, IR1_AnimationDescriptor
    {
        #region Animation Data

        /// <summary>
        /// Pointer to the animation layers
        /// </summary>
        public Pointer LayersPointer { get; set; }

        /// <summary>
        /// Pointer to the animation frames
        /// </summary>
        public Pointer FramesPointer { get; set; }

        // TODO: Parse the data from this pointer
        // Unknown - usually null
        public Pointer UnkAnimDataPointer { get; set; }

        /// <summary>
        /// The amount of layers per frame
        /// </summary>
        public ushort LayersPerFrame { get; set; }

        /// <summary>
        /// The amount of frames in the animation
        /// </summary>
        public byte FrameCount { get; set; }

        public byte UnkAnimDataCount { get; set; }

        #endregion

        #region Pointer Data

        /// <summary>
        /// The pointers to the layers
        /// </summary>
        public Pointer[] LayerPointers { get; set; }

        /// <summary>
        /// The animation layers
        /// </summary>
        public R1_AnimationLayer[][] Layers { get; set; }

        /// <summary>
        /// The animation frames
        /// </summary>
        public R1_AnimationFrame[] Frames { get; set; }

        public R1_R2UnknownAnimData[] UnkAnimData { get; set; }

        #endregion

        #region Interface Members

        /// <summary>
        /// The number of layers to use per frame
        /// </summary>
        byte IR1_AnimationDescriptor.LayersPerFrame => (byte)LayersPerFrame;

        /// <summary>
        /// The animation layers
        /// </summary>
        R1_AnimationLayer[] IR1_AnimationDescriptor.Layers => Layers.SelectMany(x => x).ToArray();

        #endregion

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize pointers
            LayersPointer = s.SerializePointer(LayersPointer, name: nameof(LayersPointer));
            FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));
            UnkAnimDataPointer = s.SerializePointer(UnkAnimDataPointer, name: nameof(UnkAnimDataPointer)); // ^points to 8 bytes

            // Serialize values
            LayersPerFrame = s.Serialize<ushort>(LayersPerFrame, name: nameof(LayersPerFrame));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            UnkAnimDataCount = s.Serialize<byte>(UnkAnimDataCount, name: nameof(UnkAnimDataCount));

            // Serialize layers
            s.DoAt(LayersPointer, () =>
            {
                // Serialize the layer pointers
                LayerPointers = s.SerializePointerArray(LayerPointers, FrameCount, name: nameof(LayerPointers));

                if (Layers == null)
                    Layers = new R1_AnimationLayer[FrameCount][];

                // Serialize the layers for each frame
                for (int i = 0; i < Layers.Length; i++)
                    Layers[i] = s.SerializeObjectArray<R1_AnimationLayer>(Layers[i], LayersPerFrame, name: $"{nameof(Layers)} [{i}]");
            });

            // Serialize frames
            s.DoAt(FramesPointer, () => Frames = s.SerializeObjectArray<R1_AnimationFrame>(Frames, FrameCount, name: nameof(Frames)));

            // Serialize unknown animation data
            s.DoAt(UnkAnimDataPointer, () => UnkAnimData = s.SerializeObjectArray<R1_R2UnknownAnimData>(UnkAnimData, UnkAnimDataCount, name: nameof(UnkAnimData)));
        }
    }
}