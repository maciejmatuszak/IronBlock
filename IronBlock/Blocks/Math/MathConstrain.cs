using System;
using IronBlock.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Math
{
    public class MathConstrain : IBlock
    {
        public override object Evaluate(Context context)
        {
            var value = (double) Values.Evaluate("VALUE", context);
            var low = (double) Values.Evaluate("LOW", context);
            var high = (double) Values.Evaluate("HIGH", context);

            return System.Math.Min(System.Math.Max(value, low), high);
        }

        public override SyntaxNode Generate(Context context)
        {
            var valueExpression = Values.Generate("VALUE", context) as ExpressionSyntax;
            if (valueExpression == null) throw new ApplicationException("Unknown expression for value.");

            var lowExpression = Values.Generate("LOW", context) as ExpressionSyntax;
            if (lowExpression == null) throw new ApplicationException("Unknown expression for low.");

            var highExpression = Values.Generate("HIGH", context) as ExpressionSyntax;
            if (highExpression == null) throw new ApplicationException("Unknown expression for high.");

            return
                SyntaxGenerator.MethodInvokeExpression(
                    IdentifierName(nameof(System.Math)),
                    nameof(System.Math.Min),
                    new[]
                    {
                        SyntaxGenerator.MethodInvokeExpression(
                            IdentifierName(nameof(System.Math)),
                            nameof(System.Math.Max),
                            new[] {valueExpression, lowExpression}
                        ),
                        highExpression
                    }
                );
        }
    }
}