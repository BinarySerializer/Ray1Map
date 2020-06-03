namespace R1Engine
{
    /// <summary>
    /// ROM data for Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_ROM : R1Serializable
    {
        /// <summary>
        /// The map data for the current level
        /// </summary>
        public PS1_R1_MapBlock MapData { get; set; }

        /// <summary>
        /// The current sprite palette
        /// </summary>
        public RGB556Color[] SpritePalette { get; set; }

        public byte[] ImageBuffer { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // TODO: Don't hard-code this!
            var mapPointer = this.Offset + 3175788;
            var palPointer = new Pointer(0x009B8C6A, this.Offset.file);
            var bufferPointer = this.Offset + 486418;

            // Serialize map data
            s.DoAt(mapPointer, () => s.DoEncoded(new RNCEncoder(), () => MapData = s.SerializeObject<PS1_R1_MapBlock>(MapData, name: nameof(MapData))));
            
            // Serialize sprite palette
            s.DoAt(palPointer, () => SpritePalette = s.SerializeObjectArray<RGB556Color>(SpritePalette, 256, name: nameof(SpritePalette)));

            s.DoAt(bufferPointer, () => s.DoEncoded(new RNCEncoder(), () => ImageBuffer = s.SerializeArray<byte>(ImageBuffer, s.CurrentLength, name: nameof(ImageBuffer))));

            /*
            var output = new byte[(ImageBuffer.Length / 2) * 3];

            for (int i = 0; i < ImageBuffer.Length; i += 2)
            {
                var v = SpritePalette[ImageBuffer[i]];

                // Write RGB values
                output[(i / 2) * 3 + 0] = v.Red;
                output[(i / 2) * 3 + 1] = v.Green;
                output[(i / 2) * 3 + 2] = v.Blue;
            }

            Util.ByteArrayToFile(@"C:\Users\RayCarrot\Downloads\test.dat", output);*/
        }
    }
}