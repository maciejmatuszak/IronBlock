using System;
using System.Linq;

namespace IronBlock.Blocks.Colour
{
    public class ColourRandom : ABlock
    {
        private readonly Random random = new Random();

        public override object EvaluateInternal(IContext context)
        {
            var bytes = new byte[3];
            random.NextBytes(bytes);

            return string.Format("#{0}", string.Join("", bytes.Select(x => string.Format("{0:x2}", x))));
        }
    }
}