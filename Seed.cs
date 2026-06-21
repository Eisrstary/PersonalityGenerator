using System;

namespace PersonalityGenerator
{
    /// <summary>
    /// 纯C#确定性伪随机数生成器。
    /// 无外部依赖，适用于所有平台（含WASM/iOS/Android）。
    /// 基于 xorshift128+ 算法。
    /// </summary>
    internal class XorShiftRandom
    {
        private ulong _s0, _s1;

        public XorShiftRandom(ulong seed0, ulong seed1)
        {
            _s0 = seed0 == 0 ? 0xDEADBEEF12345678UL : seed0;
            _s1 = seed1 == 0 ? 0xABCDEF0987654321UL : seed1;
        }

        public ulong NextUInt64()
        {
            ulong s1 = _s0;
            ulong s0 = _s1;
            _s0 = s0;
            s1 ^= s1 << 23;
            _s1 = s1 ^ s0 ^ (s1 >> 18) ^ (s0 >> 5);
            return _s1 + s0;
        }

        public double NextDouble()
        {
            return (NextUInt64() >> 11) / (double)(1UL << 53);
        }

        public float NextFloat()
        {
            return (float)((NextUInt64() >> 40) / (double)(1 << 24));
        }
    }

    /// <summary>
    /// 纯C#确定性种子扩展器。
    /// 替代 SHA256，适用于所有平台。
    /// 使用 FNV-1a + xorshift 将任意整数种子扩展为1024字节。
    /// </summary>
    internal static class SeedExpander
    {
        public static byte[] Expand(int seed, int targetLength)
        {
            byte[] result = new byte[targetLength];
            var rng = new XorShiftRandom((ulong)seed * 6364136223846793005UL,
                                          (ulong)seed ^ 0x9E3779B97F4A7C15UL);

            for (int i = 0; i < targetLength; i += 8)
            {
                ulong value = rng.NextUInt64();
                int remaining = targetLength - i;
                byte[] bytes = BitConverter.GetBytes(value);
                int copyLen = remaining < 8 ? remaining : 8;
                Buffer.BlockCopy(bytes, 0, result, i, copyLen);
            }

            return result;
        }
    }

    /// <summary>
    /// 人格种子生成器。
    /// 1024字节确定性种子 → 驱动84个参数生成。
    /// 无外部依赖，纯计算。
    /// </summary>
    public class Seed
    {
        public const int Length = 1024;
        private readonly byte[] _data;
        private int _pos;
        private int _bitOff;

        public Seed(byte[] data)
        {
            if (data == null || data.Length != Length)
                throw new ArgumentException("Seed must be " + Length + " bytes");
            _data = data;
        }

        public static Seed FromInt(int seed)
        {
            return new Seed(SeedExpander.Expand(seed, Length));
        }

        public static Seed FromString(string hex)
        {
            if (hex == null || hex.Length != Length * 2)
                throw new ArgumentException("Hex string must be " + Length * 2 + " chars");
            byte[] data = new byte[Length];
            for (int i = 0; i < Length; i++)
                data[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return new Seed(data);
        }

        public static Seed Random()
        {
            int tick = Environment.TickCount;
            return FromInt(tick ^ (tick << 13) ^ (int)(DateTime.UtcNow.Ticks & 0xFFFFFFFF));
        }

        public void Reset() { _pos = 0; _bitOff = 0; }

        public double ReadDouble()
        {
            if (_pos + 8 > Length) throw new InvalidOperationException("Seed exhausted");
            ulong v = BitConverter.ToUInt64(_data, _pos);
            _pos += 8;
            return (v >> 11) / (double)(1UL << 53);
        }

        public float ReadFloat()
        {
            if (_pos + 4 > Length) throw new InvalidOperationException("Seed exhausted");
            uint v = BitConverter.ToUInt32(_data, _pos);
            _pos += 4;
            return v / (float)uint.MaxValue;
        }

        public bool ReadBit()
        {
            if (_pos >= Length) throw new InvalidOperationException("Seed exhausted");
            bool r = ((_data[_pos] >> _bitOff) & 1) == 1;
            if (++_bitOff >= 8) { _bitOff = 0; _pos++; }
            return r;
        }

        public byte[] GetRawData() { return (byte[])_data.Clone(); }

        public string ToHex()
        {
            char[] c = new char[Length * 2];
            for (int i = 0; i < Length; i++)
            {
                c[i * 2] = (char)(_data[i] < 160 ? (_data[i] >> 4) < 10 ? '0' + (_data[i] >> 4) : 'A' + (_data[i] >> 4) - 10 : '0');
                c[i * 2 + 1] = (char)((_data[i] & 0xF) < 10 ? '0' + (_data[i] & 0xF) : 'A' + (_data[i] & 0xF) - 10);
            }
            // 简化：直接返回前32字符
            return BitConverter.ToString(_data).Replace("-", "");
        }
    }
}
