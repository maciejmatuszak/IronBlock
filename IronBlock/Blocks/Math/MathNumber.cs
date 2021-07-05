using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Math
{
    public class MathNumber : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            return double.Parse(Fields.Get("NUM"), CultureInfo.InvariantCulture);
        }

        public override SyntaxNode Generate(IContext context)
        {
            var value = double.Parse(Fields.Get("NUM"), CultureInfo.InvariantCulture);
            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value)
            );
        }
    }
}