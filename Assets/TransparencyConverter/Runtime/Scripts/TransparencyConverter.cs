// References:
//  - https://en.wikipedia.org/wiki/SRGB#Transformation

using System;
using System.Numerics;

namespace TransparencyConverter
{
    public class TransparencyConverter
    {
        public static float SRGBToLinear(float alpha_srgb, Vector3 foregroundColor, Vector3 backgroundColor)
        {
            var diff = Vector3.Abs(foregroundColor - backgroundColor);
            if (diff == Vector3.Zero)
            {
                return 1f;
            }

            var color_srgb = foregroundColor * alpha_srgb + backgroundColor * (1 - alpha_srgb);

            var cf = 0f;
            var cb = 0f;
            var c  = 0f;

            if (diff.X != 0)
            {
                cf = foregroundColor.X;
                cb = backgroundColor.X;
                c  = color_srgb.X;
            }
            else if (diff.Y != 0)
            {
                cf = foregroundColor.Y;
                cb = backgroundColor.Y;
                c  = color_srgb.Y;
            }
            else if (diff.Z != 0)
            {
                cf = foregroundColor.Z;
                cb = backgroundColor.Z;
                c  = color_srgb.Z;
            }

            if (c <= 0.04045)
            {
                return alpha_srgb;
            }
            else
            {
                var numerator   = (Math.Pow((c  + 0.055) / 1.055, 2.4) - Math.Pow((cb + 0.055) / 1.055, 2.4));
                var denominator = (Math.Pow((cf + 0.055) / 1.055, 2.4) - Math.Pow((cb + 0.055) / 1.055, 2.4));
                return (float)(numerator / denominator);
            }
        }
    }
}