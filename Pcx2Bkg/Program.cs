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
        public byte[,] Tile = new byte[TILE_HEIGHT, TILE_WIDTH];

        public RawTile()
        {
            Tile = new byte[TILE_HEIGHT, TILE_WIDTH];
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



    static PCXImage Img;
    static Palette Pal = new();

    int Planes;
    int HTileCount, VTileCount;

    static RawTile[] Original;
    int[] OrigCheckSums = new int[MAX_TILECOUNT];
    int OptimCount;

    readonly ushort[,] BkgMap = new ushort[2, MAX_TILECOUNT];
    byte[] PalMap = new byte[32];


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
        HTileCount = Img.Width / TILE_WIDTH;
        VTileCount = Img.Height / TILE_HEIGHT;

        InitOriginal();

        for (int l = 0; l < Planes; l++)
        {
            for (int k = 0; k < HTileCount * VTileCount; k++)
            {
                int idx = l * HTileCount * VTileCount + k;
                OrigCheckSums[idx] = 0;

                byte minColor = (byte)(l == 0 ? 0 : 16);
                byte maxColor = (byte)(l == 0 ? 15 : 30);

                for (int i = 0; i < TILE_HEIGHT; i++)
                {
                    for (int j = 0; j < TILE_WIDTH; j++)
                    {
                        int x = (k % HTileCount) * TILE_WIDTH + j;
                        int y = (k / HTileCount) * TILE_HEIGHT + i;
                        byte color = Img.Data[y * Img.Width + x];
                        if (color < minColor || color > maxColor)
                            color = 0;
                        else
                            color = PalMap[color];//TODO массив нигде не заполняется
                        Original[idx].Tile[i, j] = color;
                        OrigCheckSums[idx] += color;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// так как при инициализации массива Original конструкто у каждого элемента не будет вызван, приходиться это делать вручную
    /// </remarks>
    void InitOriginal()
    {
        Original = new RawTile[MAX_TILECOUNT];

        for (int i = 0; i < Original.Length; i++)
        {
            Original[i] = new RawTile();
        }
    }

    void RawToGen(ref RawTile src, ref GenTile dst)
    {
        for (int i = 0; i < TILE_HEIGHT; i++)
        {
            for (int j = 0; j < TILE_WIDTH / 2; j++)
            {
                dst.Data[i, j] = (byte)((src.Tile[i, j * 2] & 0x0F) << 4 | (src.Tile[i, j * 2 + 1] & 0x0F));
            }
        }
    }

    void OutFile(string outputFile)
    {
        using FileStream fs = new(outputFile, FileMode.Create);
        using BinaryWriter writer = new(fs);
        writer.Write(new char[] { 'B', 'K', 'G', '\0' }); // Signature
        writer.Write((ushort)0x0101); // Version
        writer.Write((ushort)OptimCount);
        writer.Write((ushort)HTileCount);
        writer.Write((ushort)VTileCount);
        writer.Write((ushort)Planes);

        for (int i = 0; i < OptimCount; i++)
        {
            GenTile gen = new();
            RawToGen(ref Original[i], ref gen);
            foreach (var b in gen.Data)
                writer.Write(b);
        }

        for (int i = 0; i < Planes; i++)
        {
            for (int j = 0; j < HTileCount * VTileCount; j++)
                writer.Write((ushort)BkgMap[i, j]);
        }

        for (int i = 0; i < Planes; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                int idx = j + (i == 0 ? 0 : 16);
                ushort w = (ushort)((Pal.Colors[idx].B >> 2 & 0x0E) << 8 |
                                    (Pal.Colors[idx].G >> 2 & 0x0E) << 4 |
                                    (Pal.Colors[idx].R >> 2 & 0x0E));
                writer.Write(w);
            }
        }
    }

    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("PCXToBackground by SegaMark");
            Console.WriteLine("USAGE: PCXToBackground <image.pcx> <outfile>");
            Environment.Exit(1);
        }

        var program = new PCXToBackground();

        // Загрузка данных PCX и палитры (предполагается, что функции реализованы)
        Img = PCXHandler.AllocReadPCX(args[0]);
        Pal = PCXHandler.GetPcxPalette(args[0]);

        program.CalcPlanes();
        program.ReadTiles();
        program.OutFile(args[1]);
    }
}

