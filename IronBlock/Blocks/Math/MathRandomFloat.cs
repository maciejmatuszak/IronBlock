using System;

namespace IronBlock.Blocks.Math
{
    public class MathRandomFloat : ABlock
    {
        private static readonly Random rand = new Random();

        public override object EvaluateInternal(IContext context)
        {
            return rand.NextDouble();
        }
    }
}