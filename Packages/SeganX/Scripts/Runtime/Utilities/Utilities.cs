using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace SeganX
{
    public static class Utilities
    {
        public static DateTime UnixTimeToTime(long date)
        {
            DateTime res = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            res = res.AddSeconds(date);
            return res;
        }

        public static DateTime UnixTimeToTime(string date)
        {
            return UnixTimeToTime(long.Parse(date));
        }

        public static DateTime UnixTimeToLocalTime(long date)
        {
            return UnixTimeToTime(date).ToLocalTime();
        }

        public static DateTime UnixTimeToLocalTime(string date)
        {
            return UnixTimeToTime(date).ToLocalTime();
        }

        public static string TimeToString(double time, int decimals)
        {
            int h = (int)time / 3600;
            int m = ((int)time % 3600) / 60;
            double s = time % 60;
            switch (decimals)
            {
                case 1: return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00.0")) : (m.ToString("00") + ":" + s.ToString("00.0"));
                case 2: return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00.00")) : (m.ToString("00") + ":" + s.ToString("00.00"));
                case 3: return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00.000")) : (m.ToString("00") + ":" + s.ToString("00.000"));
            }
            return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00")) : (m.ToString("00") + ":" + s.ToString("00"));
        }

#if SX_ZIP
        public static string CompressString(string text, string failedReturn)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                MemoryStream msi = new MemoryStream(bytes);
                MemoryStream mso = new MemoryStream();
                SevenZip.Compression.LZMA.Encoder enc = new SevenZip.Compression.LZMA.Encoder();
                enc.WriteCoderProperties(mso);
                mso.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                enc.Code(msi, mso, msi.Length, -1, null);
                var res = Convert.ToBase64String(mso.ToArray());
                msi.Close(); msi.Dispose();
                mso.Close(); mso.Dispose();
                return res;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message + " | " + ex.StackTrace);
            }
            return failedReturn;
        }

        public static string DecompressString(string compressedText, string failedReturn)
        {
            if (compressedText.HasContent(10) == false) return failedReturn;
            try
            {
                MemoryStream msi = new MemoryStream(Convert.FromBase64String(compressedText));
                MemoryStream mso = new MemoryStream();
                SevenZip.Compression.LZMA.Decoder dec = new SevenZip.Compression.LZMA.Decoder();
                byte[] props = new byte[5]; msi.Read(props, 0, 5);
                byte[] length = new byte[4]; msi.Read(length, 0, 4);
                int len = BitConverter.ToInt32(length, 0);
                dec.SetDecoderProperties(props);
                dec.Code(msi, mso, msi.Length, len, null);
                var res = Encoding.UTF8.GetString(mso.ToArray());
                msi.Close(); msi.Dispose();
                mso.Close(); mso.Dispose();
                return res;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message + " | " + ex.StackTrace);
            }
            return failedReturn;
        }
#endif

        public static float RandomDoubleHigh(float min, float max)
        {
            if (max < min)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }

            var delta = max - min;
            var r1 = UnityEngine.Random.Range(min, max + delta);
            var r2 = UnityEngine.Random.Range(min, max + delta);
            var res = (r1 + r2) / 2;
            if (res > max) res = 2 * max - res;
            return res;
        }

        public static int RandomDoubleHigh(int min, int max)
        {
            if (max < min)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }

            var delta = max - min;
            var r1 = UnityEngine.Random.Range(min, max + delta);
            var r2 = UnityEngine.Random.Range(min, max + delta);
            var res = (r1 + r2) / 2;
            if (res > max) res = 2 * max - res;
            return res;
        }
    }
}
