using System.Linq;
using System.Text;

namespace R1Engine
{
    public class GBAIsometric_LocalizationTable : R1Serializable
    {
        public Pointer<Array<ushort>>[] Offsets { get; set; }
        public Pointer[] LocTables { get; set; }
        public string[][] Localization { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Offsets = s.SerializePointerArray<Array<ushort>>(Offsets, 6, resolve: true, onPreSerialize: x => x.Length = 690, name: nameof(Offsets));
            LocTables = s.SerializePointerArray(LocTables, 6, name: nameof(LocTables));
            if (Localization == null) {
                Localization = new string[Offsets.Length][];
                for (int i = 0; i < Localization.Length; i++) {
                    Localization[i] = new string[Offsets[i].Value.Length];
                    s.DoAt(LocTables[i], () => {
                        s.DoEncoded(new RHREncoder(), () => {
                            Pointer basePtr = s.CurrentPointer;
                            for (int j = 0; j < Offsets[i].Value.Length; j++) {
                                s.DoAt(basePtr + Offsets[i].Value.Value[j], () => {
                                    Localization[i][j] = s.SerializeString(Localization[i][j], encoding: Encoding.GetEncoding(1252), name: $"{nameof(Localization)}[{i}][{j}]");
                                });
                            }

                            // Go to end
                            s.Goto(s.CurrentPointer + s.CurrentLength);
                        });
                    });
                }

                s.Context.StoreObject("Loc", Localization?.ElementAtOrDefault(0));
            }
        }
    }
}