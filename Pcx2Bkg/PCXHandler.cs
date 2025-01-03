using System.Runtime.InteropServices;

namespace Pcx2Bkg
{
    // PCX Header structure
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PCXHeader
    {
        public byte Manufacturer;
        public byte Version;
        public byte Encoding;
        public byte BitsPerPixel;
        public ushort XMin, YMin, XMax, YMax;
        public ushort HDPI, VDPI;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] ColorMap16;
        public byte Reserved;
        public byte NPlanes;
        public ushort BytesPerLine;
        public ushort PaletteInfo;
        public ushort HScreenSize;
        public ushort VScreenSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 54)]
        public byte[] Filler;
    }

    public class PCXHandler
    {
        private static PCXHeader pcxHeader;
        private static byte[] pcxBuffer;

        public static void ReadPCX(ref PCXImage img, string fileName)
        {
            LoadPCXHeader(fileName);

            img.Width = pcxHeader.XMax - pcxHeader.XMin + 1;
            img.Height = pcxHeader.YMax - pcxHeader.YMin + 1;
            DecodePCX(ref img);

            Console.WriteLine("PCX Image Loaded Successfully.");
        }

        public static PCXImage AllocReadPCX(string fileName)
        {
            LoadPCXHeader(fileName);

            PCXImage img = new()
            {
                Width = pcxHeader.XMax - pcxHeader.XMin + 1,
                Height = pcxHeader.YMax - pcxHeader.YMin + 1,
                Data = new byte[(pcxHeader.XMax - pcxHeader.XMin + 1) * (pcxHeader.YMax - pcxHeader.YMin + 1)]
            };
            DecodePCX(ref img);

            Console.WriteLine("PCX Image Allocated and Loaded Successfully.");
            return img;
        }

        /// <summary>
        /// Read PCX Palette
        /// </summary>
        /// <param name="pal"></param>
        /// <param name="fileName"></param>
        public static void GetPcxPalette(ref Palette pal, string fileName)
        {
            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(fs);
            fs.Seek(-768, SeekOrigin.End); // Palette is at the end of the file
            byte[] paletteData = reader.ReadBytes(768);

            for (int i = 0; i < 256; i++)
            {
                pal.Colors[i, 0] = (byte)(paletteData[i * 3] >> 2);   // Red
                pal.Colors[i, 1] = (byte)(paletteData[i * 3 + 1] >> 2); // Green
                pal.Colors[i, 2] = (byte)(paletteData[i * 3 + 2] >> 2); // Blue
            }
        }

        private static void LoadPCXHeader(string fileName)
        {
            using FileStream fileStream = new(fileName, FileMode.Open, FileAccess.Read);
            var buffer = new byte[Marshal.SizeOf<PCXHeader>()];
            fileStream.Read(buffer, 0, buffer.Length);

            pcxHeader = ByteArrayToStructure<PCXHeader>(buffer);
            pcxBuffer = new byte[fileStream.Length - buffer.Length];
            fileStream.Read(pcxBuffer, 0, pcxBuffer.Length);
        }

        private static void DecodePCX(ref PCXImage img)
        {
            int xSize = pcxHeader.XMax - pcxHeader.XMin + 1;
            int ySize = pcxHeader.YMax - pcxHeader.YMin + 1;
            int totalBytes = pcxHeader.NPlanes * pcxHeader.BytesPerLine;

            byte[] buffer = new byte[totalBytes];
            int bufferIndex = 0;

            for (int lineNumber = 0; lineNumber < ySize; lineNumber++)
            {
                bufferIndex = DecodeLine(buffer, bufferIndex, totalBytes);

                Array.Copy(buffer, 0, img.Data, lineNumber * xSize, xSize);
            }
        }

        private static int DecodeLine(byte[] buffer, int bufferIndex, int totalBytes)
        {
            int subTotal = 0;

            while (subTotal < totalBytes)
            {
                byte data = pcxBuffer[bufferIndex++];
                int count = 1;

                if ((data & 0xC0) == 0xC0)
                {
                    count = data & 0x3F;
                    data = pcxBuffer[bufferIndex++];
                }

                for (int i = 0; i < count; i++)
                    buffer[subTotal++] = data;
            }

            return bufferIndex;
        }

        private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T obj = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();
            return obj;
        }
    }
}
