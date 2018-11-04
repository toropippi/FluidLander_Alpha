using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Loadpngs : MonoBehaviour
{

    //ここからはpng読み込みのやつ
    byte[] ReadPngFile(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);
        bin.Close();
        return values;
    }
    //pngを読み込んでtextureを返す関数
    public Texture2D PicloadPng(string path)
    {
        byte[] readBinary = ReadPngFile(path);
        int pos = 16; // 16バイトから開始
        int width = 0;
        for (int i = 0; i < 4; i++)
        {
            width = width * 256 + readBinary[pos++];
        }
        int height = 0;
        for (int i = 0; i < 4; i++)
        {
            height = height * 256 + readBinary[pos++];
        }
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.LoadImage(readBinary);
        return texture;
    }


    //bmpを読み込んで2次元配列を出力する関数
    public int[,] LoadBmp(string path)
    {
        byte[] readBinary = ReadPngFile(path);
        int pos = 18; // 18バイトから開始
        int width = 0;
        for (int i = 0; i < 3; i++)
        {
            width = width + readBinary[pos + i] * (1 << (8 * i));
        }
        int height = 0;
        pos += 4;
        for (int i = 0; i < 3; i++)
        {
            height = height + readBinary[pos + i] * (1 << (8 * i));
        }
        pos = 54;
        int[,] data = new int[width, height];

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                data[i, height - j - 1] = readBinary[pos++] * 65536;
                data[i, height - j - 1] += readBinary[pos++] * 256;
                data[i, height - j - 1] += readBinary[pos++];
            }
        }
        return data;
    }

    //pngを読み込んで2次元配列を出力する関数
    //これは使えないやつ
    public int[,] LoadPng(string path)
    {
        byte[] readBinary = ReadPngFile(path);
        int pos = 16; // 16バイトから開始
        int width = 0;
        for (int i = 0; i < 4; i++)
        {
            width = width * 256 + readBinary[pos++];
        }
        int height = 0;
        for (int i = 0; i < 4; i++)
        {
            height = height * 256 + readBinary[pos++];
        }
        //ここから本読み込み
        Debug.Log(width);
        Debug.Log(height);
        int[,] data = new int[width, height];
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                data[i, height - j - 1] = readBinary[pos++] * 65536;
                data[i, height - j - 1] += readBinary[pos++] * 256;
                data[i, height - j - 1] += readBinary[pos++];
            }
        }
        return data;
    }
}