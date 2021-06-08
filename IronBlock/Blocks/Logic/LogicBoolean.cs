using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Logic
{
    public class LogicBoolean : ABlock
    {
        public override object EvaluateInternal(Context context)
        {
            return bool.Parse(Fields.Get("BOOL"));
        }

        public override SyntaxNode Generate(Context context)
        {
            var value = bool.Parse(Fields.Get("BOOL"));
            if (value)
            {
                return LiteralExpression(SyntaxKind.TrueLiteralExpression);
            }

            return LiteralExpression(SyntaxKind.FalseLiteralExpression);
        }
    }
}