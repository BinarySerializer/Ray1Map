using System;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_LevelDataLayerDataPointers : R1Serializable
    {
        public Pointer<GBAIsometric_GraphicsData> GraphicsDataPointer { get; set; }
        public Pointer Pointer1 { get; set; } // pointer, pointer, pointer
        public Pointer Pointer2 { get; set; } // ushort
        public Pointer Pointer3 { get; set; } // ushorts

        public Pointer Pointer4 { get; set; } // 35320 bytes?
        public Pointer Pointer5 { get; set; } // Compressed

        public Pointer Pointer6 { get; set; }
        public Pointer Pointer7 { get; set; } // Compressed

        public Pointer Pointer8 { get; set; }
        public Pointer Pointer9 { get; set; } // Compressed

        public Pointer Pointer10 { get; set; }
        public Pointer Pointer11 { get; set; } // Compressed

        public Pointer AnimatedPalettesPointer { get; set; }

        // Parsed
        public ushort[] Pointer5_Data { get; set; }
        public ushort[] Pointer7_Data { get; set; }
        public ushort[] Pointer9_Data { get; set; }
        public ushort[] Pointer11_Data { get; set; }
        public ARGB1555Color[] AnimatedPalettes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            Pointer1 = s.SerializePointer(Pointer1, name: nameof(Pointer1));
            Pointer2 = s.SerializePointer(Pointer2, name: nameof(Pointer2));
            Pointer3 = s.SerializePointer(Pointer3, name: nameof(Pointer3));
            Pointer4 = s.SerializePointer(Pointer4, name: nameof(Pointer4));
            Pointer5 = s.SerializePointer(Pointer5, name: nameof(Pointer5));
            Pointer6 = s.SerializePointer(Pointer6, name: nameof(Pointer6));
            Pointer7 = s.SerializePointer(Pointer7, name: nameof(Pointer7));
            Pointer8 = s.SerializePointer(Pointer8, name: nameof(Pointer8));
            Pointer9 = s.SerializePointer(Pointer9, name: nameof(Pointer9));
            Pointer10 = s.SerializePointer(Pointer10, name: nameof(Pointer10));
            Pointer11 = s.SerializePointer(Pointer11, name: nameof(Pointer11));
            AnimatedPalettesPointer = s.SerializePointer(AnimatedPalettesPointer, name: nameof(AnimatedPalettesPointer));

            // TODO: Remove try/catch
            try
            {
                s.DoAt(Pointer5, () =>
                {
                    s.DoEncoded(new RHREncoder(), () =>
                    {
                        Pointer5_Data = s.SerializeArray<ushort>(Pointer5_Data, s.CurrentLength / 2, name: nameof(Pointer5_Data));
                        s.Log($"Max: {Pointer5_Data.Max()}");
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to decompress {Pointer5}: {ex.Message}\n{ex.InnerException?.StackTrace}");
            }
            try
            {
                s.DoAt(Pointer7, () =>
                {
                    s.DoEncoded(new RHREncoder(), () =>
                    {
                        Pointer7_Data = s.SerializeArray<ushort>(Pointer7_Data, s.CurrentLength / 2, name: nameof(Pointer7_Data));
                        s.Log($"Max: {Pointer7_Data.Max()}");
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to decompress: {ex.Message}");
            }
            try
            {
                s.DoAt(Pointer9, () =>
                {
                    s.DoEncoded(new RHREncoder(), () =>
                    {
                        Pointer9_Data = s.SerializeArray<ushort>(Pointer9_Data, s.CurrentLength / 2, name: nameof(Pointer9_Data));
                        s.Log($"Max: {Pointer9_Data.Max()}");
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to decompress: {ex.Message}");
            }
            try
            {
                s.DoAt(Pointer11, () =>
                {
                    s.DoEncoded(new RHREncoder(), () =>
                    {
                        Pointer11_Data = s.SerializeArray<ushort>(Pointer11_Data, s.CurrentLength / 2, name: nameof(Pointer11_Data));
                        s.Log($"Max: {Pointer11_Data.Max()}");
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to decompress: {ex.Message}");
            }

            //AnimatedPalettes = s.DoAt(PalettesPointer, () => s.SerializeObjectArray<ARGB1555Color>(AnimatedPalettes, 16 * 45, name: nameof(AnimatedPalettes)));
        }
    }
}