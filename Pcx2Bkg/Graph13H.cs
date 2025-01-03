using System.Runtime.InteropServices;

namespace Pcx2Bkg
{
    public static class Graphics
    {
        // Constants
        public const int MinX = 0;
        public const int MaxX = 319;
        public const int MinY = 0;
        public const int MaxY = 199;

        // Types
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Palette
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[,] Colors; // [256,3]
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vector
        {
            public int X;
            public int Y;
            public int Z;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct VectorArray
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public Vector[] Vectors;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RawImage
        {
            public ushort Width;
            public ushort Height;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 65531)]
            public byte[] Data;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ScreenBuffer
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200 * 320)]
            public byte[,] Buffer;
        }

        public static ScreenBuffer ScrBuf;

        // Procedures
        public static void InitGraph13h() => throw new NotImplementedException();
        public static void CloseGraph13h() => throw new NotImplementedException();
        public static void AllocRawImage(ref RawImage img, ushort width, ushort height) => throw new NotImplementedException();
        public static void FreeRawImage(ref RawImage img) => throw new NotImplementedException();
        public static void DisplayRawImage(int x, int y, ref RawImage img) => throw new NotImplementedException();
        public static void DisplayRawImageClip(int x, int y, ref RawImage img) => throw new NotImplementedException();
        public static void DisplayRawImageStretch(int x1, int y1, int x2, int y2, ref RawImage img) => throw new NotImplementedException();
        public static void CutRawImage(ref RawImage src, int x1, int y1, int x2, int y2, ref RawImage dest) => throw new NotImplementedException();
        public static void AllocCutRawImage(ref RawImage src, int x1, int y1, int x2, int y2, ref RawImage dest) => throw new NotImplementedException();
        public static void MirrorRawImage(ref RawImage src, ref RawImage dest) => throw new NotImplementedException();
        public static void AllocMirrorRawImage(ref RawImage src, ref RawImage dest) => throw new NotImplementedException();
        public static void TexturizeBackground(int x, int y, ref RawImage img) => throw new NotImplementedException();
        public static void SliceRawImage(ref RawImage src, int xOfs, int yOfs, ushort tileW, ushort tileH, ushort countX, ushort countY, byte xSpacing, byte ySpacing, ushort firstTile, ushort numTiles, out RawImage[] dest) => throw new NotImplementedException();
        public static void PutPal(ref Palette pal) => throw new NotImplementedException();
        public static void ReIndexColors(ref RawImage img, ref Palette pal, byte colors, byte newIndex) => throw new NotImplementedException();
        public static void MakeGradPal(ref Palette pal, byte startColor, Vector startRGB, byte endColor, Vector endRGB) => throw new NotImplementedException();
        public static void Buffer2Screen(ref ScreenBuffer buf) => throw new NotImplementedException();
        public static void ClearBuffer(ref ScreenBuffer buf) => throw new NotImplementedException();
        public static void ClearBufferLines(ref ScreenBuffer buf, int firstLine, int lineCount) => throw new NotImplementedException();
        public static void LoadFont(string fileName) => throw new NotImplementedException();
        public static void PutChar(int x, int y, byte color, char chr) => throw new NotImplementedException();
        public static void OutText(int x, int y, byte color, string text) => throw new NotImplementedException();
        public static void OutTextFmt(int x, int y, byte color, string text) => throw new NotImplementedException();
        public static void HLine(int x, int y, int len, byte color) => throw new NotImplementedException();
        public static void VLine(int x, int y, int len, byte color) => throw new NotImplementedException();
        public static void PutPixel(int x, int y, byte color) => throw new NotImplementedException();
        public static void InitStars(string mode, byte starCount, ref VectorArray stars) => throw new NotImplementedException();
        public static void MoveStars(string mode, byte starCount, int speed, ref VectorArray stars) => throw new NotImplementedException();
        public static void DrawStars(string mode, byte starCount, byte startColor, byte endColor, ref VectorArray stars) => throw new NotImplementedException();
        public static void WaitVsyncStart() => throw new NotImplementedException();
    }
}
