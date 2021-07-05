using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Text
{
    public class TextABlock : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            var text = Fields.Get("TEXT");

            return text;
        }

        public override SyntaxNode Generate(IContext context)
        {
            var text = Fields.Get("TEXT");

            return LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                Literal(text)
            );
        }
    }
}