using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Logic
{
    public class LogicBoolean : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            return bool.Parse(Fields.Get("BOOL"));
        }

        public override SyntaxNode Generate(IContext context)
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