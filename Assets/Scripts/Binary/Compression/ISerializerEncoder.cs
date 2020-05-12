using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Encodes/decodes serializer data
    /// </summary>
    public interface ISerializerEncoder
    {
        /// <summary>
        /// Decodes the data and returns it in a stream
        /// </summary>
        /// <param name="s">The serializer object</param>
        /// <returns>The stream with the decoded data</returns>
        Stream Decode(SerializerObject s);

        // TODO: Support encoding
    }
}