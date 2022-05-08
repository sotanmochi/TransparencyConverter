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

            var t_r_srgb = TransferFunction(color_srgb.X);
            var t_g_srgb = TransferFunction(color_srgb.Y);
            var t_b_srgb = TransferFunction(color_srgb.Z);

            var t_rf = TransferFunction(foregroundColor.X);
            var t_gf = TransferFunction(foregroundColor.Y);
            var t_bf = TransferFunction(foregroundColor.Z);

            var t_rb = TransferFunction(backgroundColor.X);
            var t_gb = TransferFunction(backgroundColor.Y);
            var t_bb = TransferFunction(backgroundColor.Z);

            var rf_rb = foregroundColor.X - backgroundColor.X;
            var gf_gb = foregroundColor.Y - backgroundColor.Y;
            var bf_bb = foregroundColor.Z - backgroundColor.Z;

            if (rf_rb == 0f) { rf_rb = 1f; }
            if (gf_gb == 0f) { gf_gb = 1f; }
            if (bf_bb == 0f) { bf_bb = 1f; }

            var top    = ((t_r_srgb - t_rb) * gf_gb * bf_bb) + (rf_rb * (t_g_srgb - t_gb) * bf_bb) + (rf_rb * gf_gb * (t_b_srgb - t_bb));
            var bottom = ((t_rf     - t_rb) * gf_gb * bf_bb) + (rf_rb * (t_gf     - t_gb) * bf_bb) + (rf_rb * gf_gb * (t_bf     - t_bb));

            return (float)(top / bottom);
        }

        public static float TransferFunction(float x)
        {
            return (float)Math.Pow((x + 0.055) / 1.055, 2.4);
        }
    }
}