using Microsoft.CodeAnalysis;

namespace IronBlock
{
    public class Statement : IFragment
    {
        public string Name { get; set; }
        public IBlock Block { get; set; }

        public object Evaluate(IContext context)
        {
            if (null == Block)
            {
                return null;
            }

            return Block.Evaluate(context);
        }

        public SyntaxNode Generate(IContext context)
        {
            if (null == Block)
            {
                return null;
            }

            return Block.Generate(context);
        }
    }
}