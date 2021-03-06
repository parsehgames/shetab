﻿using System;

namespace SeganX
{
    [Serializable]
    public struct CryptoInt
    {
        public int k;
        public int v;
        public int c;

        public int Value
        {
            get
            {
                int res = Decrypt(v, k);
                return res == c ? res : 0;
            }
        }

        private CryptoInt(int a)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            do { k = rand.Next(int.MinValue, int.MaxValue); } while (k == 0);
            v = Encrypt(a, k);
            c = a;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator CryptoInt(int value)
        {
            return new CryptoInt(value);
        }

        public static implicit operator int(CryptoInt value)
        {
            return value.Value;
        }

        public static implicit operator string(CryptoInt value)
        {
            return value.Value.ToString();
        }

        public static CryptoInt operator ++(CryptoInt input)
        {
            input = input.Value + 1;
            return input;
        }

        public static CryptoInt operator --(CryptoInt input)
        {
            input = input.Value - 1;
            return input;
        }

        private static int Encrypt(int value, int key)
        {
            var v = (value ^ key);
            var res = v + key;
            return res;
        }

        private static int Decrypt(int value, int key)
        {
            var v = (value - key);
            var res = v ^ key;
            return res;
        }
    }
}
