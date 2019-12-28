﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Crypto
{
    public static class BitArrayExtension
    {
        public static BitArray LeftShift(this BitArray bit, int index)
        {
            //Console.WriteLine("St:" + string.Join(",", GetBinary(bit).Select(x => x ? 1 : 0)));
            for (int i = 0; i < bit.Length; i++)
            {
                if (i + index < bit.Length)
                    bit[i] = bit[i + index];
                else
                    bit[i] = false;
            }
            //Console.WriteLine("Ed:" + string.Join(",", GetBinary(bit).Select(x => x ? 1 : 0)));
            return new BitArray(bit);
        }

        private static bool[] GetBinary(BitArray bitArray)
        {
            bool[] bin = new bool[bitArray.Length];
            bitArray.CopyTo(bin, 0);
            return bin;
        }
    }
    public class DES
    {
        //Propertys
        public string Key { get; private set; }
        //Fields
        private BitArray _binaryKey;
        //Ctor
        public DES(string key)
        {
            Key = key;
            var bytes = Encoding.Unicode.GetBytes(Key);
            if (bytes.Length > 64) throw new ArgumentOutOfRangeException();
            _binaryKey = new BitArray(bytes);
        }
        //Classes
        private class Block
        {
            //Propertys
            public BitArray BitArray { get; set; }
            public int Size { get { return BitArray.Length; } }
            //Ctor
            public Block(byte[] bytes)
            {
                BitArray = new BitArray(bytes);
            }
            public Block(BitArray bits)
            {
                BitArray = new BitArray(bits);
            }
            //Methods
            /// <summary>
            /// Конвертирует биты в массив из байтов
            /// </summary>
            /// <returns></returns>
            public byte[] GetBytes()
            {
                byte[] bytes = new byte[BitArray.Length / 8];
                BitArray.CopyTo(bytes, 0);
                return bytes;
            }
            /// <summary>
            /// Начальная перестановка IP
            /// </summary>
            public void IP()
            {
                int[] pos = { 58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4, 62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8, 57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3, 61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7 };
                BitArray = new BitArray(pos.Select(x => BitArray[x - 1]).ToArray());
            }
            /// <summary>
            /// Обратная перестановка IP ^{-1}
            /// </summary>
            public void INV_IP()
            {
                int[] pos = { 40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31, 38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29, 36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27, 34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25 };
                BitArray = new BitArray(pos.Select(x => BitArray[x - 1]).ToArray());
            }
            #region ROUND
            /// <summary>
            /// Цикл шифрования
            /// </summary>
            /// <param name="key"></param>
            public void Round(BitArray key, int roundIndex)
            {
                List<bool> result = new List<bool>();
                byte[] bytes = GetBytes();
                BitArray L = new BitArray(SubArray(bytes, 0, 4));
                BitArray R = new BitArray(SubArray(bytes, 4, 4));
                BitArray FR = F(R, key, roundIndex);
                L = L.Xor(FR);
                result.AddRange(GetBinary(R));
                result.AddRange(GetBinary(L));
                BitArray = new BitArray(result.ToArray());
            }
            /// <summary>
            /// функция расширения E
            /// </summary>
            /// <param name="bitArray"></param>
            /// <returns></returns>
            private BitArray Expand(BitArray bitArray)
            {
                int[] pos = { 32, 1, 2, 3, 4, 5, 4, 5, 6, 7, 8, 9, 8, 9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17, 16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25, 24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32, 1 };
                return new BitArray(pos.Select(x => bitArray[x - 1]).ToArray());
            }
            //
            #region S
            /// <summary>
            /// Таблица преобразования S
            /// </summary>
            private readonly int[,,] S = new int[,,] {
                {
                    { 14,4,13,1,2,15,11,8,3,10,6,12,5,9,0,7},
                    { 0,15,7,4,14,2,13,1,10,6,12,11,9,5,3,8},
                    { 4,1,14,8,13,6,2,11,15,12,9,7,3,10,5,0},
                    { 15,12,8,2,4,9,1,7,5,11,3,14,10,0,6,13}
                },
                {
                    { 15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10},
                    { 3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5},
                    { 0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15},
                    { 13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9}
                },
                {
                    { 10,0,9,14,6,3,15,5,1,13,12,7,11,4,2,8},
                    { 13,7,0,9,3,4,6,10,2,8,5,14,12,11,15,1},
                    { 13,6,4,9,8,15,3,0,11,1,2,12,5,10,14,7},
                    { 1,10,13,0,6,9,8,7,4,15,14,3,11,5,2,12}
                },
                {
                    { 7,13,14,3,0,6,9,10,1,2,8,5,11,12,4,15},
                    { 13,8,11,5,6,15,0,3,4,7,2,12,1,10,14,9},
                    { 10,6,9,0,12,11,7,13,15,1,3,14,5,2,8,4},
                    { 3,15,0,6,10,1,13,8,9,4,5,11,12,7,2,14}
                },
                {
                    { 2,12,4,1,7,10,11,6,8,5,3,15,13,0,14,9},
                    { 14,11,2,12,4,7,13,1,5,0,15,10,3,9,8,6},
                    { 4,2,1,11,10,13,7,8,15,9,12,5,6,3,0,14},
                    { 11,8,12,7,1,14,2,13,6,15,0,9,10,4,5,3}
                },
                {
                    { 12,1,10,15,9,2,6,8,0,13,3,4,14,7,5,11},
                    { 10,15,4,2,7,12,9,5,6,1,13,14,0,11,3,8},
                    { 9,14,15,5,2,8,12,3,7,0,4,10,1,13,11,6},
                    { 4,3,2,12,9,5,15,10,11,14,1,7,6,0,8,13}
                },
                {
                    { 4,11,2,14,15,0,8,13,3,12,9,7,5,10,6,1},
                    { 13,0,11,7,4,9,1,10,14,3,5,12,2,15,8,6},
                    { 1,4,11,13,12,3,7,14,10,15,6,8,0,5,9,2},
                    { 6,11,13,8,1,4,10,7,9,5,0,15,14,2,3,12}
                },
                {
                    { 13,2,8,4,6,15,11,1,10,9,3,14,5,0,12,7},
                    { 1,15,13,8,10,3,7,4,12,5,6,11,0,14,9,2},
                    { 7,11,4,1,9,12,14,2,0,6,10,13,15,3,5,8},
                    { 2,1,14,7,4,10,8,13,15,12,9,0,3,5,6,11}
                }
            };
            #endregion
            //
            /// <summary>
            /// Перестановка P
            /// </summary>
            /// <returns></returns>
            private BitArray P(BitArray bitArray)
            {
                int[] pos = { 16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26, 5, 18, 31, 10, 2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22, 11, 4, 25 };
                return new BitArray(pos.Select(x => bitArray[x - 1]).ToArray());
            }
            /// <summary>
            /// Генерирование ключей k_i
            /// </summary>
            /// <param name="bitArray"></param>
            /// <returns></returns>
            private BitArray GenerateKey(BitArray bitArray, int roundIndex)
            {
                //TODO: Генерирование ключей k_i
                bool[] bin = GetBinary(bitArray);
                for (int i = 0; i < bin.Length / 8; i++)
                {
                    bool[] subBin = SubArray(bin, i * 8, 8);
                    int c = subBin.Where(x => x == true).Count();
                    if (c % 2 == 0)
                    {
                        bin[i * 8 + 7] = true;
                    }
                }
                int[] pos = { 57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36, 63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4 };
                bool[] CD = pos.Select(x => bin[x - 1]).ToArray();
                BitArray C = new BitArray(SubArray(CD, 0, 28));
                BitArray D = new BitArray(SubArray(CD, 28, 28));
                int[] rpos = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };
                for (int i = 0; i < 28; i++)
                {
                    C.LeftShift(rpos[roundIndex]);
                    D.LeftShift(rpos[roundIndex]);
                }
                bool[] C_bin = GetBinary(C);
                bool[] D_bin = GetBinary(D);
                List<bool> CD_bin = new List<bool>();
                CD_bin.AddRange(C_bin);
                CD_bin.AddRange(D_bin);
                BitArray Key56bit = new BitArray(CD_bin.ToArray());
                int[] comp_pos = { 14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32 };
                return new BitArray(comp_pos.Select(x => Key56bit[x - 1]).ToArray());
            }
            /// <summary>
            /// Основная функция шифрования
            /// </summary>
            /// <param name="bitArray"></param>
            /// <param name="bitKey"></param>
            /// <returns></returns>
            private BitArray F(BitArray bitArray, BitArray bitKey, int roundIndex)
            {
                List<bool> result = new List<bool>();
                BitArray Expanded = Expand(bitArray);
                Expanded = Expanded.Xor(GenerateKey(bitKey, roundIndex));
                int size = 8;
                bool[] binary = GetBinary(Expanded);
                for (int i = 0; i < size; i++)
                {
                    bool[] subBin = SubArray(binary, i * 6, 6);
                    string rankBin = string.Join("", new bool[] { subBin[0], subBin[subBin.Length - 1] }.Select(x => x ? 1 : 0).ToArray());
                    string Bin = string.Join("", SubArray(subBin.Select(x => x ? 1 : 0).ToArray(), 1, 4));
                    byte rank = Convert.ToByte(rankBin.PadLeft(8, '0'), 2);
                    byte B = Convert.ToByte(Bin.PadLeft(8, '0'), 2);
                    byte s = (byte)S[i, rank, B];
                    BitArray S_4bit = new BitArray(new[] { s });
                    bool[] S_4bit_bool = new bool[4];
                    for (int j = 0; j < 4; j++)
                    {
                        S_4bit_bool[j] = S_4bit[j];
                    }
                    result.AddRange(S_4bit_bool);
                }

                return P(new BitArray(result.ToArray()));
            }
            private bool[] GetBinary(BitArray bitArray)
            {
                bool[] bin = new bool[bitArray.Length];
                bitArray.CopyTo(bin, 0);
                return bin;
            }
            #endregion

        }
        //Methods
        /// <summary>
        /// Вырезает массив из существующего масства
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        //
        #region ENCODE
        /// <summary>
        /// Разделяет массив байтов на блоки по 64 бита
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private Block[] GetBlocks(byte[] src)
        {
            int size = src.Length / 8;

            Block[] blocks = new Block[size];
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = new Block(SubArray(src, i * 8, 8));
            }

            return blocks;
        }
        public string Encode(string src)
        {
            Block[] blocks = GetBlocks(Encoding.Unicode.GetBytes(src));
            List<byte> result = new List<byte>();
            foreach (Block block in blocks)
            {
                block.IP();
                for (int i = 0; i < 16; i++)
                    block.Round(_binaryKey, i);
                block.INV_IP();
                result.AddRange(block.GetBytes());
            }
            return Encoding.Unicode.GetString(result.ToArray());
        }
        #endregion
        //
        public string Decode(string src) => throw new NotImplementedException();

    }
}
