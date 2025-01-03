using System;
using System.IO;

namespace Pcx2Bkg;
public class PCXToBackground
{
    const int MAX_TILECOUNT = 2048;
    const int TILE_WIDTH = 8;
    const int TILE_HEIGHT = 8;

    struct RawTile
    {
        public byte[,] Data;

        public RawTile()
        {
            Data = new byte[TILE_HEIGHT, TILE_WIDTH];
        }
    }

    struct GenTile
    {
        public byte[,] Data;

        public GenTile()
        {
            Data = new byte[TILE_HEIGHT, TILE_WIDTH / 2];
        }
    }

    

    PCXImage Img = new();

    int Planes;
    int Width, Height;

    RawTile[] Original = new RawTile[MAX_TILECOUNT];
    int[] OrigCheckSums = new int[MAX_TILECOUNT];
    int OptimCount;

    readonly ushort[,] BkgMap = new ushort[2, MAX_TILECOUNT];
    byte[] PalMap = new byte[32];

    byte[,] Pal = new byte[32, 3];

    void CalcPlanes()
    {
        int colors = 0;
        for (int i = 0; i < Img.Width * Img.Height; i++)
        {
            if (Img.Data[i] > colors)
                colors = Img.Data[i];
        }
        Planes = colors > 15 ? 2 : 1;
    }

    void ReadTiles()
    {
        Width = Img.Width / TILE_WIDTH;
        Height = Img.Height / TILE_HEIGHT;

        for (int l = 0; l < Planes; l++)
        {
            for (int k = 0; k < Width * Height; k++)
            {
                int idx = l * Width * Height + k;
                OrigCheckSums[idx] = 0;

                byte minColor = (byte)(l == 0 ? 0 : 16);
                byte maxColor = (byte)(l == 0 ? 15 : 30);

                for (int i = 0; i < TILE_HEIGHT; i++)
                {
                    for (int j = 0; j < TILE_WIDTH; j++)
                    {
                        int x = (k % Width) * TILE_WIDTH + j;
                        int y = (k / Width) * TILE_HEIGHT + i;
                        byte color = Img.Data[y * Img.Width + x];
                        if (color < minColor || color > maxColor)
                            color = 0;
                        else
                            color = PalMap[color];
                        Original[idx].Data[i, j] = color;
                        OrigCheckSums[idx] += color;
                    }
                }
            }
        }
    }

    void RawToGen(ref RawTile src, ref GenTile dst)
    {
        for (int i = 0; i < TILE_HEIGHT; i++)
        {
            for (int j = 0; j < TILE_WIDTH / 2; j++)
            {
                dst.Data[i, j] = (byte)((src.Data[i, j * 2] & 0x0F) << 4 | (src.Data[i, j * 2 + 1] & 0x0F));
            }
        }
    }

    void OutFile(string outputFile)
    {
        using (FileStream fs = new FileStream(outputFile, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            writer.Write(new char[] { 'B', 'K', 'G', '\0' }); // Signature
            writer.Write((ushort)0x0101); // Version
            writer.Write((ushort)OptimCount);
            writer.Write((ushort)Width);
            writer.Write((ushort)Height);
            writer.Write((ushort)Planes);

            for (int i = 0; i < OptimCount; i++)
            {
                GenTile gen = new GenTile();
                RawToGen(ref Original[i], ref gen);
                foreach (var b in gen.Data)
                    writer.Write(b);
            }

            for (int i = 0; i < Planes; i++)
            {
                for (int j = 0; j < Width * Height; j++)
                    writer.Write((ushort)BkgMap[i, j]);
            }

            for (int i = 0; i < Planes; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    int idx = j + (i == 0 ? 0 : 16);
                    ushort w = (ushort)((Pal[idx, 2] >> 2 & 0x0E) << 8 |
                                        (Pal[idx, 1] >> 2 & 0x0E) << 4 |
                                        (Pal[idx, 0] >> 2 & 0x0E));
                    writer.Write(w);
                }
            }
        }
    }

    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("PCXToBackground v1.1 by SegaMark");
            Console.WriteLine("USAGE: PCXToBackground <image.pcx> <outfile>");
            Environment.Exit(1);
        }

        var program = new PCXToBackground();

        // Загрузка данных PCX и палитры (предполагается, что функции реализованы)
        PCXHandler.AllocReadPCX(out program.Img, args[0]);
        // GetPCXPalette(program.Pal, args[0]);

        program.CalcPlanes();
        program.ReadTiles();
        program.OutFile(args[1]);
    }
}

