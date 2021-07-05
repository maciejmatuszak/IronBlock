using System;

namespace IronBlock.Blocks.Colour
{
    public class ColourBlend : ABlock
    {
        private Random random = new Random();

        public override object EvaluateInternal(IContext context)
        {
            var colour1 = (Values.Evaluate("COLOUR1", context) ?? "").ToString();
            var colour2 = (Values.Evaluate("COLOUR2", context) ?? "").ToString();
            var ratio = System.Math.Min(System.Math.Max((double) Values.Evaluate("RATIO", context), 0), 1);

            if (string.IsNullOrWhiteSpace(colour1) || colour1.Length != 7)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(colour2) || colour2.Length != 7)
            {
                return null;
            }

            var red = (byte) (Convert.ToByte(colour1.Substring(1, 2), 16) * (1 - ratio) +
                              Convert.ToByte(colour2.Substring(1, 2), 16) * ratio);
            var green = (byte) (Convert.ToByte(colour1.Substring(3, 2), 16) * (1 - ratio) +
                                Convert.ToByte(colour2.Substring(3, 2), 16) * ratio);
            var blue = (byte) (Convert.ToByte(colour1.Substring(5, 2), 16) * (1 - ratio) +
                               Convert.ToByte(colour2.Substring(5, 2), 16) * ratio);

            return $"#{red:x2}{green:x2}{blue:x2}";
        }
    }
}