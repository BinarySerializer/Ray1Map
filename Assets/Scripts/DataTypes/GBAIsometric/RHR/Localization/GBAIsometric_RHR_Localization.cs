using System.Text;
using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_RHR_Localization : BinarySerializable
    {
        public Pointer<GBAIsometric_RHR_Cutscene>[] Cutscenes { get; set; }
        public Pointer<Array<ushort>>[] Offsets { get; set; }
        public Pointer[] LocTables { get; set; }
        public string[][] Localization { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Cutscenes = s.SerializePointerArray<GBAIsometric_RHR_Cutscene>(Cutscenes, 194, name: nameof(Cutscenes));
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

                s.Context.StoreObject("Loc", Localization);
            }

            foreach (var c in Cutscenes)
                c.Resolve(s);
        }
    }
}