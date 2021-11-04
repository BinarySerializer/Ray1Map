using System;
using System.IO;
using System.Linq;
using BinarySerializer.PS1;
using UnityEngine;

namespace Ray1Map
{
    public class PS1VRAMTexture
    {
        public PS1VRAMTexture(PS1_TSB tsb, PS1_CBA cba, PS1_TMD_UV[] uvs)
        {
            TSB = tsb;
            CBA = cba;

            int xMin = uvs.Min(x => x.U);
            int xMax = uvs.Max(x => x.U) + 1;
            int yMin = uvs.Min(x => x.V);
            int yMax = uvs.Max(x => x.V) + 1;
            int w = xMax - xMin;
            int h = yMax - yMin;

            Bounds = new RectInt(xMin, yMin, w, h);

            bool is8bit = TSB.TP == PS1_TSB.TexturePageTP.CLUT_8Bit;
            TextureRegion = new RectInt(TSB.TX * PS1_VRAM.PageWidth + Bounds.x / (is8bit ? 1 : 2), TSB.TY * PS1_VRAM.PageHeight + Bounds.y, Bounds.width / (is8bit ? 1 : 2), Bounds.height);

            int palLength = (is8bit ? 256 : 16) * 2;
            PaletteRegion = new RectInt(CBA.ClutX * 2 * 16, CBA.ClutY, palLength, 1);
        }

        public PS1_TSB TSB { get; }
        public PS1_CBA CBA { get; }
        public RectInt Bounds { get; set; }
        public RectInt TextureRegion { get; }
        public RectInt PaletteRegion { get; }
        public Texture2D Texture { get; protected set; }
        public PS1VRAMAnimatedTexture AnimatedTexture { get; protected set; }

        public bool HasOverlap(PS1VRAMTexture b)
        {
            if (b.TSB.TP != TSB.TP ||
                b.TSB.TX != TSB.TX ||
                b.TSB.TY != TSB.TY ||
                b.CBA.ClutX != CBA.ClutX ||
                b.CBA.ClutY != CBA.ClutY)
                return false;

            return Bounds.Overlaps(b.Bounds);
        }

        public Texture2D GetTexture(PS1_VRAM vram, Texture2D tex = null)
        {
            PS1_TIM.TIM_ColorFormat colFormat = TSB.TP switch
            {
                PS1_TSB.TexturePageTP.CLUT_4Bit => PS1_TIM.TIM_ColorFormat.BPP_4,
                PS1_TSB.TexturePageTP.CLUT_8Bit => PS1_TIM.TIM_ColorFormat.BPP_8,
                PS1_TSB.TexturePageTP.Direct_15Bit => PS1_TIM.TIM_ColorFormat.BPP_16,
                _ => throw new InvalidDataException($"PS1 TSB TexturePageTP was {TSB.TP}")
            };

            tex ??= TextureHelpers.CreateTexture2D(Bounds.width, Bounds.height, clear: true);
            tex.wrapMode = TextureWrapMode.Repeat;

            vram.FillTexture(
                tex: tex,
                width: Bounds.width,
                height: Bounds.height,
                colorFormat: colFormat,
                texX: 0,
                texY: 0,
                clutX: CBA.ClutX * 16,
                clutY: CBA.ClutY,
                texturePageX: TSB.TX,
                texturePageY: TSB.TY,
                texturePageOffsetX: Bounds.x,
                texturePageOffsetY: Bounds.y,
                flipY: true);

            tex.Apply();

            return tex;
        }

        public void SetTexture(Texture2D tex)
        {
            Texture = tex;
        }

        public void SetAnimatedTexture(PS1VRAMAnimatedTexture tex)
        {
            AnimatedTexture = tex;
        }

        public void ExpandWithBounds(PS1VRAMTexture b)
        {
            var minX = Math.Min(Bounds.x, b.Bounds.x);
            var minY = Math.Min(Bounds.y, b.Bounds.y);
            var maxX = Math.Max(Bounds.x + Bounds.width, b.Bounds.x + b.Bounds.width);
            var maxY = Math.Max(Bounds.y + Bounds.height, b.Bounds.y + b.Bounds.height);
            Bounds = new RectInt(
                minX,
                minY,
                maxX - minX,
                maxY - minY);
        }
    }
}