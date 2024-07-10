using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public static class Mathf
    {
        public static int Clamp(int value, int min, int max)
        {
            return Math.Max(Math.Min(value, max), min);
        }

        public static float Lerp(float a, float b, float t)
        {
            return a * (1.0f - t) + t * b;
        }

    }
}
