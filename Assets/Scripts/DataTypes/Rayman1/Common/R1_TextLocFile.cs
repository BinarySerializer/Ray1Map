using System.Collections.Generic;

namespace R1Engine
{
    public class R1_TextLocFile : R1TextSerializable
    {
        public string[] Strings { get; set; }

        public override void Read(R1TextParser parser)
        {
            var tempStrings = new List<string>();

            string value;

            // Read values into a temporary list
            while ((value = parser.ReadValue(true)) != null)
                tempStrings.Add(value);

            // Set strings
            Strings = tempStrings.ToArray();
        }
    }
}