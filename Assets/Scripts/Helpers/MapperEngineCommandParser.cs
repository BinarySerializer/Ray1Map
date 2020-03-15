using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Parser for Mapper engine command data
    /// </summary>
    public class MapperEngineCommandParser : IDisposable
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="stream">The data stream</param>
        public MapperEngineCommandParser(Stream stream)
        {
            Stream = stream;
            Reader = new StreamReader(stream, Settings.StringEncoding, false, 1024, true);
            PreviousIfCommands = new List<string>();
            LineBuffer = new List<string>();
        }

        /// <summary>
        /// The last read if commands
        /// </summary>
        public List<string> PreviousIfCommands { get; set; }

        /// <summary>
        /// The data stream
        /// </summary>
        protected Stream Stream { get; }

        /// <summary>
        /// The stream reader to use
        /// </summary>
        protected StreamReader Reader { get; }

        /// <summary>
        /// Flag for if the end of the file has been reached
        /// </summary>
        protected bool Finished { get; set; }

        /// <summary>
        /// The line value buffer
        /// </summary>
        protected List<string> LineBuffer { get; set; }

        /// <summary>
        /// Gets the next line
        /// </summary>
        /// <returns>The line</returns>
        protected string GetNewLine()
        {
            bool isInComment = false;

            // Loop until we find a valid line
            while (!Reader.EndOfStream)
            {
                // Read the line
                var line = Reader.ReadLine();

                // Ignore if empty
                if (line == null || String.IsNullOrWhiteSpace(line))
                    continue;

                string processedLine = String.Empty;

                // Process every character in the line
                foreach (var c in line)
                {
                    // Check if it's the comment separator
                    if (c == '/')
                        isInComment ^= true;

                    // If not in a comment, add the character
                    else if (!isInComment)
                        processedLine += c;
                }

                // Return if not empty
                if (!String.IsNullOrWhiteSpace(processedLine))
                    return processedLine;
            }

            // Throw if end of file was reached
            throw new Exception("The end of the file has been reached");
        }

        /// <summary>
        /// Gets the next value, or null if reached the end
        /// </summary>
        /// <returns>The next value</returns>
        public string NextValue()
        {
            // Keep track if within certain scopes
            bool isInIf = false;

            // Make sure we haven't reached the end
            if (Reader.EndOfStream || Finished)
                return null;

            // The value to return
            string value;

            // Indicates if it has to loop again
            bool redo = false;

            do
            {
                // If this was a redo of the loop, set to false to finish
                if (redo)
                    redo = false;

                // If buffer is empty, read a new line
                if (!LineBuffer.Any())
                    // Split it up and trim
                    LineBuffer = GetNewLine().Split(',').Select(x => x.Trim()).Where(x => !String.IsNullOrWhiteSpace(x)).ToList();

                // Get the value
                value = LineBuffer.First();

                // Remove the retrieved value
                LineBuffer.RemoveAt(0);

                // If the if command is used we are entering an engine if scope
                if (value == "£if")
                {
                    // Clear the buffer
                    PreviousIfCommands.Clear();

                    // Indicate that we are inside of an if command
                    isInIf = true;
                }
                // Check if we're exiting an if command
                else if (value == "£endif")
                {
                    // Indicate that the if command has ended
                    isInIf = false;

                    // Indicate that we need to loop again to get the actual value
                    redo = true;
                }
                // If inside of an if command, add to the buffer
                else if (isInIf)
                {
                    PreviousIfCommands.Add(value);
                }
            } while (isInIf || redo);

            if (value == "*")
            {
                Finished = true;
                return null;
            }

            // Return the value
            return value;
        }

        /// <summary>
        /// Disposes the parser
        /// </summary>
        public void Dispose()
        {
            Reader?.Dispose();
        }
    }
}