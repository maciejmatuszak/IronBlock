using Microsoft.CodeAnalysis;

namespace IronBlock
{
    public class Statement : IFragment
    {
        public string Name { get; set; }
        public ABlock ABlock { get; set; }

        public object Evaluate(Context context)
        {
            if (null == ABlock)
            {
                return null;
            }

            return ABlock.Evaluate(context);
        }

        public SyntaxNode Generate(Context context)
        {
            if (null == ABlock)
            {
                return null;
            }

            return ABlock.Generate(context);
        }
    }
}