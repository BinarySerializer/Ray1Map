using System;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Event graphics block for some special events in Rayman 1 (Jaguar)
    /// </summary>
    public class Jaguar_R1_EventComplexData : R1Serializable
    {
        public ushort StructType { get; set; } // Read from EventDefinition
		public ushort NumLayers { get; set; }

        public byte[] UnkBytes { get; set; }
        public Pointer ImageDescriptorsPointer { get; set; }
        public Jaguar_R1_EventComplexDataTransition[] Transitions { get; set; }
		public Jaguar_R1_EventComplexDataState[] States { get; set; }

		// Parsed
		public Common_ImageDescriptor[] ImageDescriptors { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            if (StructType != 29) {
                UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x10, name: nameof(UnkBytes));
            }
            ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
            if (StructType != 29) {
                Transitions = s.SerializeObjectArray<Jaguar_R1_EventComplexDataTransition>(Transitions, 7, onPreSerialize: g => {
					g.StructType = StructType;
					g.NumLayers = NumLayers;
				}, name: nameof(Transitions));
            }

			// Serialize from first state index
			{
				var temp = new List<Jaguar_R1_EventComplexDataState>();

				var index = 0;
				while (true) {
					// Always check for pointer
					{
						Pointer CheckPtr0 = null;
						bool success = true;
						s.DoAt(s.CurrentPointer, () => {
							try {
								CheckPtr0 = s.SerializePointer(CheckPtr0, name: nameof(CheckPtr0));
							} catch (Exception) {
								success = false;
							}
						});
						if (!success
						|| (CheckPtr0 != null && CheckPtr0.file != Offset.file)) {
							break;
						} else if(CheckPtr0 != null) {
							// Can't check animation header, the frame pointer doesn't always point to the start of the actual animation
							/*byte[] CheckBytes = null;
							s.DoAt(CheckPtr0 - 4, () => {
								CheckBytes = s.SerializeArray<byte>(CheckBytes, 4, name: nameof(CheckBytes));
								if (CheckBytes[1] != 0 || CheckBytes[3] != 0
								|| CheckBytes[0] == 0 || CheckBytes[2] == 0) {
									// Padding should be padding, other values should be filled in
									success = false;
								}
							});*/
							if (!success) break;
						}
					}
					var i = s.SerializeObject<Jaguar_R1_EventComplexDataState>(default, onPreSerialize: state => state.LayersPerFrame = NumLayers, name: $"{nameof(States)}[{index}]");

					temp.Add(i);

					index++;
				}

				States = temp.ToArray();
			}
			s.DoAt(ImageDescriptorsPointer, () => {
				// TODO: This doesn't seem to work consistently at all - fallback to previous method for now
				if (States != null && States.Length > 0) {
					int maxImageIndex = States
						.Where(x => x?.Layers != null)
						.SelectMany(x => x.Layers)
						.Max(x => /*UShort_12 == 5 ? BitHelpers.ExtractBits(x.ImageIndex, 7, 0) :*/ x.ImageIndex);
					ImageDescriptors = s.SerializeObjectArray<Common_ImageDescriptor>(ImageDescriptors, maxImageIndex + 1, name: nameof(ImageDescriptors));
					//Debug.Log(ImageDescriptors.Length);
				} else {
					var temp = new List<Common_ImageDescriptor>();

					var index = 0;
					while (true) {
						var i = s.SerializeObject<Common_ImageDescriptor>(default, name: $"{nameof(ImageDescriptors)}[{index}]");

						if (temp.Any() && i.Index != 0xFF && i.ImageBufferOffset < temp.Last().ImageBufferOffset)
							break;

						temp.Add(i);

						index++;
					}

					ImageDescriptors = temp.ToArray();
				}
			});
		}
    }
}