// References:
//  - https://en.wikipedia.org/wiki/SRGB#Transformation

using System;

namespace TransparencyConverter
{
    public class TransparencyConverter
    {
        public static float SRGBToLinear(float alpha)
        {
            if (alpha >= (1 - 0.04045))
            {
                return (float)(1 - (1 - alpha) / 12.92);
            }
            else
            {
                return (float)(1 - Math.Pow((1 - alpha + 0.055) / 1.055, 2.4));
            }
        }

        public static float SRGBToLinearApproximately(float alpha)
        {
            return (float)(1 - Math.Pow(1 - alpha, 2.2));
        }
    }
}