using Microsoft.CodeAnalysis;

namespace IronBlock
{
    public class Statement : IFragment
    {
        public string Name { get; set; }
        public IBlock Block { get; set; }

        public object Evaluate(Context context)
        {
            if (null == Block)
            {
                return null;
            }

            return Block.Evaluate(context);
        }

        public SyntaxNode Generate(Context context)
        {
            if (null == Block)
            {
                return null;
            }

            return Block.Generate(context);
        }
    }
}