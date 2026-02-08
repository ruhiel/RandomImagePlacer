using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomImagePlacer
{
    // メルセンヌ・ツイスタ (MT19937) の簡易実装
    public class MersenneTwister
    {
        private uint[] mt = new uint[624];
        private int index = 625;

        public MersenneTwister(int seed)
        {
            mt[0] = (uint)seed;
            for (var i = 1; i < 624; i++)
            {
                mt[i] = (uint)(1812433253 * (mt[i - 1] ^ (mt[i - 1] >> 30)) + i);
            }
        }

        // 0.0 以上 1.0 未満の乱数を返す
        public double NextDouble() =>
            // 32ビット整数の範囲 (0 ～ 4,294,967,295) を利用
            NextUint() * (1.0 / 4294967296.0);

        public int Next(int min, int max)
        {
            // 範囲が 0 の場合の例外回避
            if (min >= max)
            {
                return min;
            }
            return (int)(NextUint() % (uint)(max - min)) + min;
        }

        private uint NextUint()
        {
            if (index >= 624) { Twist(); }
            var y = mt[index++];
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680;
            y ^= (y << 15) & 0xefc60000;
            y ^= (y >> 18);
            return y;
        }

        private void Twist()
        {
            for (var i = 0; i < 624; i++)
            {
                var y = (mt[i] & 0x80000000) + (mt[(i + 1) % 624] & 0x7fffffff);
                mt[i] = mt[(i + 397) % 624] ^ (y >> 1);
                if (y % 2 != 0)
                {
                    mt[i] ^= 0x9908b0df;
                }
            }
            index = 0;
        }
    }
}
