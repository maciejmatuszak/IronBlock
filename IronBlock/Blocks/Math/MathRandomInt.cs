using System;

namespace IronBlock.Blocks.Math
{
    public class MathRandomInt : ABlock
    {
        private static readonly Random rand = new Random();

        public override object EvaluateInternal(IContext context)
        {
            var from = (double) Values.Evaluate("FROM", context);
            var to = (double) Values.Evaluate("TO", context);
            return (double) rand.Next((int) System.Math.Min(from, to), (int) System.Math.Max(from, to));
        }
    }
}