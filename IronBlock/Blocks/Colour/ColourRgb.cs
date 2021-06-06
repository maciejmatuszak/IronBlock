using System;

namespace IronBlock.Blocks.Text
{
    public class ColourRgb : IBlock
    {
        private Random random = new Random();

        public override object Evaluate(Context context)
        {
            var red = Convert.ToByte(Values.Evaluate("RED", context));
            var green = Convert.ToByte(Values.Evaluate("GREEN", context));
            var blue = Convert.ToByte(Values.Evaluate("BLUE", context));

            return $"#{red:x2}{green:x2}{blue:x2}";
        }
    }
}