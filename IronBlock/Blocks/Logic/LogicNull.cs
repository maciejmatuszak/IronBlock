using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Logic
{
    public class LogicNull : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            return null;
        }

        public override SyntaxNode Generate(IContext context)
        {
            return ReturnStatement(
                LiteralExpression(
                    SyntaxKind.NullLiteralExpression
                )
            );
        }
    }
}